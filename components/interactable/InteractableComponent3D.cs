using Godot;

public enum InteractionStatus
{
    Unavailable,
    Available,
    Failed,
    InProgress,
    Success,
}

public partial class InteractableComponent3D : Area3D
{
    [Signal]
    public delegate void InteractedEventHandler();
    [Export]
    public bool Enabled { get; set; } = true;
    [Export]
    public float InteractionDuration { get; private set; } = 0;
    public float InteractionProgress { get; private set; } = 0;
    public bool InteractionActive { get; private set; } = false;
    public string promptText { get; private set; } = "";

    public override void _PhysicsProcess(double delta)
    {
        var newProgress = InteractionProgress + (float)(InteractionActive ? delta : -delta);
        InteractionProgress = Mathf.Clamp(newProgress, 0, InteractionDuration);

        InteractionActive = false;
    }

    public virtual (InteractionStatus, float) HandleInteract()
    {
        InteractionActive = true;
        var progress = (InteractionProgress / InteractionDuration);
        if (InteractionDuration > 0 && InteractionProgress < InteractionDuration)
        {
            return (InteractionStatus.InProgress, progress);
        }

        EmitSignal(SignalName.Interacted);
        Enabled = false;
        InteractionActive = false;
        return (InteractionStatus.Success, 1);
    }
}
