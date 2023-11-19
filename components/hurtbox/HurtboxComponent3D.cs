using Godot;

public partial class HurtboxComponent3D : Area3D {
  [Signal]
  public delegate void HitEventHandler(int damageAmount);

  [Export]
  public HealthComponent healthComponent {get; private set;}

  [Export]
  public bool checkEachFrame { get; private set; }

  [Export] public float DamageModifier = 1.0f;

  [Export]
  public PackedScene hitEffect;

  public override void _Process(double delta) {
    if (Monitoring && checkEachFrame) {
      var areas = GetOverlappingAreas();
      foreach (var area in areas) {
        if (area is HitboxComponent3D)
          HandleHitboxCollision(area as HitboxComponent3D);
      }
    }
  }

  public void _on_HurtboxComponent_area_entered(Area3D area) {
    if (checkEachFrame)
      return;
    if (Monitoring && area is HitboxComponent3D)
      HandleHitboxCollision(area as HitboxComponent3D);
  }

  private void HandleHitboxCollision(HitboxComponent3D hitbox) {
    Hurt(hitbox.DamageAmount);
  }

  public void Hurt(int damageAmount) {
    int damage = (int)(-damageAmount  * DamageModifier);
    healthComponent.UpdateHealth(damage);
    EmitSignal(SignalName.Hit, -damage);
  }
}

