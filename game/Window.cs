using System.Drawing;

namespace Game;

using System.Timers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Timer = System.Timers.Timer;

public class Window : GameWindow {
    private readonly float[] _vertices = {
        // 3x Positions   3x Normals   2x Texture coords
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,

        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f,

        0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,

        -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f
    };

    private readonly Vector3[] _objectsPositions = {
        new(4.0f, 0.0f, 0.0f),
        new(0.0f, 0.0f, 4.0f),
        new(-4.0f, 0.0f, 0.0f),
        new(0.0f, 0.0f, -4.0f),
        new(3.0f, 0.0f, 3.0f),
        new(3.0f, 0.0f, -3.0f),
        new(-3.0f, 0.0f, -3.0f),
        new(-3.0f, 0.0f, 3.0f),
    };

    // We need the point lights' positions to draw the lamps and to get light the materials properly
    private readonly Vector3[] _pointLightPositions = {
        new(0.0f, 0.0f, 0.0f),
    };

    private Shader _shader;
    private Shader _lampShader;

    private Texture _texture;

    private Camera _camera;
    private bool _firstMove = true;
    private Vector2 _lastPos;

    private int _frames;
    private readonly Timer _timer = new() { Interval = 1000, AutoReset = true, Enabled = true };

    private readonly List<Mesh> _figures = new();
    private readonly List<Mesh> _lights = new();

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings) {
        _timer.Elapsed += OnTimerElapsed;
    }

    protected override void OnLoad() {
        base.OnLoad();

        // Set the clear color.
        GL.ClearColor(Color.DimGray);

        // Enable face culling.
        GL.CullFace(CullFaceMode.Back);
        GL.Enable(EnableCap.CullFace);

        // Enable depth testing.
        GL.DepthFunc(DepthFunction.Lequal);
        GL.Enable(EnableCap.DepthTest);

        // Enable polygon and line smooth
        GL.Enable(EnableCap.PolygonSmooth);
        GL.Enable(EnableCap.LineSmooth);

        _shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/lighting.frag");
        _lampShader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");

        _texture = Texture.LoadFromFile("../../../Textures/duck.jpg");

        _camera = new Camera(new Vector3(0.0f, 5.0f, 13.0f), Size.X / (float)Size.Y);

        // Create figures meshes
        foreach (var _ in _objectsPositions) {
            // _figures.Add(new Mesh(_shader, _vertices, _texture));
            _figures.Add(new Mesh(_shader, "../../../Objects/sphere.obj", _texture));
        }

        // var tex1 = Texture.LoadFromFile("../../../Textures/duck.jpg");
        // var tex2 = Texture.LoadFromFile("../../../Textures/box.png");
        // _figures.Add(new Mesh(_shader, "../../../Objects/sphere.obj", tex1));
        // _figures.Add(new Mesh(_shader, "../../../Objects/cube.obj", tex2));

        // Create lights meshes
        foreach (var _ in _pointLightPositions) {
            _lights.Add(new Mesh(_lampShader, "../../../Objects/sphere.obj"));
        }

        // Swap front and back buffers.
        SwapBuffers();
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
        base.OnRenderFrame(e);

        // Clear the color and depth buffer.
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Preset shaders
        _shader.PresetShaders(_camera, _pointLightPositions);
        _lampShader.PresetShaders(_camera, _pointLightPositions);

        // Draw cubes
        for (var i = 0; i < _objectsPositions.Length; i++) {
            var model = Matrix4.CreateScale(0.4f);
            model *= Matrix4.CreateTranslation(_objectsPositions[i] * 1.5f);
            _figures[i].Draw(_camera, model);
        }

        // Draw lights
        for (var i = 0; i < _pointLightPositions.Length; i++) {
            var model = Matrix4.CreateScale(0.3f);
            model *= Matrix4.CreateTranslation(_pointLightPositions[i]);
            _lights[i].Draw(_camera, model);
        }

        // Count frames
        _frames++;

        // Swap front and back buffers.
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e) {
        base.OnUpdateFrame(e);

        if (!IsFocused) return;

        HandleKeyboardInput();
        HandleMouseInput();

        if (CursorState != CursorState.Grabbed) return;

        HandleCameraMovement((float)e.Time);
        HandleCameraTurn();
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e) {
        base.OnMouseWheel(e);

        _camera.Fov -= e.OffsetY;
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X * 2, Size.Y * 2);

        _camera.AspectRatio = Size.X / (float)Size.Y;
    }

    private void HandleKeyboardInput() {
        if (KeyboardState.IsKeyPressed(Keys.Escape)) {
            if (CursorState == CursorState.Normal) {
                Close();
            }

            if (CursorState == CursorState.Grabbed) {
                CursorState = CursorState.Normal;
                _firstMove = true;
            }
        }

        if (KeyboardState.IsKeyPressed(Keys.V)) {
            ChangeVSync(Context.SwapInterval == 1 ? 0 : 1);
        }
    }

    private void HandleCameraMovement(float time) {
        if (KeyboardState.IsKeyDown(Keys.W)) _camera.Move(Camera.Direction.Forward, time);
        if (KeyboardState.IsKeyDown(Keys.S)) _camera.Move(Camera.Direction.Backward, time);
        if (KeyboardState.IsKeyDown(Keys.A)) _camera.Move(Camera.Direction.Left, time);
        if (KeyboardState.IsKeyDown(Keys.D)) _camera.Move(Camera.Direction.Right, time);
        if (KeyboardState.IsKeyDown(Keys.Space)) _camera.Move(Camera.Direction.Up, time);
        if (KeyboardState.IsKeyDown(Keys.LeftShift)) _camera.Move(Camera.Direction.Down, time);
    }

    private void HandleCameraTurn() {
        if (_firstMove) {
            _lastPos = new Vector2(MouseState.X, MouseState.Y);
            _firstMove = false;
        }
        else {
            var deltaX = MouseState.X - _lastPos.X;
            var deltaY = MouseState.Y - _lastPos.Y;
            _lastPos = new Vector2(MouseState.X, MouseState.Y);
            _camera.Turn(deltaX, deltaY);
        }
    }

    private void HandleMouseInput() {
        if (MouseState.IsButtonDown(MouseButton.Left) && CursorState != CursorState.Grabbed) {
            CursorState = CursorState.Grabbed;
        }
    }

    private void ChangeVSync(int vsync) {
        Context.SwapInterval = vsync;
        Console.WriteLine("VSync: " + (vsync == 1 ? "On" : "Off"));
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
        Console.WriteLine($"FPS: {_frames}");
        _frames = 0;
    }
}