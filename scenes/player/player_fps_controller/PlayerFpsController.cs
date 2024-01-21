using Godot;

public partial class PlayerFpsController : CharacterBody3D
{
    [Export] public Camera3D Cam { get; private set; }
    [Export] public Node3D Head { get; private set; }
    [Export] public Node3D Arms { get; private set; }
    [Export] public CollisionShape3D StandingCollisionShape { get; private set; }
    [Export] public CollisionShape3D CrouchingCollisionShape { get; private set; }
    [Export] public RayCast3D CrouchCheckRay { get; private set; }

    // Mouse look
    [ExportGroup("Mouse Settings")]
    [Export(PropertyHint.Range, "0,10,1,")]
    public float MouseSensitivity = ProjectSettings.GetSetting("player/look_sensitivity", 0.0f).As<float>();

    [Export(PropertyHint.Range, "0,90,1,")]
    public float CameraMaxAngle { get; private set; } = 90;
    public Vector2 InputDir { get; private set; } = new Vector2();
    public Vector2 MouseInput { get; private set; } = new Vector2();

    // Physics
    [ExportGroup("Physics Settings")]
    [Export] public float Gravity { get; private set; } = 12.0f;
    [Export] public float Acceleration { get; private set; } = 10f;
    [Export] public float Deceleration { get; private set; } = 10f;
    [Export] public float WalkSpeed { get; private set; } = 8.0f;
    [Export] public float RunSpeed { get; private set; } = 16.0f;
    [Export] public float CrouchWalkSpeed { get; private set; } = 4.0f;
    [Export] public float JumpVelocity { get; private set; } = 8f;
    [Export] public float AirControl { get; private set; } = 0.25f;
    [Export] public float SlidingDeceleration { get; private set; } = 3f;
    [Export] public float SlideSpeedBoost { get; private set; } = 20.0f;
    [Export] public float CrouchChangeSpeed { get; private set; } = 10.0f;

    //State
    public bool Jumping { get; private set; }
    public bool Running { get; private set; }
    public bool Crouching { get; private set; }
    public bool Sliding { get; private set; }

    float defaultHeadHeight, crouchingHeadHeight = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        defaultHeadHeight = Head.Position.Y;
        crouchingHeadHeight = defaultHeadHeight - ((StandingCollisionShape.Shape as CylinderShape3D).Height - (CrouchingCollisionShape.Shape as CylinderShape3D).Height);
    }

    public override void _PhysicsProcess(double delta)
    {

        // Get move direction
        InputDir = Godot.Input.GetVector("move_left", "move_right", "move_forward",
                                         "move_backward");
        var direction = (Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y)).Normalized();

        // Apply gravity
        Velocity = Velocity with { Y = (!IsOnFloor() ? Velocity.Y : 0) - Gravity * (float)delta };

        // Handle states
        var movingForward = IsOnFloor() && InputDir.Y <= 0 && !InputDir.IsZeroApprox();
        Jumping = IsOnFloor() && Input.IsActionJustPressed("jump");
        Crouching = IsOnFloor() && !Jumping && Input.IsActionPressed("crouch") || Sliding || CrouchCheckRay.IsColliding();
        Running = movingForward && !Jumping && Input.IsActionPressed("run");
        Sliding = Sliding && IsOnFloor() && !Jumping; // Cancel slide if player jumps

        // GD.Print("Jumping: ", Jumping, ", Crouching: ", Crouching, ", Running: ", Running, ", Sliding: ", Sliding);

        //Handle jump
        if (Jumping)
            Velocity = Velocity with { Y = JumpVelocity };

        // Set maxSpeed
        var curMaxSpeed = WalkSpeed;
        if (Running)
            curMaxSpeed = RunSpeed;
        if (Crouching)
            curMaxSpeed = CrouchWalkSpeed;

        // Handle sliding
        var startSlide = IsOnFloor() && !Jumping && Crouching && movingForward && (Velocity with { Y = 0 }).Length() > WalkSpeed;
        if (startSlide && !Sliding)
        {
            Sliding = true;
            Velocity += direction * SlideSpeedBoost;
        }

        //Handle crouch
        var targetHeadHeight = Crouching ? crouchingHeadHeight : defaultHeadHeight;
        Head.Position = Head.Position with { Y = Mathf.Lerp(Head.Position.Y, targetHeadHeight, CrouchChangeSpeed * (float)delta) };
        StandingCollisionShape.Disabled = Crouching;
        CrouchingCollisionShape.Disabled = !Crouching;

        var curDeceleration = Sliding ? SlidingDeceleration : Deceleration;

        // Accelerate toward inputDir
        if (IsOnFloor() && !direction.IsZeroApprox() && !Sliding)
            Velocity = Velocity.Lerp((direction * curMaxSpeed) with { Y = Velocity.Y }, Acceleration * (float)delta);
        // Decelerate
        else if (IsOnFloor())
            Velocity = Velocity.Lerp(new Vector3(0, Velocity.Y, 0), curDeceleration * (float)delta);
        // Air control
        else if (!direction.IsZeroApprox())
            Velocity = Velocity.Lerp((direction * Velocity.Length()) with { Y = Velocity.Y }, AirControl * (float)delta);

        // Decelerate if faster than max speed 
        if (IsOnFloor() && Velocity.Length() > curMaxSpeed)
            Velocity = Velocity.Lerp(Velocity.LimitLength(curMaxSpeed) with { Y = Velocity.Y }, curDeceleration * (float)delta).LimitLength(Velocity.Length());

        // End slide when speed is slow enough
        if ((Velocity with { Y = 0 }).Length() < WalkSpeed)
            Sliding = false;

        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion &&
            Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            var mouseEvent = @event as InputEventMouseMotion;
            MouseInput = mouseEvent.Relative;
            RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * MouseSensitivity));
            var newXRotation = Mathf.Clamp(Head.Rotation.X + Mathf.DegToRad(-mouseEvent.Relative.Y * MouseSensitivity), Mathf.DegToRad(-CameraMaxAngle), Mathf.DegToRad(CameraMaxAngle));
            Head.Rotation = Head.Rotation with { X = newXRotation };
        }
    }
}
