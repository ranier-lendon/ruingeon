using Godot;
using System;
using GameGlobals;

public partial class RoomTrigger : Area3D
{
    [Export] public Node3D Door;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print($"{body.Name} entered the room!");
        DoorControl(DoorState.Close);
    }

    private void DoorControl(DoorState state)
    {
        Vector3 targetScale = state == DoorState.Close ? 
            new Vector3(Door.Scale.X, 10f, Door.Scale.Z) : 
            new Vector3(1f, 1f, 1f);
        
        Door.Scale = targetScale;
    }
}
