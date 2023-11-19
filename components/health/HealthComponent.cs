using Godot;

public partial class HealthComponent : Node
{
    // Declare member variables here. Examples:
    [Export]
    public int health { get; private set; } = 100;

    [Export]
    public int maxHealth { get; private set; } = 100;

    [Export]
    public Timer immunityTimer {get; private set; }

    [Signal]
    public delegate void HealthChangedEventHandler(int amount);

    [Signal]
    public delegate void DiedEventHandler();

    public bool Enabled = true;

    public override void _Ready()
    {
        if(immunityTimer != null)
            immunityTimer.Timeout += () => Enabled = true;
    }

    public void UpdateHealth(int amount)
    {

        if(!Enabled) {
            return;
        }

        var newHealth = health + amount;

        if(amount < 0) {
            health = newHealth < 0 ? 0 : newHealth;
            if(health <= 0) {
                EmitSignal(SignalName.Died);
            }
        }
        else if(amount > 0) {
            health = newHealth > maxHealth ? maxHealth : newHealth;
        }

        if(immunityTimer != null) {
            Enabled = false;
            if(!IsDead()) {
                immunityTimer.Start();
            }

        }
        EmitSignal(SignalName.HealthChanged, amount);
    }

    public void Heal(int amount) 
    {
        UpdateHealth(amount);
    }

    public void Damage(int amount) {
        UpdateHealth(-amount);
    }

    public void HealToMax() 
    {
        Heal(maxHealth);
    }

    public bool IsDead() {
        return health <= 0;
    }
}
