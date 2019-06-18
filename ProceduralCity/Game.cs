using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    public class Game : GameWindow
    {
        private readonly string _title;
        private readonly Renderer.Renderer _renderer;
        private readonly Skybox _skybox;
        //private readonly Camera _camera = new Camera(new Vector3(96, 70, -80), 50, 20);
        private readonly Camera _camera;

        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _modelMatrix = Matrix4.Identity;

        private readonly Building _building;

        public Game(int width, int height, GraphicsMode mode, string title) : base(width, height, mode, title)
        {
            _title = title;

            var glVendor = GL.GetString(StringName.Vendor);
            var glRenderer = GL.GetString(StringName.Renderer);
            Log.Information($"Vendor: {glVendor} | Renderer: {glRenderer}");

            _renderer = new Renderer.Renderer();
            _skybox = new Skybox();
            _camera = new Camera(new Vector3(0, 0, 0), 50, 20);

            GL.ClearColor(Color4.Green);
            var buildingShader = new Shader("vs.vert", "fs.frag");
            var buildingTexture = new Texture("building/1.jpg");

            _building = new Building(Vector3.Zero, new Vector2(1000, 1000), buildingTexture, buildingShader);
            _renderer.AddToScene(_building);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Title = $"{_title} - FPS: {Math.Round(1f / e.Time, 1)}";

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var viewMatrix = _camera.Use();
            var mvp = _projectionMatrix * viewMatrix * _modelMatrix;
            _skybox.Render(_projectionMatrix, viewMatrix);
            _renderer.RenderScene(mvp);

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Log.Information($"Window resized: {Width}x{Height}");
            GL.Viewport(this.ClientRectangle);
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), (float)Width / Height, 0.1f, 5000.0f);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            if (e.Key == Key.A)
            {
                _camera.StrafeLeft();
            }
            else if (e.Key == Key.D)
            {
                _camera.StrafeRight();
            }

            if (e.Key == Key.W)
            {
                _camera.MoveForward();
            }
            else if (e.Key == Key.S)
            {
                _camera.MoveBackward();
            }

            if (e.Key == Key.Up)
            {
                _camera.SetVertical(-1.0f);
            }
            else if (e.Key == Key.Down)
            {
                _camera.SetVertical(1.0f);
            }

            if (e.Key == Key.Left)
            {
                _camera.SetHorizontal(-1.0f);
            }
            else if (e.Key == Key.Right)
            {
                _camera.SetHorizontal(1.0f);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Log.Information("Disposing objects");
            _renderer.Dispose();
            _skybox.Dispose();
            _building.Dispose();
        }
    }
}
