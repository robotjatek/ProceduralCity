using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProceduralCity.Camera;
using ProceduralCity.Camera.Controller;
using ProceduralCity.Config;
using ProceduralCity.GameObjects;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using ProceduralCity.Renderer.PostProcess;
using ProceduralCity.Renderer.Uniform;
using ProceduralCity.Renderer.Utils;

using Serilog;

using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Immutable;

namespace ProceduralCity
{
    // High priority tasks
    //TODO: render onscreen text AFTER the postprocess pipeline

    //TODO: show fps counter on screen instead of the titlebar
    //TODO: dynamic text rendering
    //TODO: generate building textures procedurally
    //TODO: more building types
    //TODO: add more variety to the existing building types
    //TODO: Do not render hidden traffic lights
    //TODO: Building LOD levels

    // Low priority tasks
    //TODO: Render skybox into texture when generating the world, to reduce GPU usage
    //TODO: Add the ability to render post process effects in a lower resolution
    //TODO: Generators should not own any texture or shader references, these should be asked from a resource manager class
    //TODO: dispose all generators after the generation has been completed
    //TODO: Incorporate shared logic between InstancedBatch and ObjectBatch into a shared class
    //TODO: Mipmaping modes for generated textures (created with new Texture(w,h))
    //TODO: add decal rendering (stencil buffer?) [streetlights, billboards] (polygonOffset opengl?)
    //TODO: add state change capability to the renderer
    class Game : IGame, IDisposable
    {
        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _ndcRendererMatrix = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);

        private readonly IAppConfig _config;
        private readonly ILogger _logger;
        private readonly IRenderer _renderer;
        private readonly IRenderer _ndcRenderer;
        private readonly IRenderer _skyboxRenderer;

        private readonly IRenderer _textRenderer;
        private Matrix4 _textRendererMatrix = Matrix4.Identity;
        private readonly Textbox _textbox = new Textbox("Consolas")
            .WithText("Árvíztűrő tükörfúrógép", new Vector2(10, 0));

        private readonly ISkybox _skybox;
        private readonly ICamera _camera;
        private readonly CameraController _cameraController;
        private readonly IWorld _world;
        private readonly ColorGenerator _colorGenerator;

        private readonly OpenGlContext _context;
        private readonly BackBufferRenderer _worldRenderer;
        private readonly Texture _backbufferTexture;
        private readonly Shader _fullscreenShader;

        private bool _isBloomEnabled = true;
        private readonly PostprocessPipeline _postprocessPipeline;

        private double _elapsedFrameTime = 0;

        public Game(
            IAppConfig config,
            ILogger logger,
            ICamera camera,
            CameraController cameraController,
            IWorld world,
            OpenGlContext context,
            IRenderer renderer,
            IRenderer ndcRenderer,
            IRenderer skyboxRenderer,
            IRenderer textRenderer,
            ISkybox skybox,
            ColorGenerator colorGenerator)
        {
            _camera = camera;
            _cameraController = cameraController;
            _cameraController.SetFadeout = SetFadeout;
            _logger = logger;
            _config = config;

            _context = context;
            ConfigureContext();

            _skybox = skybox;
            _world = world;
            _colorGenerator = colorGenerator;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.ClearColor(Color4.Green);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _renderer = renderer;
            _ndcRenderer = ndcRenderer;
            _textRenderer = textRenderer;
            _textRenderer.AddToScene(_textbox.Text);
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

            _renderer.AddToScene(_world.Traffic); // TODO: rendering traffic should be dynamic, based on the camera frustum
            _renderer.AddToScene(_world.Renderables);

            _backbufferTexture = new Texture(_config.ResolutionWidth, config.ResolutionHeight);
            _worldRenderer = new BackBufferRenderer(
                _logger,
                _backbufferTexture,
                _config.ResolutionWidth,
                _config.ResolutionHeight,
                useDepthBuffer: true);

            _postprocessPipeline = new PostprocessPipeline(_logger, _config, _worldRenderer.Texture);

            _fullscreenShader = new Shader("vs.vert", "fullscreen.frag");
            _fullscreenShader.SetUniformValue("tex", new IntUniform
            {
                Value = 0
            });

            var fullScreenQuad = new FullScreenQuad(new[] { _worldRenderer.Texture }, _fullscreenShader);
            _ndcRenderer.AddToScene(fullScreenQuad);
        }

        private void ConfigureContext()
        {
            _context.RenderFrame += this.OnRenderFrame;
            _context.UpdateFrame += this.OnUpdateFrame;
            _context.Size = new Vector2i(_config.ResolutionWidth, _config.ResolutionHeight);
            _context.Resize += (e) => OnResize();
            _context.KeyDown += OnKeyDown;
        }

        public void RunGame()
        {
            _context.Run();
        }

        private void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboardState = _context.KeyboardState;
            HandleCameraInput(e, keyboardState);
            _cameraController.Update((float)e.Time);

