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
using System.Diagnostics;

namespace ProceduralCity
{
    // High priority tasks
    //TODO: generate building textures procedurally
    //TODO: more building types
    //TODO: add more variety to the existing building types
    //TODO: Further traffic light optimizations:
    //    Before optimizations: ~4000-4100 frames in 30 seconds, ~8000 frames in 60 seconds
    //    After reducing mesh count to one: ~5400 frames in 30 seconds, ~10000 frames in 60 seconds
    //    Rendering only the lights that are in the camera frustum: ~11000 frames in 60 seconds in general situations, BUT:
    //         -- FPS can go up into unseen heights: ~500-700 FPS
    //         -- Occlusion culling potentially can make this much faster
    //    Updating traffic lights at 60 fps: ~52000 frames in 60 seconds
    //    TODO: Optimizations:
    //          - Fix light position problem - position should mean the center of the light
    //          - Calculate model matrix on gpu for traffic lights => create a vertex shader that is the variation of the instanced_vert. Send position vector and lookat vector instead of model matrix
    //TODO: Occlusion cull traffic lights
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
        private Stopwatch _stopwatch = new();
        private int _frames = 0;
        private readonly Textbox _benchmarkTextbox = new("Consolas");

        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _ndcRendererMatrix = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);

        private readonly IAppConfig _config;
        private readonly ILogger _logger;
        private readonly IRenderer _renderer;
        private readonly IRenderer _ndcRenderer;
        private readonly IRenderer _skyboxRenderer;
        private readonly IRenderer _trafficRenderer;
        private readonly Matrix4[] _trafficMatrixCache;
        private readonly InstancedBatch _trafficInstanceBatch;

        private readonly IRenderer _textRenderer;
        private Matrix4 _textRendererMatrix = Matrix4.Identity;
        private readonly Textbox _fpsCounterTextbox = new("Consolas");
        private readonly Textbox _visibleLightsTextbox = new("Consolas");
        private readonly Textbox _lightsInFrustumTextbox = new("Consolas");
        private readonly Textbox _allTrafficLightsTextbox = new("Consolas");

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
            IRenderer trafficRenderer,
            ISkybox skybox,
            ColorGenerator colorGenerator)
        {
            _benchmarkTextbox.WithText("Collecting data...", new Vector2(0, 150), 0.5f);
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
            _textRenderer.BeforeRender = () => GL.Enable(EnableCap.Blend);
            _textRenderer.AfterRender = () => GL.Disable(EnableCap.Blend);

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

            _trafficRenderer = trafficRenderer;
            _trafficInstanceBatch = _trafficRenderer.AddAsInstanced(_world.Traffic.First().Meshes.First());
            _trafficMatrixCache = new Matrix4[_world.Traffic.Count()];
            _trafficRenderer.BeforeRender = () => GL.Disable(EnableCap.CullFace);
            _trafficRenderer.AfterRender = () => GL.Enable(EnableCap.CullFace);

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

            _stopwatch.Start();
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


        private double timeSinceLastTrafficUpdate = 0;

        private void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboardState = _context.KeyboardState;
            HandleCameraInput(e, keyboardState);
            _cameraController.Update((float)e.Time);

            if (timeSinceLastTrafficUpdate > 1.0f / _config.TrafficLightUpdateRate)
            {
                UpdateTraffic(timeSinceLastTrafficUpdate);
            }
            timeSinceLastTrafficUpdate += e.Time;
        }

        private void UpdateTraffic(double delta)
        {
            var visibleTraffic = _world.BspTree.GetLeavesInFrustum(_camera).SelectMany(site => site.Traffic);

            var visibleTrafficInstancesToUpdate = visibleTraffic
                .AsParallel()
                /* 
                 * This may not worth the effort at all. Per site culling culls most of the traffic outside the view frustum already.
                 * There is little to win here (IF ANY!) with one more per light pass
                 */
                //.Where(traffic => _camera.IsInViewFrustum(traffic.Position)) 
                .Where(traffic => Vector3.DistanceSquared(traffic.Position, _camera.Position) < 490000f) // discard everything that is further than 700f
                .ToImmutableList();


            var trafficModels = visibleTraffic.Select(t => t.Model);
            var trafficCount = trafficModels.Count();
            Parallel.ForEach(
                trafficModels,
                (modelMatrix, _, i) =>
                {
                    _trafficMatrixCache[i] = modelMatrix;
                });
            _trafficInstanceBatch.UpdateModels(_trafficMatrixCache, trafficCount);
            Parallel.ForEach(visibleTrafficInstancesToUpdate, t => t.Move((float)delta)); // Only animate visible traffic

            _visibleLightsTextbox.WithText($"Traffic to update: {visibleTrafficInstancesToUpdate.Count}", new Vector2(0, 30), 0.5f);
            _lightsInFrustumTextbox.WithText($"Traffic lights in camera frustum: {trafficCount}", new Vector2(0, 60), 0.5f);
            _allTrafficLightsTextbox.WithText($"All traffic lights: {_world.Traffic.Count()}", new Vector2(0, 90), 0.5f);

            timeSinceLastTrafficUpdate = 0;
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
            _textRenderer.Clear();
            _textRenderer.AddToScene(_fpsCounterTextbox.Text);
            _textRenderer.AddToScene(_visibleLightsTextbox.Text);
            _textRenderer.AddToScene(_lightsInFrustumTextbox.Text);
            _textRenderer.AddToScene(_allTrafficLightsTextbox.Text);
            _textRenderer.AddToScene(_benchmarkTextbox.Text);

            CountFps(e.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            var viewMatrix = _camera.ViewMatrix;

            _worldRenderer.Clear();
            _worldRenderer.RenderToTexture(_skyboxRenderer, _projectionMatrix, new Matrix4(new Matrix3(viewMatrix)));
            _worldRenderer.RenderToTexture(_renderer, _projectionMatrix, viewMatrix);
            _worldRenderer.RenderToTexture(_trafficRenderer, _projectionMatrix, viewMatrix);

            if (_isBloomEnabled)
                _postprocessPipeline.RunPipeline();

            _worldRenderer.RenderToTexture(_textRenderer, _textRendererMatrix, Matrix4.Identity);

            GL.Viewport(0, 0, _context.ClientRectangle.Size.X, _context.ClientRectangle.Size.Y);
            _ndcRenderer.RenderScene(_ndcRendererMatrix, Matrix4.Identity);
            _context.SwapBuffers();
            if (_stopwatch.ElapsedMilliseconds < 60000) // Collect data for 60 seconds
            {
                _frames++;
            }
            else
            {
                _benchmarkTextbox.WithText($"Frames: {_frames}", new Vector2(0, 150), 0.5f);
            }
        }

        private void CountFps(double elapsed)
        {
            _elapsedFrameTime += elapsed;
            if (_elapsedFrameTime >= 1.0)
            {
                var fps = Math.Round(1f / elapsed, 0);
                _fpsCounterTextbox.WithText(text: $"{fps} FPS", scale: 0.5f);

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
            _fpsCounterTextbox.Dispose();
            _visibleLightsTextbox.Dispose();
            _lightsInFrustumTextbox.Dispose();
            _allTrafficLightsTextbox.Dispose();
            _trafficRenderer.Dispose();
        }
    }
}
