using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    public class Game : IGame, IDisposable
    {
        private readonly string _title;
        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _modelMatrix = Matrix4.Identity;

        private readonly IAppConfig _config;
        private readonly ILogger _logger;
        private readonly IRenderer _renderer;
        private readonly ISkybox _skybox;
        private readonly ICamera _camera;
        private readonly IWorld _world;

        private readonly OpenGlContext _context;

        public Game(IAppConfig config, ILogger logger, ICamera camera, IWorld world, OpenGlContext context, IRenderer renderer, ISkybox skybox)
        {
            _camera = camera;
            _logger = logger;
            _config = config;
            _title = config.WindowTitle;

            _context = context;
            ConfigureContext();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.ClearColor(Color4.Green);

            _renderer = renderer;
            _skybox = skybox;
            _world = world;

            _renderer.AddToScene(_world.Renderables);
        }

        private void ConfigureContext()
        {
            _context.RenderFrame += (sender, e) => this.OnRenderFrame(e);
            _context.UpdateFrame += (sender, e) => this.OnUpdateFrame(e);
            _context.Resize += (sender, e) => this.OnResize(e);
            _context.KeyDown += (sender, e) => this.OnKeyDown(e);
            _context.Width = _config.ResolutionWidth;
            _context.Height = _config.ResolutionHeight;
            _context.Title = _config.WindowTitle;
        }

        public void RunGame()
        {
            _context.Run(_config.FrameRate);
        }

        private void OnUpdateFrame(FrameEventArgs e)
        {
        }

        private void OnRenderFrame(FrameEventArgs e)
        {
            _context.Title = $"{_title} - FPS: {Math.Round(1f / e.Time, 1)}";

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var viewMatrix = _camera.Use();
            _skybox.Render(_projectionMatrix, viewMatrix);
            GL.DepthFunc(DepthFunction.Lequal);
            _renderer.RenderScene(_projectionMatrix, viewMatrix, _modelMatrix);

            _context.SwapBuffers();
        }

        private void OnResize(EventArgs e)
        {
            _logger.Information($"Window resized: {_context.Width}x{_context.Height}");
            GL.Viewport(_context.ClientRectangle);
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), (float)_context.Width / _context.Height, 0.1f, 5000.0f);
        }

        private void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _context.Close();
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

            if (e.Alt && e.Key == Key.Enter)
            {
                _context.ToggleFullscreen();
            }
        }

        public void Dispose()
        {
            _logger.Information("Disposing objects");
            _renderer?.Dispose();
            _skybox?.Dispose();
            _world.Dispose();
            _context.Dispose();
        }
    }
}
