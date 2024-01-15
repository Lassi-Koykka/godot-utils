using Godot;

public partial class PlayerFps : CharacterBody3D
{
    [Export] public Node3D Cam { get; set; }
    [Export] Node3D ItemHolder;
    [Export] CollisionShape3D collShape;

    // Mouse look
    [ExportGroup("Mouse Settings")]
    [Export(PropertyHint.Range, "0,10,1,")]
    public float MouseSensitivity = 1;

    [Export(PropertyHint.Range, "0,90,1,")]
    public float CameraMaxAngle { get; private set; } = 75;
    const float MouseScale = 0.001f;
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
    [Export] float CamHolderRotationAmount = 0.04f;
    [Export] float ItemSwayAmount = 0.01f;
    [Export] float ItemHolderRotationAmount = 0.04f;
    Vector3 DefaultItemHolderPos, DefaultCamHolderPos = Vector3.Zero;

    // Other
    [ExportGroup("Other settings")]
    [Export] public bool Crouched { get; private set; }

    float defaultHeight = 0.0f;
    Tween crouchTween = null;
    CapsuleShape3D capsule;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        capsule = collShape.Shape as CapsuleShape3D;
        defaultHeight = capsule.Height;
        DefaultCamHolderPos = Cam.Position;
        DefaultItemHolderPos = ItemHolder.Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        InputDir = Godot.Input.GetVector("move_left", "move_right", "move_up",
                                         "move_down");
        // Handle crouch
        if (Input.IsActionJustPressed("crouch"))
        {
            if (crouchTween != null && crouchTween.IsRunning())
                return;
            Crouched = !Crouched;
            crouchTween = CreateTween();
            crouchTween.SetParallel(true);
            crouchTween.TweenProperty(capsule, "height",
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
        JuiceUtils.ApplyTilt(Cam, InputDir.X, CamHolderRotationAmount, delta);
        JuiceUtils.ApplyTilt(ItemHolder, InputDir.X, ItemHolderRotationAmount, delta);
        JuiceUtils.ApplySway(ItemHolder, MouseInput, ItemSwayAmount, delta);
        var playerMoving = Velocity.Length() > 0 && IsOnFloor();
        JuiceUtils.ApplyBob(ItemHolder, Velocity.Length(), playerMoving, DefaultItemHolderPos, new Vector2(BobAmount, BobAmount), BobFreq, delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion &&
            Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            var mouseEvent = @event as InputEventMouseMotion;
            MouseInput = mouseEvent.Relative;
            RotateY(-mouseEvent.Relative.X * MouseScale * MouseSensitivity);
            var camXRotationAmount = -mouseEvent.Relative.Y * MouseScale;
            var newCamRotation = Cam.Rotation;
            var maxAngle = Mathf.DegToRad(CameraMaxAngle);
            var newXRotation = newCamRotation.X + camXRotationAmount;
            newCamRotation.X = Mathf.Clamp(newXRotation, -maxAngle, maxAngle);
            Cam.Rotation = newCamRotation;
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
