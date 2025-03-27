
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui;

public class KeyRepeater
{
	private double _repeatTimer = 0;
	private double _repeatDelay;
	private double _repeatRate;
	private Keys? _heldKey = null;
	private Action _action;

	public KeyRepeater(double repeatDelay = 0.4, double repeatRate = 0.05)
	{
		_repeatDelay = repeatDelay;
		_repeatRate = repeatRate;
	}

	public void Update(GameTime gameTime)
	{
		if (_heldKey == null) return;

		_repeatTimer -= gameTime.ElapsedGameTime.TotalSeconds;
		if (_repeatTimer <= 0)
		{
			_repeatTimer += _repeatRate; // Reset timer for next repeat
			_action?.Invoke();
		}
	}

	public void Start(Keys key, Action action)
	{
		if (_heldKey != key)
		{
			_heldKey = key;
			_repeatTimer = _repeatDelay; // Initial delay before repeating
			_action = action;
			action.Invoke(); // Execute immediately on first press
		}
	}

	public void Stop(Keys key)
	{
		if (_heldKey == key)
		{
			_heldKey = null;
			_repeatTimer = 0;
			_action = null;
		}
	}
}
