using Godot;

public abstract partial class State : Node
{
    protected StateMachine fsm;

    public virtual void Init(StateMachine stateMachine)
    {
        fsm = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit(string nextState)
    {
        fsm.ChangeState(nextState);
    }

    public virtual void Process(double delta) { }
    public virtual void PhysicsProcess(double delta) { }
    public virtual void Input(InputEvent @event) { }
    public virtual void UnhandledInput(InputEvent @event) { }
    public virtual void UnhandledKeyInput(InputEvent @event) { }
    public virtual void Notification(int what) { }
}
