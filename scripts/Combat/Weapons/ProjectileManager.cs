using Godot;
using System.Collections.Generic;

public partial class ProjectileManager : Node
{
    public static ProjectileManager Instance { get; private set; }

    private PackedScene _projectile { get; set; }
    private List<Projectile> _pool = new();
    private int _poolSize = 30;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
            return;
        }

        _projectile = ResourceLoader.Load<PackedScene>("res://scenes/arrow.tscn");

        for (int i = 0; i < _poolSize; i++)
        {
            var instance = _projectile.Instantiate<Projectile>();
            AddChild(instance);
            _pool.Add(instance);
        }
    }

    public Projectile GetProjectile()
    {
        if (_pool.Count > 0)
        {
            var projectile = _pool[0];
            _pool.RemoveAt(0);
            return projectile;
        }
        else
        {
            var instance = _projectile.Instantiate<Projectile>();
            AddChild(instance);
            return instance;
        }
    }

    public void ReturnProjectile(Projectile projectile)
    {
        _pool.Add(projectile);
    }
}