using Godot;

public partial class MainMenu : Control {
  const string SCENE_FOLDER = "res://scenes/";

  SceneManager sm;

  public override void _Ready() {
	sm = GetNode<SceneManager>("/root/SceneManager");
	GetNode<Button>("%StartButton").Pressed += StartButtonPressedEventHandler;
	GetNode<Button>("%OptionsButton").Pressed += OptionsButtonPressedEventHandler;
	GetNode<Button>("%QuitButton").Pressed += QuitButtonPressedEventHandler;

	GetTree().Paused = false;
  }

  private void StartButtonPressedEventHandler() {
	  sm.GotoScene(SCENE_FOLDER + "levels/level_one.tscn");
  }

  private void OptionsButtonPressedEventHandler() {
	  GD.PrintErr("NOT IMPLEMENTED");
  }

  private void QuitButtonPressedEventHandler() { GetTree().Quit(); }
}
