using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Serilog;

namespace ProceduralCity
{
    public class OpenGlContext : GameWindow
    {
        private readonly ILogger _logger;

        public bool IsFullscreen { get; private set; }

        public OpenGlContext(int width, int height, GraphicsMode mode, string title, ILogger logger) : base(width, height, mode, title)
        {
            _logger = logger;
            var glVendor = GL.GetString(StringName.Vendor);
            var glRenderer = GL.GetString(StringName.Renderer);
            _logger.Information($"Vendor: {glVendor} | Renderer: {glRenderer}");
        }

        public void ToggleFullscreen()
        {
            if (IsFullscreen == true)
            {
                this.WindowState = WindowState.Normal;
                this.CursorVisible = true;
            }
            else
            {
                this.WindowState = WindowState.Fullscreen;
                this.CursorVisible = false;
            }

            IsFullscreen = !IsFullscreen;
        }
    }
}
