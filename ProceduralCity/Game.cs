using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace ProceduralCity
{
    public class Game : GameWindow
    {
        private readonly Renderer.Renderer _renderer = new Renderer.Renderer();

        public Game(int width, int height, GraphicsMode mode, string title) : base(width, height, mode, title)
        {

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

        }
    }
}
