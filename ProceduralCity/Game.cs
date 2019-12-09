using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using ProceduralCity.Renderer.PostProcess;
using ProceduralCity.Renderer.Uniform;
using ProceduralCity.Renderer.Utils;
using Serilog;

namespace ProceduralCity
{
    //TODO: Add the ability to render post process effects in a lower resolution
    //TODO: Generators should not own any texture or shader references, these should be asked from a resource manager class
    //TODO: dispose all generators after the generation has been completed
    //TODO: generate building textures procedurally
    //TODO: more building types
    //TODO: add more variety to the existing building types
    //TODO: add traffic lights
    //TODO: textures on buildings seem to be upside down
    //TODO: dynamic text rendering
    //TODO: fog
    //TODO: Mipmaping modes for generated textures (created with new Texture(w,h))
    //TODO: add decal rendering (stencil buffer?)
    class Game : IGame, IDisposable
    {
        private readonly string _title;
        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _ndcRendererMatrix;

        private readonly IAppConfig _config;
        private readonly ILogger _logger;
        private readonly IRenderer _renderer;
        private readonly IRenderer _ndcRenderer;
        private readonly IRenderer _skyboxRenderer;
        private readonly ISkybox _skybox;
        private readonly ICamera _camera;
        private readonly IWorld _world;

        private readonly OpenGlContext _context;
        private readonly BackBufferRenderer _worldRenderer;
        private readonly Texture _backbufferTexture;
        private readonly Shader _fullscreenShader;

        private bool _isBloomEnabled = true;
        private readonly PostprocessPipeline _postprocessPipeline;
        private readonly Texture _postprocessTexture;
        private Texture _ndcTexture;

        private double _elapsedFrameTime = 0;

        public Game(
            IAppConfig config,
            ILogger logger,
            ICamera camera,
            IWorld world,
            OpenGlContext context,
            IRenderer renderer,
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

            _backbufferTexture = new Texture(_config.ResolutionWidth, config.ResolutionHeight);
            _worldRenderer = new BackBufferRenderer(
                _logger,
                _backbufferTexture,
                _config.ResolutionWidth,
                _config.ResolutionHeight,
                useDepthBuffer: true);

            _postprocessTexture = new Texture(_config.ResolutionWidth, _config.ResolutionHeight);
            _postprocessPipeline = new PostprocessPipeline(_logger, _config, _worldRenderer.Texture, _postprocessTexture);

            _fullscreenShader = new Shader("vs.vert", "fs.frag");
            _fullscreenShader.SetUniformValue("tex", new IntUniform
            {
                Value = 0
            });

            _ndcTexture = _postprocessTexture;
            var fullScreenQuad = new FullScreenQuad(new[] { _ndcTexture }, _fullscreenShader);
            _ndcRenderer.AddToScene(fullScreenQuad);
        }

        private void ConfigureContext()
        {
            _context.RenderFrame += (sender, e) => this.OnRenderFrame(e);
            _context.UpdateFrame += (sender, e) => this.OnUpdateFrame(e);
            _context.Resize += (sender, e) => this.OnResize();
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
            _worldRenderer.RenderToTexture(_skyboxRenderer, _projectionMatrix, new Matrix4(new Matrix3(viewMatrix)));
            _worldRenderer.RenderToTexture(_renderer, _projectionMatrix, viewMatrix);

            if (_isBloomEnabled)
                _postprocessPipeline.RunPipeline();

            GL.Viewport(0, 0, _context.ClientRectangle.Width, _context.ClientRectangle.Height);
            _ndcRenderer.RenderScene(_ndcRendererMatrix, Matrix4.Identity);
            _context.SwapBuffers();
        }

        private void CountFps(FrameEventArgs e)
        {
            _elapsedFrameTime += e.Time;
            if (_elapsedFrameTime >= 0.4)
            {
                _context.Title = $"{_title} - FPS: {Math.Round(1f / e.Time, 0)}";
                _elapsedFrameTime = 0;
            }
        }

        private void OnResize()
        {
            _logger.Information($"Window resized: {_context.Width}x{_context.Height}");
            GL.Viewport(_context.ClientRectangle);
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75), (float)_context.Width / _context.Height, 1.0f, 5000.0f);
            _ndcRendererMatrix = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);
            _worldRenderer.Resize(_context.ClientRectangle.Width, _context.ClientRectangle.Height, 1.0f);
            _postprocessPipeline.Resize(_context.ClientRectangle.Width, _context.ClientRectangle.Height, 1.0f);
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

            if (e.Key == Key.G)
            {
                _skybox.Update();
            }

            if (e.Key == Key.B)
            {
                ToggleBloom();
            }
        }

        private void ToggleBloom()
        {
            if (_isBloomEnabled)
            {
                _ndcTexture = _worldRenderer.Texture;
            }
            else
            {
                _ndcTexture = _postprocessTexture;
            }

            _ndcRenderer.Clear();
            var fullScreenQuad = new FullScreenQuad(new[] { _ndcTexture }, _fullscreenShader);
            _ndcRenderer.AddToScene(fullScreenQuad);

            _isBloomEnabled = !_isBloomEnabled;
        }

        public void Dispose()
        {
            _logger.Information("Disposing objects");
            _renderer.Dispose();
            _skybox.Dispose();
            _world.Dispose();
            _context.Dispose();
            _worldRenderer.Dispose();
            _ndcRenderer.Dispose();
            _skyboxRenderer.Dispose();
            _fullscreenShader.Dispose();
            _backbufferTexture.Dispose();

            _postprocessTexture.Dispose();
            _postprocessPipeline.Dispose();
        }
    }
}
