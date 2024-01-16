using Godot;

[GlobalClass]
public partial class HitscanComponent3D : RayCast3D
{

    public HurtboxComponent3D GetHurtbox()
    {
        if (!this.IsColliding())
        {
            return null;
        }

        var coll = this.GetCollider();
        if (!(coll is HurtboxComponent3D))
        {
            return null;
        }

        return coll as HurtboxComponent3D;
    }
}
