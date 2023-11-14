using Godot;

public partial class HurtboxComponent3D : Area3D
{
    [Signal]
    public delegate void HitEventHandler(int damageAmount);

    [Export]
    public HealthComponent healthComponent;

    [Export]
    public float DamageModifier = 1.0f;

    [Export]
    public PackedScene hitEffect;

    public void _on_HurtboxComponent_area_entered(Area3D area)
    {
        if(area is HitboxComponent3D) {
            int damageAmount = (int)((area as HitboxComponent3D).DamageAmount * DamageModifier);
            Hurt(damageAmount);
        }
    }

    public void Hurt(int damageAmount) {
        healthComponent.UpdateHealth(-damageAmount);
        EmitSignal(SignalName.Hit, -damageAmount);
    }
}
