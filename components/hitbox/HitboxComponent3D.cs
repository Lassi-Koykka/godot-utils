using Godot;

public partial class HitboxComponent3D : Area3D
{
	[Export]
	public int DamageAmount { get; set; } = 1;
}

