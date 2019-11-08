using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using ProceduralCity.Renderer.Utils;
using Serilog;

namespace ProceduralCity
{
    class Game : IGame, IDisposable
    {
        private readonly string _title;
        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _modelMatrix = Matrix4.Identity;

        private Matrix4 _textRendererMatrix = Matrix4.Identity;
        private Matrix4 _ndcRendererMatrix;
        private readonly Textbox _text = new Textbox("Consolas");

        private readonly IAppConfig _config;
        private readonly ILogger _logger;
        private readonly IRenderer _renderer;
        private readonly IRenderer _textRenderer;
        private readonly IRenderer _ndcRenderer;
        private readonly IRenderer _skyboxRenderer;
        private readonly ISkybox _skybox;
        private readonly ICamera _camera;
        private readonly IWorld _world;

        private readonly OpenGlContext _context;

        private readonly BackBufferRenderer _billboardRenderer;
        private readonly BackBufferRenderer _worldRenderer;

        private double _elapsedFrameTime = 0;

        public Game(
            IAppConfig config,
            ILogger logger,
            ICamera camera,
            IWorld world,
            OpenGlContext context,
            IRenderer renderer,
            IRenderer textRenderer,
            IRenderer ndcRenderer,
            IRenderer skyboxRenderer,
            ISkybox skybox)
        {
            _camera = camera;
            _logger = logger;
            _config = config;
            _title = config.WindowTitle;

            _context = context;
            ConfigureContext();

            _skybox = skybox;
            _world = world;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.ClearColor(Color4.Green);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _renderer = renderer;
            _textRenderer = textRenderer;
            _textRenderer.BeforeRender = () =>
            {
                GL.Enable(EnableCap.Blend);
            };
            _textRenderer.AfterRender = () =>
            {
                GL.Disable(EnableCap.Blend);
            };
            _ndcRenderer = ndcRenderer;
            
            _skyboxRenderer = skyboxRenderer;
            _skyboxRenderer.BeforeRender = () =>
            {
                GL.DepthFunc(DepthFunction.Lequal);
                GL.CullFace(CullFaceMode.Front);
            };
            _skyboxRenderer.AfterRender = () =>
            {
                GL.CullFace(CullFaceMode.Back);
            };
            _skyboxRenderer.AddToScene(skybox);

            _renderer.AddToScene(_world.Renderables);

            _worldRenderer = new BackBufferRenderer(
                _logger,
                _config.ResolutionWidth,
                _config.ResolutionHeight,
                useDepthBuffer: true);

            _text.WithText("Árvíztűrő tükörfúrógép", new Vector2(0, 200), 1.0f);
            _text.Saturation = 1;
            _textRenderer.AddToScene(_text.Text);

            _ndcRenderer.AddToScene(new FullScreenQuad(_worldRenderer.Texture));

            //var w = 320;
            //var h = 240;
            //_billboardRenderer = new BackBufferRenderer(_logger, w, h, useDepthBuffer: false);
            //var r = new Renderer.Renderer();
            //var text = new Textbox("Consolas").WithText("Yaaaay!!!", new Vector2(0, 0), 1);
            //r.AddToScene(text.Text);
            //var p = Matrix4.CreateOrthographicOffCenter(0, w, h, 0, -1, 1);
            //_billboardRenderer.RenderToTexture(r, p, Matrix4.Identity, Matrix4.Identity);

            //var b = new Billboard(_billboardRenderer.Texture);
            //_textRenderer.AddToScene(new[] { b }); //TODO: billboards should be rendered with a different renderer with blending enabled
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
            CountFps(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var viewMatrix = _camera.Use();

            _worldRenderer.Clear();
            _worldRenderer.RenderToTexture(_skyboxRenderer, _projectionMatrix, new Matrix4(new Matrix3(viewMatrix)), Matrix4.Identity);
            _worldRenderer.RenderToTexture(_renderer, _projectionMatrix, viewMatrix, _modelMatrix);
            _worldRenderer.RenderToTexture(_textRenderer, _textRendererMatrix, Matrix4.Identity, Matrix4.Identity);

            _ndcRenderer.RenderScene(_ndcRendererMatrix, Matrix4.Identity, Matrix4.Identity);
            _context.SwapBuffers();
        }

        private void CountFps(FrameEventArgs e)
        {
            _elapsedFrameTime += e.Time;
            if (_elapsedFrameTime >= 0.5)
            {
                _context.Title = $"{_title} - FPS: {Math.Round(1f / e.Time, 0)}";
                _elapsedFrameTime = 0;
            }
        }

        private void OnResize(EventArgs e)
        {
            _logger.Information($"Window resized: {_context.Width}x{_context.Height}");
            GL.Viewport(_context.ClientRectangle);
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), (float)_context.Width / _context.Height, 0.1f, 5000.0f);
            _textRendererMatrix = Matrix4.CreateOrthographicOffCenter(0, _context.Width, _context.Height, 0, -1, 1);
            _ndcRendererMatrix = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);
            _worldRenderer.Resize(_context.ClientRectangle.Width, _context.ClientRectangle.Height);
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

            if (e.Key == Key.F1)
            {
                _context.ToggleVSync();
            }

            if (e.Key == Key.KeypadPlus)
            {
                _text.Hue += 0.01f;
            }
            else if (e.Key == Key.KeypadMinus)
            {
                _text.Hue -= 0.01f;
            }
        }

        public void Dispose()
        {
            _logger.Information("Disposing objects");
            _renderer?.Dispose();
            _skybox?.Dispose();
            _world.Dispose();
            _context.Dispose();
            _text.Dispose();
            _worldRenderer.Dispose();
            _ndcRenderer.Dispose();
            _skyboxRenderer.Dispose();
        }
    }
}
