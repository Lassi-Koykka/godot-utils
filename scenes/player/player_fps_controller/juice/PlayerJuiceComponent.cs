using Godot;

public partial class PlayerJuiceComponent : Node
{
    PlayerFpsController player;
    [ExportGroup("Juice")]
    [Export] public float BobAmount = 0.01f;
    [Export] public float BobFreq = 0.01f;
    [Export] public float HeadRotationAmount = 0.04f;
    [Export] public float SlideHeadRotationAmount = 0.15f;
    [Export] public float ArmsSwayAmount = 0.01f;
    [Export] public float ArmsRotationAmount = 0.04f;
    [Export] public float SlideArmsRotationAmount = 0.3f;
    [Export] public float MaxVelocityFovIncrease = 30.0f;
    [Export] public float FovChangeMargin = 10.0f;


    float defaultCamFov, maxFov = 90.0f;
    Vector3 defaultArmsPos, defaultHeadPos = Vector3.Zero;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetParent<PlayerFpsController>();
        defaultHeadPos = player.Head.Position;
        defaultArmsPos = player.Arms.Position;
        defaultCamFov = player.Cam.Fov;
        maxFov = defaultCamFov + MaxVelocityFovIncrease;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        JuiceUtils.ApplyVelocityFovIncrease(player.Cam, defaultCamFov, maxFov, player.Velocity.Length(), player.WalkSpeed * 2, delta, FovChangeMargin);
        JuiceUtils.ApplyTilt(player.Head, player.InputDir.X, HeadRotationAmount, delta);
        JuiceUtils.ApplyTilt(player.Arms, player.InputDir.X, ArmsRotationAmount, delta);
        if (player.Sliding)
        {
            JuiceUtils.ApplyTilt(player.Head, 1, SlideHeadRotationAmount, delta);
            JuiceUtils.ApplyTilt(player.Arms, -1, SlideArmsRotationAmount, delta);

        }
        JuiceUtils.ApplySway(player.Arms, player.MouseInput, ArmsSwayAmount, delta);
        var playerMoving = player.Velocity.Length() > 0 && player.IsOnFloor();
        var armBob = new Vector2(BobAmount, BobAmount) * (player.Velocity.Length() / player.WalkSpeed);
        JuiceUtils.ApplyBob(player.Arms, playerMoving, defaultArmsPos, armBob, BobFreq, delta);
    }
}
