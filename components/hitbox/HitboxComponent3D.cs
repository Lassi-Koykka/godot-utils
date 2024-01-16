using Godot;

[GlobalClass]
public partial class HitboxComponent3D : Area3D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int DamageAmount { get; set; } = 1;

    public void _on_HitboxComponent_area_entered(Area3D area)
    {
        if (area is HurtboxComponent3D)
        {
            EmitSignal(SignalName.Hit);
        }
    }
}

