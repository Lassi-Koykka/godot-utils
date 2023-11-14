using Godot;

public partial class EnemyDead : State
{
	Enemy3D enemy;

	public override void Enter()
	{
		if(fsm.Target == null || !(fsm.Target is Enemy3D)) {
			fsm.Back();
			return;
		}
		enemy = fsm.Target as Enemy3D;

		enemy.healthComponent.Enabled = false;

		enemy.animationPlayer.AnimationFinished += (Godot.StringName animationName) => {
			if(animationName == "death") {
				var timer = GetTree().CreateTimer(1);
				timer.Timeout += () => enemy.QueueFree();
			}
		};

		enemy.animationPlayer.Play("death");
	}
}