            var sites = _world.BspTree.GetLeavesInFrustum(_camera).ToImmutableArray();

            var culledTrafficInstanes = sites
                .SelectMany(site => site.Traffic)
                .AsParallel()
                .Where(traffic => _camera.IsInViewFrustum(traffic.Position))
                .Where(traffic => Vector3.DistanceSquared(traffic.Position, _camera.Position) < 490000f) // discard everything that is further than 700f
                .ToImmutableArray();

            Parallel.ForEach(culledTrafficInstanes, t => t.Move((float)e.Time)); // Only animate visible traffic
        }

        private void HandleCameraInput(FrameEventArgs e, KeyboardState keyboardState)
        {
            if (keyboardState[Keys.A])
            {
                _camera.StrafeLeft((float)e.Time);
            }
            else if (keyboardState[Keys.D])
            {
                _camera.StrafeRight((float)e.Time);
            }

            if (keyboardState[Keys.W])
            {
                _camera.MoveForward((float)e.Time);
            }
            else if (keyboardState[Keys.S])
            {
                _camera.MoveBackward((float)e.Time);
            }

            if (keyboardState[Keys.Up])
            {
                _camera.SetVertical(1.0f, (float)e.Time);
            }
            else if (keyboardState[Keys.Down])
            {
                _camera.SetVertical(-1.0f, (float)e.Time);
            }

            if (keyboardState[Keys.Left])
            {
                _camera.SetHorizontal(-1.0f, (float)e.Time);
            }
            else if (keyboardState[Keys.Right])
            {
                _camera.SetHorizontal(1.0f, (float)e.Time);
            }
        }

        private void OnRenderFrame(FrameEventArgs e)
        {
            CountFps(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            var viewMatrix = _camera.ViewMatrix;

            _worldRenderer.Clear();
            _worldRenderer.RenderToTexture(_skyboxRenderer, _projectionMatrix, new Matrix4(new Matrix3(viewMatrix)));
            _worldRenderer.RenderToTexture(_renderer, _projectionMatrix, viewMatrix);
            _worldRenderer.RenderToTexture(_textRenderer, _textRendererMatrix, Matrix4.Identity);
            if (_isBloomEnabled)
                _postprocessPipeline.RunPipeline();

            GL.Viewport(0, 0, _context.ClientRectangle.Size.X, _context.ClientRectangle.Size.Y);
            _ndcRenderer.RenderScene(_ndcRendererMatrix, Matrix4.Identity);
            _context.SwapBuffers();
        }

        private void CountFps(FrameEventArgs e)
        {
            _elapsedFrameTime += e.Time;
            if (_elapsedFrameTime >= 0.4)
            {
                _context.Title = $"{_config.WindowTitle} - FPS: {Math.Round(1f / e.Time, 0)}";
                _elapsedFrameTime = 0;
            }
        }

        private void OnResize()
        {
            _logger.Information("Window resized: {x}x{y}", _context.Size.X, _context.Size.Y);
            GL.Viewport(0, 0, _context.ClientRectangle.Size.X, _context.ClientRectangle.Size.Y);
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75), (float)_context.ClientRectangle.Size.X / _context.ClientRectangle.Size.Y, 1.0f, 4000.0f);
            _camera.ProjectionMatrix = _projectionMatrix;
            _textRendererMatrix = Matrix4.CreateOrthographicOffCenter(0, _context.ClientRectangle.Size.X, _context.ClientRectangle.Size.Y, 0, -1, 1);
            _worldRenderer.Resize(_context.ClientRectangle.Size.X, _context.ClientRectangle.Size.Y, 1.0f);
            _postprocessPipeline.Resize(_context.ClientRectangle.Size.X, _context.ClientRectangle.Size.Y, 1.0f);
        }

        private void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.Escape)
            {
                _context.Close();
            }

            if (e.Alt && e.Key == Keys.Enter)
            {
                _context.ToggleFullscreen();
            }

            if (e.Key == Keys.F1)
            {
                _context.ToggleVSync();
            }

            if (e.Key == Keys.G)
            {
                _colorGenerator.GenerateColors();
                _skybox.Update();
            }

            if (e.Key == Keys.B)
            {
                ToggleBloom();
            }

            if (e.Key == Keys.F2)
            {
                ToggleFrameLimit();
            }

            if (e.Key == Keys.T)
            {
                _cameraController.TeleportToNewPosition();
            }

            if (e.Key == Keys.F)
            {
                _cameraController.ToggleFlyby();
            }
        }

        private void ToggleFrameLimit()
        {
            _context.UpdateFrequency = _context.UpdateFrequency == 0 ? _config.FrameRate : 0;
        }

        private void ToggleBloom()
        {
            _isBloomEnabled = !_isBloomEnabled;
        }

        private void SetFadeout(float fadeout)
        {
            _fullscreenShader.SetUniformValue("fadeFactor", new FloatUniform
            {
                Value = fadeout
            });
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
            _postprocessPipeline.Dispose();
        }
    }
}
