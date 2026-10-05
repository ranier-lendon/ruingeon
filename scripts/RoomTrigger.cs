using Godot;
using System;

public partial class RoomTrigger : Area3D
{

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print($"{body.Name} entered the room!");
    }
}
