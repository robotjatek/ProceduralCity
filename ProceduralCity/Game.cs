﻿using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    //TODO: cull face
    public class Game : GameWindow
    {
        private readonly string _title;
        private readonly Renderer.Renderer _renderer;
        private readonly Skybox _skybox;
        private readonly Camera _camera = new Camera(new Vector3(96, 70, -80), 50, 20);

        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _modelMatrix = Matrix4.Identity;

        private readonly Shader _buildingShader;
        private readonly Texture _buildingTexture;

        private readonly Building _building;

        public Game(int width, int height, GraphicsMode mode, string title) : base(width, height, mode, title)
        {
            _title = title;

            var glVendor = GL.GetString(StringName.Vendor);
            var glRenderer = GL.GetString(StringName.Renderer);
            Log.Information($"Vendor: {glVendor} | Renderer: {glRenderer}");

            GL.Enable(EnableCap.DepthTest);
         //   GL.Enable(EnableCap.CullFace);

            _renderer = new Renderer.Renderer();
            _skybox = new Skybox();

            GL.ClearColor(Color4.Green);

            _buildingShader = new Shader("vs.vert", "fs.frag");
            _buildingTexture = new Texture("building/1.jpg");
            _building = new Building(new Vector3(0, 0, 0), new Vector2(10f, 10f), _buildingTexture, _buildingShader);
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
            _skybox.Render(_projectionMatrix, viewMatrix);
            GL.DepthFunc(DepthFunction.Lequal);
            _renderer.RenderScene(_projectionMatrix, viewMatrix, _modelMatrix);

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
            _skybox?.Dispose();

            _buildingShader?.Dispose();
            _buildingTexture?.Dispose();
        }
    }
}
