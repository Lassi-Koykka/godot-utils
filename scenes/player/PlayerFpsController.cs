using Godot;

public partial class PlayerFpsController : CharacterBody3D
{
    [Export] public Node3D Head { get; set; }
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
    [Export] public float Speed { get; private set; } = 3.0f;
    [Export] public float JumpVelocity { get; private set; } = 4.5f;
    [Export] public float gravity { get; private set; } = 12.0f;

    // Rig
    [ExportGroup("Juice")]
    [Export] float BobAmount = 0.01f;
    [Export] float BobFreq = 0.01f;
    [Export] float HeadRotationAmount = 0.04f;
    [Export] float ArmsSwayAmount = 0.01f;
    [Export] float ArmsRotationAmount = 0.04f;
    Vector3 DefaultItemHolderPos, DefaultHeadPos = Vector3.Zero;

    // Other
    [ExportGroup("Other settings")]
    [Export] public bool Crouched { get; private set; }

    float defaultHeight = 0.0f;
    Tween crouchTween = null;
    CylinderShape3D shape;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Input.MouseMode = Input.MouseModeEnum.Captured;
        shape = collShape.Shape as CylinderShape3D;
        defaultHeight = shape.Height;
        DefaultHeadPos = Head.Position;
        DefaultItemHolderPos = Arms.Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // if (Input.IsActionJustPressed("ui_cancel"))
        //     Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
        InputDir = Godot.Input.GetVector("move_left", "move_right", "move_forward",
                                         "move_backward");
        // Handle crouch
        if (Input.IsActionJustPressed("crouch"))
        {
            if (crouchTween != null && crouchTween.IsRunning())
                return;
            Crouched = !Crouched;
            crouchTween = CreateTween();
            crouchTween.SetParallel(true);
            crouchTween.TweenProperty(shape, "height",
                                      Crouched ? defaultHeight / 3 : defaultHeight,
                                      0.25f);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var vel = GetMovementVelocity(delta);

        var canRun = !Crouched && IsOnFloor() && InputDir.Y < 0;
        if (Crouched && IsOnFloor())
            vel *= new Vector3(0.5f, 1, 0.5f);
        else if (canRun && Input.IsActionPressed("run"))
            vel *= new Vector3(2, 1, 2);
        Velocity = vel;
        MoveAndSlide();
        JuiceUtils.ApplyTilt(Head, InputDir.X, HeadRotationAmount, delta);
        JuiceUtils.ApplyTilt(Arms, InputDir.X, ArmsRotationAmount, delta);
        JuiceUtils.ApplySway(Arms, MouseInput, ArmsSwayAmount, delta);
        var playerMoving = Velocity.Length() > 0 && IsOnFloor();
        JuiceUtils.ApplyBob(Arms, Velocity.Length(), playerMoving, DefaultItemHolderPos, new Vector2(BobAmount, BobAmount), BobFreq, delta);
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

    public Vector3 GetMovementVelocity(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        if (Godot.Input.IsActionJustPressed("jump") && IsOnFloor())
            velocity.Y = JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        Vector3 direction =
            (Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y)).Normalized();

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
}
