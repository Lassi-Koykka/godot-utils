using Godot;

public partial class HealthComponent : Node
{
    // Declare member variables here. Examples:
    [Export]
    public int Health { get; private set; } = 100;

    [Export]
    public int MaxHealth { get; private set; } = 100;

    [Export]
    public float ImmunityTime {get; private set; } = 0.5f;

    [Signal]
    public delegate void HealthChangedEventHandler(int amount);

    [Signal]
    public delegate void DiedEventHandler();

    public bool Enabled = true;

    public void UpdateHealth(int amount)
    {

        if(!Enabled) {
            GD.Print("Immune");
            return;
        }

        GD.Print("Updating health:" + amount);

        if(amount < 0)
            Damage(-amount);
        else if(amount > 0)
            Heal(amount);

        Enabled = false;
        if(!IsDead()) {
            var timer = GetTree().CreateTimer(ImmunityTime);
            timer.Timeout += () => Enabled = true;
        }
        EmitSignal(SignalName.HealthChanged, amount);
    }

    public void Heal(int amount) 
    {
        var newHealth = Health + amount;
        Health = newHealth > MaxHealth ? MaxHealth : newHealth;
        EmitSignal(SignalName.HealthChanged, amount);
    }

    public void HealToMax() 
    {
        Heal(MaxHealth);
    }

    public void Damage(int amount) {
        var newHealth = Health - amount;
        Health = newHealth < 0 ? 0 : newHealth;
        EmitSignal(SignalName.HealthChanged, amount);
        if(Health <= 0) {
            EmitSignal(SignalName.Died);
        }
    }

    public bool IsDead() {
        return Health <= 0;
    }
}
