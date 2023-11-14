using Godot;

public partial class PlayerFirstPerson3D : CharacterBody3D
{
	// General components
	[Export]
	public Camera3D WorldCamera {get; set;}
	[Export]
	public Camera3D FpsLayerCamera {get; set;}
	[Export]
	public AnimationPlayer AnimPlayer {get; private set;}
	// [Export]
	// public AnimationTree AnimTree {get; private set;}
	public AnimationNodeStateMachinePlayback AnimStateMachine {get; private set;} 
	[Export]
	public InteractorComponent3D interactor {get; private set;}
	[Export]
	public StateMachine Fsm {get; private set;}

	// Mouse look
	[ExportGroup("Mouse Settings")]
	[Export(PropertyHint.Range, "0,10,1,")]
	public float MouseSensitivity = 1;

	[Export(PropertyHint.Range, "0,90,1,")]
	public float CameraMaxAngle {get; private set;} = 75;
	private const float MouseScale = 0.001f;

	//Physics
	[ExportGroup("Physics Settings")]
	[Export]
	public float Speed {get; private set;} = 3.0f;
	[Export]
	public float JumpVelocity {get; private set;} = 4.5f;

	//Other
	[ExportGroup("Other settings")]
	[Export]
	public bool Crouched {get; private set;}

	// Input
	private Vector2 InputDir = new Vector2();
	//MouseDir
	private Vector2 MouseDir, PrevMouseDir = new Vector2();
	private Vector2 AttackDir = new Vector2();

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity {get; private set;} = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	// --- READY ---
	public override void _Ready()
	{
		// stateMachine.Init(this);
		// AnimStateMachine = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/playback");
		if(interactor != null)
			interactor.InteractionUpdate += OnInteractionUpdate;
	}

	// --- PROCESS ----
	public override void _Process(double delta)
	{
		InputDir = Godot.Input.GetVector("move_left", "move_right", "move_up", "move_down");
		
		if(interactor != null && Input.IsActionPressed("use"))
			interactor.Interact();

		if(Input.IsActionJustPressed("crouch"))
			ToggleCrouch();

		PrevMouseDir = MouseDir;
		MouseDir = Vector2.Zero;
		FpsLayerCamera.GlobalTransform = WorldCamera.GlobalTransform;
	}

	// --- PHYSICS PROCESS ---
	public override void _PhysicsProcess(double delta)
	{
		var vel = GetMovementVelocity(delta);

		var canRun = !Crouched && IsOnFloor() && InputDir.Y < 0;
		if(Crouched && IsOnFloor())
			vel *= new Vector3(0.5f, 1, 0.5f);
		else if(canRun && Input.IsActionPressed("run"))
			vel *= new Vector3(2, 1, 2);
		Velocity = vel;
		MoveAndSlide();
	}

	// --- INPUT --
	public override void _Input(InputEvent @event) 
	{
		if(@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) 
		{
			var mouseEvent = @event as InputEventMouseMotion;
			LookAround(mouseEvent);
		}
	}

	public void OnInteractionUpdate(InteractionStatus status, float progress)
	{
		GD.Print(status + ", PROGRESS: " + progress + "%");
	}

	public Vector3 GetMovementVelocity(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Godot.Input.IsActionJustPressed("jump") && IsOnFloor()) {
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector3 direction = (Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		return velocity;
	}

	public void LookAround(InputEventMouseMotion mouseEvent)
	{
		MouseDir = mouseEvent.Relative.Normalized();
		RotateY(-mouseEvent.Relative.X * MouseScale * MouseSensitivity);
		float camRotationAmount = -mouseEvent.Relative.Y * MouseScale;
		var newCamRotation = WorldCamera.Rotation;
		var maxAngle = Mathf.DegToRad(CameraMaxAngle);
		var newXRotation = newCamRotation.X + camRotationAmount;
		newCamRotation.X = Mathf.Clamp(newXRotation, -maxAngle, maxAngle);
		WorldCamera.Rotation = newCamRotation;
	}

	public void ToggleCrouch()
	{
		if(Crouched)
			AnimPlayer.PlayBackwards("Crouch");
		else
			AnimPlayer.Play("Crouch");

		Crouched = !Crouched;
	}
}
