using Godot;

[GlobalClass]
public partial class HitboxComponent2D : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int DamageAmount { get; set; } = 1;

    public void _on_HitboxComponent_area_entered(Area2D area)
    {
        if (area is HurtboxComponent2D)
        {
            EmitSignal(SignalName.Hit);
        }
    }
}
