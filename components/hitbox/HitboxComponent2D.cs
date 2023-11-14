using Godot;

public partial class HitboxComponent2D : Area2D
{
	[Export]
	public int DamageAmount { get; set; } = 1;
}
