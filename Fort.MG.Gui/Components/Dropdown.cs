using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public class Dropdown : GuiComponent
{
	private Button _button;
	private StackPanel _menu;
	private bool _isOpen;
	private List<string> _items = new();
	private string _selectedItem;

	public event Action<string> OnSelectionChanged;

	public List<string> Items
	{
		get => _items;
		set
		{
			_items = value;
			RebuildMenu();
		}
	}

	public string SelectedItem
	{
		get => _selectedItem;
		set
		{
			if (_selectedItem != value)
			{
				_selectedItem = value;
				_button.Text = _selectedItem;
				OnSelectionChanged?.Invoke(_selectedItem);
			}
		}
	}

	public Dropdown()
	{
		_button = new Button
		{
			Text = "Select...",
			Size = new Vector2(120, 30)
		};
		_button.OnMouseEvent += (_) => ToggleMenu();
		AddComponent(_button);

		_menu = new StackPanel
		{
			AutoSize = true,
			ItemOrientation = Orientation.Vertical,
			IsVisible = false,
			Size = new Vector2(120, 0)
		};
		AddComponent(_menu);
	}

	private void ToggleMenu()
	{
		_isOpen = !_isOpen;
		_menu.IsVisible = _isOpen;
	}

	private void RebuildMenu()
	{
		_menu.Items.Clear();

		foreach (var item in _items)
		{
			var menuItem = new Button
			{
				Text = item,
				Size = new Vector2(120, 30)
			};
			menuItem.OnMouseEvent += (_) => SelectItem(item);
			_menu.Items.Add(menuItem);
		}
	}

	private void SelectItem(string item)
	{
		SelectedItem = item;
		ToggleMenu();
	}

	protected override void UpdateTransforms()
	{
		base.UpdateTransforms();

		_button.Position = Position;
		_menu.Position = new Vector2(Position.X, Position.Y + _button.Size.Y + 2);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (_isOpen && !Bounds.Contains(Canvas.MousePosition) && Input.LeftClick)
		{
			ToggleMenu(); // Close menu if clicked outside
		}
	}
}
