using Godot;

public partial class Enemy3DAlive : State
{
	Enemy3D enemy;
	public override void Enter()
	{
		if(fsm.Target == null || !(fsm.Target is Enemy3D)) {
			fsm.Back();
			return;
		}

		enemy = fsm.Target as Enemy3D;

		enemy.healthComponent.Died += Die;
	}
	
	public override void Exit(string nextState)
	{
		enemy.healthComponent.Died -= Die;
		fsm.ChangeState(nextState);
	}

	public override void PhysicsProcess(double delta)
	{
		Vector3 velocity = enemy.Velocity;

		// Add the gravity.
		if (!enemy.IsOnFloor())
			velocity.Y -= enemy.gravity * (float)delta;

		var direction = new Vector3();
		var player = enemy.GetParent().GetNodeOrNull<PlayerFirstPerson3D>("Player");
		if(player != null) {
			direction = enemy.Position.DirectionTo(player.GlobalPosition);
			enemy.LookAt(player.GlobalPosition, Vector3.Up, true);
		}

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * enemy.Speed;
			velocity.Z = direction.Z * enemy.Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(enemy.Velocity.X, 0, enemy.Speed);
			velocity.Z = Mathf.MoveToward(enemy.Velocity.Z, 0, enemy.Speed);
		}

		enemy.Velocity = velocity;
		enemy.MoveAndSlide();
	}

	private void Die() => Exit("Dead");
}
