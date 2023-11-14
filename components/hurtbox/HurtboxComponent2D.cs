using Godot;

public partial class HurtboxComponent2D : Area2D
{
    [Signal]
    public delegate void HitEventHandler(int damageAmount);

    [Export]
    public float DamageModifier = 1.0f;

    [Export]
    public PackedScene hitEffect;

    public void _on_HurtboxComponent_area_entered(Area2D area)
    {
        if(area is HitboxComponent2D) {
            float damageAmount = (area as HitboxComponent2D).DamageAmount * DamageModifier;
            EmitSignal(SignalName.Hit, -damageAmount);
        }
    }
}
