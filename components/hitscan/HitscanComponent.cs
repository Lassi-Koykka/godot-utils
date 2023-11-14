using Godot;

public partial class HitscanComponent : RayCast3D {

  public HurtboxComponent3D GetHurtbox() {
    if (!this.IsColliding()) {
      return null;
    }

    var coll = this.GetCollider();
    if(!(coll is HurtboxComponent3D)) {
      return null;
    }

    return coll as HurtboxComponent3D;
  }
}
