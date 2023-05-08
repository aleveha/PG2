namespace Game;

using OpenTK.Mathematics;

public class Camera {
    public enum Direction {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }

    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;

    private float _pitch;
    private float _yaw = -MathHelper.PiOver2;
    private float _fov = MathHelper.PiOver2;

    private const float Speed = 3.0f;
    private const float Sensitivity = 0.2f;

    public Camera(Vector3 position, float aspectRatio) {
        Position = position;
        AspectRatio = aspectRatio;
    }

    public Vector3 Position { get; private set; }

    public float AspectRatio { private get; set; }
    public Vector3 Front => _front;
    private Vector3 Up => _up;
    private Vector3 Right => _right;

    private float Pitch {
        get => MathHelper.RadiansToDegrees(_pitch);
        set {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    private float Yaw {
        get => MathHelper.RadiansToDegrees(_yaw);
        set {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public float Fov {
        get => MathHelper.RadiansToDegrees(_fov);
        set {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Matrix4 GetViewMatrix() {
        return Matrix4.LookAt(Position, Position + _front, _up);
    }

    public Matrix4 GetProjectionMatrix() {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    public void Move(Direction direction, float coefficient) {
        switch (direction) {
            case Direction.Forward:
                Position += Front * Speed * coefficient;
                break;
            case Direction.Backward:
                Position -= Front * Speed * coefficient;
                break;
            case Direction.Left:
                Position -= Right * Speed * coefficient;
                break;
            case Direction.Right:
                Position += Right * Speed * coefficient;
                break;
            case Direction.Up:
                Position += Up * Speed * coefficient;
                break;
            case Direction.Down:
                Position -= Up * Speed * coefficient;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public void Turn(float x, float y) {
        Yaw += x * Sensitivity;
        Pitch -= y * Sensitivity;
    }

    private void UpdateVectors() {
        _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _front.Y = MathF.Sin(_pitch);
        _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        _front = Vector3.Normalize(_front);

        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }
}