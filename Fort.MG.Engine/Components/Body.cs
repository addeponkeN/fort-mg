using Fort.MG.EntitySystem;
using Microsoft.Xna.Framework;

namespace Fort.MG.Components;

public class Body : Component
{
    private const float MovementThreshold = 0.001f;

    private bool _movedThisFrame = false;
    private bool _movedPreviousFrame = false;

    [Serialize]
    public float Mass = 1f;

    public Vector2 Velocity;

    private float _timeSinceLastMove;

    public bool IsMoving =>
        _movedPreviousFrame || _timeSinceLastMove < 0.01f || Velocity.LengthSquared() > MovementThreshold;

    public void ApplyMovement(Vector2 movement)
    {
        _movedThisFrame = true;
        Transform.Position3 += new Vector3(movement, 0f);
        _timeSinceLastMove = 0f;
    }

    public override void Update(IGameTime t)
    {
        _movedThisFrame = false;

        if (Velocity != Vector2.Zero)
        {
            ApplyMovement(Velocity * t.Delta);
        }

        _movedPreviousFrame = _movedThisFrame;
        _timeSinceLastMove += t.Delta;
    }
}
