using Godot;

public static class JuiceUtils
{
    public static void ApplyTilt(Node3D target, float inputX, float rotationAmount, double delta)
    {
        if (target == null)
            return;

        var tempRotation = target.Rotation;
        tempRotation.Z = (float)Mathf.LerpAngle(target.Rotation.Z, -inputX * rotationAmount, 10 * delta);
        target.Rotation = tempRotation;
    }

    public static void ApplySway(Node3D target, Vector2 input, float amount, double delta)
    {
        var tempInput = input.Lerp(Vector2.Zero, 10 * (float)delta);
        var tempRotation = target.Rotation;
        tempRotation.X = Mathf.Lerp(tempRotation.X, tempInput.Y * amount, 10 * (float)delta);
        tempRotation.Y = Mathf.Lerp(tempRotation.Y, tempInput.X * amount, 10 * (float)delta);
        target.Rotation = tempRotation;
    }

    public static void ApplyBob(Node3D target, float vel, bool isMoving, Vector3 defaultPos, Vector2 amplitude, float freq, double delta)
    {
        if (target == null)
            return;

        var tempPos = target.Position;
        var deltaY = Mathf.Sin(Time.GetTicksMsec() * freq) * amplitude.Y;
        var deltaX = Mathf.Sin(Time.GetTicksMsec() * freq) * amplitude.X;
        tempPos.Y = Mathf.Lerp(tempPos.Y, defaultPos.Y + (isMoving ? deltaY : 0), 10 * (float)delta);
        tempPos.X = Mathf.Lerp(tempPos.X, defaultPos.X + (isMoving ? deltaX : 0), 10 * (float)delta);
        target.Position = tempPos;
    }
}
