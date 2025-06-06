﻿using Fort.MG.Extensions;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fort.MG.Gui;

public class FocusManager
{
	private Container _rootContainer;
	public GuiComponent? FocusedComponent { get; private set; }

	private GuiComponent _lastFocusedComponent;

	public void Update(Container rootContainer)
	{
		_rootContainer = rootContainer;
	}

	private IEnumerable<GuiComponent> GetAllComponents(GuiComponent component)
	{
		if (!component?.IsEnabled ?? false) yield break;

		yield return component;

		if (component is Container container)
		{
			foreach (var child in container.Items)
			{
				foreach (var descendant in GetAllComponents(child))
					yield return descendant;
			}
		}
	}

	private bool IsComponentInTree(GuiComponent component)
	{
		return _rootContainer != null && GetAllComponents(_rootContainer).Contains(component);
	}

	public void SetFocus(GuiComponent? component)
	{
		if (FocusedComponent == component) return;

		FocusedComponent?.OnFocus(false);

		if (component != null && component.IsEnabled && IsComponentInTree(component) && component.IsFocusable)
		{
			FocusedComponent = component;
			FocusedComponent.OnFocus(true);
			_lastFocusedComponent = FocusedComponent;
		}
		else
		{
			FocusedComponent = null;
		}
	}

	public void ClearFocus()
	{
		if (FocusedComponent != null)
		{
			FocusedComponent.OnFocus(false);
			FocusedComponent = null;
		}
	}

	public void HandleMouseClick(Vector2 mousePosition)
	{
		var allComponents = GetAllComponents(_rootContainer).ToList();
		if (allComponents.Count <= 0) return;
		var newFocus = allComponents.LastOrDefault(c => c != null && c.IsFocusable && c.IsVisible && c.Bounds.Contains(mousePosition));

		SetFocus(newFocus);
	}

	private void CycleFocus(int direction)
	{
		var focusableComponents = GetAllComponents(_rootContainer)
			.Where(c => c.IsFocusable && c.IsVisible)
			.ToList();

		if (focusableComponents.Count == 0) return;

		int count = focusableComponents.Count;
		int startIndex = FocusedComponent != null ? focusableComponents.IndexOf(FocusedComponent) : -1;

		int nextIndex = (startIndex + direction + count) % count;

		SetFocus(focusableComponents[nextIndex]);
	}

	//hmm
	public void DrawSelection()
	{
		if (FocusedComponent == null) return;
		//FocusedComponent.Bounds.DrawLined(Color.White);
	}

	public void UpdateInput(InputHandlerArgs frameInputHandler)
	{
		if (Input.KeyClick(Keys.Tab))
		{
			int direction = Input.ShiftDown ? -1 : 1;
			CycleFocus(direction);
		}
		else if (FocusedComponent != null && Input.KeyClick(Keys.Enter))
		{
			FocusedComponent.OnTrigger();
		}
	}
}
