using Fort.Utility;

namespace Fort.MG.TilemapEngine;

public class WeightedVariantSelector(TileDataManager tileDataManager)
{
	private readonly struct TileWeightKey(byte type, TileOrientation orientation) : IEquatable<TileWeightKey>
	{
		private readonly byte _type = type;
		private readonly TileOrientation _orientation = orientation;

		public bool Equals(TileWeightKey other) =>
			_type == other._type && _orientation == other._orientation;

		public override bool Equals(object obj) => obj is TileWeightKey other && Equals(other);

		public override int GetHashCode() =>
			HashCode.Combine(_type, (int)_orientation);

		public override string ToString()
		{
			return $"{_type},{_orientation}";
		}
	}

	private static readonly Dictionary<string, TileOrientation> OrientationLookup =
		Enum.GetValues<TileOrientation>()
			.ToDictionary(e => e.ToString().ToLowerInvariant(), e => e);

	private readonly Dictionary<TileWeightKey, WeightedVariantData> _cache = new();

	public byte SelectVariant(byte tileType, TileOrientation orientation)
	{
		var key = new TileWeightKey(tileType, orientation);
		if (!_cache.TryGetValue(key, out var weightedData))
		{
			var chances = GetChances(tileType, orientation);
			weightedData = new WeightedVariantData(chances);
			_cache[key] = weightedData;
		}
		return weightedData.SelectWeightedVariant();
	}

	public int GetVariantCount(byte tileType, TileOrientation orientation)
	{
		var chances = GetChances(tileType, orientation);
		return chances.Length;
	}

	public int[] GetChances(byte tileType, TileOrientation orientation)
	{
		var tileset = tileDataManager.GetTilesetData(tileType);
		if (tileset == null || tileset.Variances == null)
			return [1];

		foreach (var variance in tileset.Variances)
		{
			if (variance.Orientation is not null &&
				OrientationLookup.TryGetValue(variance.Orientation.ToLowerInvariant(), out var ori) &&
				ori == orientation)
			{
				return variance.Chances?.Length > 0 ? variance.Chances : [1];
			}
		}
		return [1];
	}

	public void ClearCache() => _cache.Clear();
}

internal class WeightedVariantData
{
	public int[] Chances { get; private set; }
	public int TotalWeight { get; private set; }
	public int VariantCount => Chances.Length;

	public WeightedVariantData(int[] chances)
	{
		if (chances == null || chances.Length == 0)
		{
			Chances = [1];
		}
		else
		{
			Chances = chances.Select(c => Math.Max(1, c)).ToArray();
		}
		TotalWeight = Chances.Sum();
	}

	public byte SelectWeightedVariant()
	{
		if (Chances.Length == 1) return 0;

		int randomValue = Rng.Next(0, TotalWeight);
		int currentWeight = 0;

		for (int i = 0; i < Chances.Length; i++)
		{
			currentWeight += Chances[i];
			if (randomValue < currentWeight)
			{
				return (byte)i;
			}
		}
		return (byte)(Chances.Length - 1);
	}
}