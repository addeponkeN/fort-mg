using Fort.MG.EntitySystem;
using Fort.MG.Extensions;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;

namespace Fort.MG.Editor.Panels;

/// <summary>
/// Schematic viewport for the working entity tree: each entity is drawn as a gizmo rectangle at its
/// world position/size, the selection is outlined, and clicking selects while dragging moves.
///
/// Deviation from the plan (stated rather than silently substituted): this is a 2D gizmo/bounds
/// viewport drawn into the editor canvas rather than a live textured render hosted in a dedicated
/// engine <c>Scene</c>. A second <c>Scene</c> cannot simply be drawn alongside the active one -
/// <c>SceneManager</c> owns the single active scene and <c>Scene.Draw</c> blits to the whole back
/// buffer - and driving one manually would duplicate the scene draw path and risk the "do not change
/// update/draw ordering" constraint (AI_CONTEXT.md §13). Drawing only bounds/gizmos also avoids
/// re-entering <c>SpriteBatch.Begin</c> from inside the canvas transform, which the sprite path would
/// require. Rendering real sprites in the viewport is a natural follow-up.
/// </summary>
public sealed class EditorPreview : GuiComponent
{
	private const float MinExtent = 4f;

	private readonly EntityEditorSession _session;
	private readonly List<Entity> _buffer = new();

	private Entity? _dragging;
	private Vector2 _dragLast;
	private bool _dragMoved;

	public EditorPreview(EntityEditorSession session, Vector2 position, Vector2 size)
	{
		_session = session;
		Position = position;
		Size = size;
		Style.Background = new Color(18, 18, 22, 255);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (Canvas == null)
			return;

		var mouse = Canvas.MousePosition;
		var inside = Bounds.Contains(mouse);

		if (Input.LeftClick)
		{
			var hit = inside ? HitTest(mouse) : null;

			if (hit != null)
				_session.Select(hit);

			_dragging = hit;
			_dragLast = mouse;
			_dragMoved = false;
		}

		if (_dragging != null && Input.LeftHold)
		{
			var delta = mouse - _dragLast;
			if (delta != Vector2.Zero)
			{
				// Position3 converts world -> local against the parent, so dragging a child moves it
				// correctly relative to its parent.
				_dragging.Transform.Position3 += new Vector3(delta.X, delta.Y, 0f);
				_dragLast = mouse;
				_dragMoved = true;
			}
		}

		if (Input.LeftRelease)
		{
			// One undo entry per drag rather than one per mouse-move.
			if (_dragging != null && _dragMoved)
				_session.Commit();

			_dragging = null;
			_dragMoved = false;
		}
	}

	public override void Draw()
	{
		if (!IsVisible)
			return;

		var bounds = Bounds;
		bounds.DrawRec(Style.Background);
		bounds.DrawLined(Color.DimGray);

		var root = _session.Root;
		if (root == null)
			return;

		// Reused buffer: no per-frame allocation in the draw path (AI_CONTEXT.md §15).
		EntityEditorSession.FlattenInto(root, _buffer);

		for (var i = 0; i < _buffer.Count; i++)
		{
			var entity = _buffer[i];
			var rect = EntityRect(entity);

			if (ReferenceEquals(entity, _session.SelectedEntity))
			{
				var highlight = rect;
				highlight.Inflate(2, 2);
				highlight.DrawLined(Color.Orange);
			}

			rect.DrawLined(Color.SteelBlue);
		}
	}

	private Rectangle EntityRect(Entity entity)
	{
		var world = entity.Transform.Position;
		var size = entity.Transform.Size;

		return new Rectangle(
			(int)(Bounds.X + world.X),
			(int)(Bounds.Y + world.Y),
			(int)Math.Max(size.X, MinExtent),
			(int)Math.Max(size.Y, MinExtent));
	}

	/// <summary>Topmost entity whose bounds contain <paramref name="point"/> (last one wins).</summary>
	private Entity? HitTest(Vector2 point)
	{
		var root = _session.Root;
		if (root == null)
			return null;

		Entity? found = null;
		EntityEditorSession.FlattenInto(root, _buffer);

		for (var i = 0; i < _buffer.Count; i++)
		{
			if (EntityRect(_buffer[i]).Contains(point))
				found = _buffer[i];
		}

		return found;
	}
}
