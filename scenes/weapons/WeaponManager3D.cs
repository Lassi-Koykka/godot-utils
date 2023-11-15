using Godot;

public partial class WeaponManager3D : Node3D
{
	[Export]
	public HitscanComponent3D hitscanRay {get; private set;}
	// Called when the node enters the scene tree for the first time.

	private Godot.Collections.Array<Weapon> weapons = new Godot.Collections.Array<Weapon>();
	public int currentWeaponIndex = 0;
	private int prevWeaponIndex;
	private Weapon currentWeapon;

	public override void _Ready()
	{
		foreach (var node in this.GetChildren()) {
			if(node is Weapon) weapons.Add(node as Weapon);
		}
		ChangeWeapon(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("attack"))
			Attack();
	}

	private void ChangeWeapon(int index) {
		foreach (var weapon in weapons) {
			RemoveChild(weapon);
		}
		currentWeapon = weapons[0];
		hitscanRay.TargetPosition = new Vector3(0, 0, -currentWeapon.stats.range);
		AddChild(currentWeapon);
	}

	public void Attack()
	{
		GD.Print("BANG");
		currentWeapon.GetNode<AnimationPlayer>("AnimationPlayer").Play("shoot");
		var enemyHurtBox = hitscanRay.GetHurtbox();
		if(enemyHurtBox != null) {
			enemyHurtBox.Hurt(currentWeapon.stats.Damage);
		}
	}
}
