using Godot;

public partial class PlayerFpsController : CharacterBody3D
{
    [Export] Camera3D Cam;
    [Export] Node3D Head;
    [Export] Node3D Arms;
    [Export] CollisionShape3D collShape;

    // Mouse look
    [ExportGroup("Mouse Settings")]
    [Export(PropertyHint.Range, "0,10,1,")]
    public float MouseSensitivity = ProjectSettings.GetSetting("player/look_sensitivity", 0.0f).As<float>();

    [Export(PropertyHint.Range, "0,90,1,")]
    public float CameraMaxAngle { get; private set; } = 75;
    Vector2 InputDir, MouseInput = new Vector2();

    // Physics
    [ExportGroup("Physics Settings")]
    [Export] public float Acceleration { get; private set; } = 0.5f;
    [Export] public float Friction { get; private set; } = 0.10f;
    [Export] public float AirControl { get; private set; } = 0.25f;
    [Export] public float MaxVelocity { get; private set; } = 8.0f;
    [Export] public float MaxRunVelocity { get; private set; } = 16.0f;
    [Export] public float MaxCrouchVelocity { get; private set; } = 4.0f;
    [Export] public float JumpVelocity { get; private set; } = 6f;
    [Export] public float Gravity { get; private set; } = 12.0f;

    // Rig
    [ExportGroup("Juice")]
    [Export] float BobAmount = 0.01f;
    [Export] float BobFreq = 0.01f;
    [Export] float HeadRotationAmount = 0.04f;
    [Export] float ArmsSwayAmount = 0.01f;
    [Export] float ArmsRotationAmount = 0.04f;
    [Export] bool VelocityAffectsFov = false;
    [Export] public float MaxVelocityFovIncrease { get; private set; } = 30.0f;

    float DefaultCamFov = 90.0f;
    Vector3 DefaultArmsPos, DefaultHeadPos = Vector3.Zero;

    // Other
    [ExportGroup("Other settings")]
    [Export] public bool Crouched { get; private set; }

    float defaultHeight = 0.0f;
    Tween crouchTween = null;
    CylinderShape3D shape;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        shape = collShape.Shape as CylinderShape3D;
        defaultHeight = shape.Height;
        DefaultHeadPos = Head.Position;
        DefaultArmsPos = Arms.Position;
        DefaultCamFov = Cam.Fov;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
        InputDir = Godot.Input.GetVector("move_left", "move_right", "move_forward",
                                         "move_backward");
        // Handle crouch
        if (Input.IsActionJustPressed("crouch"))
        {
            ToggleCrouch();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var vel = GetMovementVelocity(delta);
        Cam.Fov = Mathf.Lerp(DefaultCamFov, DefaultCamFov + MaxVelocityFovIncrease, Mathf.Clamp((Velocity.Length() - 10) / MaxRunVelocity, 0, 1));
        Velocity = vel;
        MoveAndSlide();
        if (VelocityAffectsFov)
            JuiceUtils.ApplyVelocityFovIncrease(Cam, DefaultCamFov, DefaultCamFov + MaxVelocityFovIncrease, Velocity.Length(), MaxVelocity);
        JuiceUtils.ApplyTilt(Head, InputDir.X, HeadRotationAmount, delta);
        JuiceUtils.ApplyTilt(Arms, InputDir.X, ArmsRotationAmount, delta);
        JuiceUtils.ApplySway(Arms, MouseInput, ArmsSwayAmount, delta);
        var playerMoving = Velocity.Length() > 0 && IsOnFloor();
        JuiceUtils.ApplyBob(Arms, Velocity.Length(), playerMoving, DefaultArmsPos, new Vector2(BobAmount, BobAmount) * (Velocity.Length() / MaxVelocity), BobFreq, delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion &&
            Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            var mouseEvent = @event as InputEventMouseMotion;
            MouseInput = mouseEvent.Relative;
            RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * MouseSensitivity));
            var newCamRotation = Head.Rotation;
            var newXRotation = newCamRotation.X + Mathf.DegToRad(-mouseEvent.Relative.Y * MouseSensitivity);
            newCamRotation.X = Mathf.Clamp(newXRotation, Mathf.DegToRad(-CameraMaxAngle), Mathf.DegToRad(CameraMaxAngle));
            Head.Rotation = newCamRotation;
        }
    }

    public void ToggleCrouch(bool force = false)
    {
        if (!force && crouchTween != null && crouchTween.IsRunning())
            return;
        Crouched = !Crouched;
        crouchTween = CreateTween();
        crouchTween.SetParallel(true);
        var newHeight = Crouched ? defaultHeight / 3 : defaultHeight;
        crouchTween.TweenProperty(shape, "height", newHeight, 0.25f);
        if (Input.IsActionPressed("run"))
            Velocity *= new Vector3(5, 1, 5);
    }

    public Vector3 GetMovementVelocity(double delta)
    {
        Vector3 velocity = Velocity;
        var velY = velocity.Y;
        // Add the gravity.

        // Get the input direction and handle the movement/deceleration.
        Vector3 direction =
            (Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y)).Normalized();

        var weight = (direction.IsZeroApprox() ? Acceleration : Friction);
        var maxVelocity = direction * MaxVelocity;

        var canRun = !Crouched && InputDir.Y < 0;
        if (Crouched && IsOnFloor())
            maxVelocity = direction * MaxCrouchVelocity;
        else if (canRun && Input.IsActionPressed("run"))
            maxVelocity = direction * MaxRunVelocity;

        if (!IsOnFloor())
        {
            weight *= AirControl;
            // Apply gravity
            velY -= Gravity * (float)delta;
        }

        var newVel = velocity.Lerp(maxVelocity, weight);
        velocity = new Vector3(newVel.X, velY, newVel.Z);


        // Handle Jump.
        if (Godot.Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
            if (Crouched) ToggleCrouch(true);
        }

        return velocity;
    }
}
