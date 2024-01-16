using Godot;

[GlobalClass]
public partial class HurtboxComponent2D : Area2D
{
    [Signal]
    public delegate void HitEventHandler(int damageAmount);

    [Export]
    public HealthComponent healthComponent { get; private set; }

    [Export]
    public bool checkEachFrame { get; private set; }

    [Export] public float DamageModifier = 1.0f;

    [Export]
    public PackedScene hitEffect;

    public override void _Process(double delta)
    {
        if (Monitoring && checkEachFrame)
        {
            var areas = GetOverlappingAreas();
            foreach (var area in areas)
            {
                if (area is HitboxComponent2D)
                    HandleHitboxCollision(area as HitboxComponent2D);
            }
        }
    }

    public void _on_HurtboxComponent_area_entered(Area2D area)
    {
        if (checkEachFrame)
            return;
        if (Monitoring && area is HitboxComponent2D)
            HandleHitboxCollision(area as HitboxComponent2D);
    }

    private void HandleHitboxCollision(HitboxComponent2D hitbox)
    {
        Hurt(hitbox.DamageAmount);
    }

    public void Hurt(int damageAmount)
    {
        int damage = (int)(-damageAmount * DamageModifier);
        healthComponent.UpdateHealth(damage);
        EmitSignal(SignalName.Hit, -damage);
    }
}
