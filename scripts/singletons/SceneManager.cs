using Godot;

public partial class SceneManager : Node {

  public Node CurrentScene { get; set; }

  public override void _Ready() {
	Viewport root = GetTree().Root;
	CurrentScene = root.GetChild(root.GetChildCount() - 1);
	GetTree().AutoAcceptQuit = false;
  }

	public override void _Notification(int what)
	{
			if (what == Window.NotificationWMCloseRequest) {
				GD.Print("Goodbye");
				GetTree().Quit();
			}
	}

  public void GotoScene(string path) {
	// This function will usually be called from a signal callback,
	// or some other function from the current scene.
	// Deleting the current scene at this point is
	// a bad idea, because it may still be executing code.
	// This will result in a crash or unexpected behavior.

	// The solution is to defer the load to a later time, when
	// we can be sure that no code from the current scene is running:

	GD.Print("Switching to scene: " + path);
	CallDeferred(nameof(DeferredGotoScene), path);
  }

  private void DeferredGotoScene(string path) {
	// It is now safe to remove the current scene
	CurrentScene.Free();

	// Load a new scene.
	var nextScene = (PackedScene)GD.Load(path);

	// Instance the new scene.
	CurrentScene = nextScene.Instantiate();

	// Add it to the active scene, as child of root.
	GetTree().Root.AddChild(CurrentScene);

	// Optionally, to make it compatible with the SceneTree.change_scene() API.
	GetTree().CurrentScene = CurrentScene;
  }
}

