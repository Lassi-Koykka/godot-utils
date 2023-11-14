using Godot;

public partial class Enemy3D : CharacterBody3D
{
	[Export]
	private StateMachine stateMachine;
	[Export]
	public AnimationPlayer animationPlayer {get; private set;}
	[Export]
	public HealthComponent healthComponent {get; private set;}
	[Export]
	public HurtboxComponent3D hurtboxComponent {get; private set;}

	[ExportGroup("Movement Options")]
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		if(stateMachine != null)
			stateMachine?.Init(this);
	}
}
