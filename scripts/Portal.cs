using Godot;
using System;

public partial class Portal : Area3D
{
    [Export] public String Path = "res://scenes/dungeon.tscn";

    public override void _Ready()
    {
        BodyEntered += PortalBodyEntered;
    }

    private void PortalBodyEntered(Node3D body)
    {
        GetTree().ChangeSceneToFile(Path);
    }
}
