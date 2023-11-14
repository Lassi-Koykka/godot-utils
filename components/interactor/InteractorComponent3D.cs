using Godot;

public partial class InteractorComponent3D : RayCast3D
{
	private InteractableComponent3D target;
	private bool prevCanInteract;

	[Signal]
	public delegate void InteractionUpdateEventHandler(InteractionStatus status, float progress = 0);

	public override void _Process(double delta)
	{
		target = GetInteractable();
		var canInteract = target != null && target.Enabled;
		if(canInteract && !prevCanInteract) {
			EmitSignal(SignalName.InteractionUpdate, (int)InteractionStatus.Available, 0);
		} else if(!canInteract && prevCanInteract) {
			EmitSignal(SignalName.InteractionUpdate, (int)InteractionStatus.Unavailable, 0);
		}
		prevCanInteract = canInteract;
	}
	
	private InteractableComponent3D GetInteractable() 
	{
		if(!this.IsColliding())
			return null;

		var coll = this.GetCollider();
		if(!(coll is InteractableComponent3D))
			return null;
		
		return coll as InteractableComponent3D;
	}

	public void Interact()
	{
		if(target == null || target.Enabled == false)
			return;

		var (status, progress) = target.HandleInteract();
		EmitSignal(SignalName.InteractionUpdate, (int)status, progress);
	}
}
