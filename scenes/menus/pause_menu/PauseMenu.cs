using Godot;

public partial class PauseMenu : Control {

  const string SCENE_FOLDER = "res://scenes/";

  SceneManager sm;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    sm = GetNode<SceneManager>("/root/SceneManager");

    GetNode<Button>("%ResumeButton").Pressed += ResumeButtonPressedEventHandler;
    GetNode<Button>("%OptionsButton").Pressed += OptionsButtonPressedEventHandler;
    GetNode<Button>("%QuitToMenuButton").Pressed += QuitToMenuButtonPressedEventHandler;
    GetNode<Button>("%QuitButton").Pressed += QuitButtonPressedEventHandler;

    Hide();
  }

  public override void _UnhandledKeyInput(InputEvent @event) {
    var inputEvent = @event as InputEventAction;

    if (Input.IsActionJustPressed("ui_cancel")) {
      if (GetTree().Paused) {
        Unpause();
      } else {
        Pause();
      }
    }
  }

  public void Pause() {
    Input.MouseMode = Input.MouseModeEnum.Visible;
    GetTree().Paused = true;
    Show();
  }

  public void Unpause() {
    Input.MouseMode = Input.MouseModeEnum.Captured;
    GetTree().Paused = false;
    Hide();
  }

  private void ResumeButtonPressedEventHandler() { Unpause(); }

  private void OptionsButtonPressedEventHandler() {
    GD.PrintErr("NOT IMPLEMENTED");
  }

  private void QuitToMenuButtonPressedEventHandler() {
    sm.GotoScene(SCENE_FOLDER + "menus/main_menu/main_menu.tscn");
  }

  private void QuitButtonPressedEventHandler() { GetTree().Quit(); }
}
