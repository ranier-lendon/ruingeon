using Godot;

/// <summary>
/// Interface for any CharacterBody3D that uses gravity.
/// Implement this on PlayerControl, Zombie, and any other character.
/// </summary>
public interface IGravity
{
    /// <summary>Gravity scale multiplier (1.0 = normal, 2.0 = double gravity, etc.)</summary>
    float GravityScale { get; }

    /// <summary>Whether the character is currently on the floor.</summary>
    bool IsOnFloor();

    /// <summary>The character's current velocity.</summary>
    Vector3 Velocity { get; set; }
}

/// <summary>
/// Static helper that applies gravity to any IGravity implementor.
/// Call ApplyGravity() inside _PhysicsProcess before MoveAndSlide().
/// </summary>
public static class GravityComponent
{
    /// <summary>
    /// Applies gravity to the character's Y velocity.
    /// </summary>
    /// <param name="character">The character implementing IGravity.</param>
    /// <param name="delta">Physics delta time from _PhysicsProcess.</param>
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

