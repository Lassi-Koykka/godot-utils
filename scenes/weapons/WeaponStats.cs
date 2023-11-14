using Godot;

enum FireMode {
	SemiAuto,
	Auto,
}

public partial class WeaponStats : Resource {
	[Export]
	public int Damage { get; set; }

	[Export]
	public int range { get; set; }

	[Export]
	public int magazineSize = 1;

	[Export]
	public int reserveAmmoMax = 1;

	[Export]
	public float shotCooldown = 0.5f;

	[Export]
	public float reloadTime = 1f;

	[Export(PropertyHint.Range, "0,100")]
	public float horizontalSway = 0;

	[Export(PropertyHint.Range, "0,100")]
	public float verticalSway = 0;

	[Export(PropertyHint.Range, "0,100")]
	public float horizontalRecoil = 0;

	[Export(PropertyHint.Range, "0,100")]
	public float verticalRecoil = 0;
}
