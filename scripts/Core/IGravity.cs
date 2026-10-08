using Godot;

// Interface for any CharacterBody3D that uses gravity.
public interface IGravity
{
    float GravityScale { get; } // Multiplier: 1.0 = normal, 2.0 = double, etc.
    bool IsOnFloor();
    Vector3 Velocity { get; set; }
}

// Static helper — call ApplyGravity() in _PhysicsProcess before MoveAndSlide().
public static class GravityComponent
{
    public static void ApplyGravity(IGravity character, double delta)
    {
        if (!character.IsOnFloor())
        {
            float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
            Vector3 vel = character.Velocity;
            vel.Y -= gravity * character.GravityScale * (float)delta;
            character.Velocity = vel;
        }
    }
}
