using Godot;

public partial class PlayerTopDown2D : CharacterBody2D
{

    [Export]
    private int MOVE_SPEED = 300;

    private Vector2 lookVec = new Vector2();

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
     {
         var x = 0.0f;
         var y = 0.0f;

         if(Input.IsActionPressed("move_up"))
             y -= 1;
         if(Input.IsActionPressed("move_down"))
             y += 1;
         if(Input.IsActionPressed("move_left"))
             x -= 1;
         if(Input.IsActionPressed("move_right"))
             x += 1;

         var moveVec = new Vector2(x, y).Normalized();
         Velocity = moveVec * MOVE_SPEED;
         MoveAndSlide();
 
         lookVec = GetGlobalMousePosition() - GlobalPosition;
     }

}
