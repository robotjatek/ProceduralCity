using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using Serilog;

namespace ProceduralCity
{
    public class OpenGlContext : GameWindow
    {
        private readonly ILogger _logger;

        public OpenGlContext(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, ILogger logger) : base(gameWindowSettings, nativeWindowSettings)
        {
            _logger = logger;
            var glVendor = GL.GetString(StringName.Vendor);
            var glRenderer = GL.GetString(StringName.Renderer);
            _logger.Information("Vendor: {glVendor} | Renderer: {glRenderer}", glVendor, glRenderer);
            VSync = VSyncMode.Off;
        }

        public void ToggleFullscreen()
        {
            if (IsFullscreen == true)
            {
                this.WindowState = WindowState.Normal;
                this.CursorState = CursorState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Fullscreen;
                this.CursorState = CursorState.Hidden;
            }
        }

        public void ToggleVSync()
        {
            if (VSync == VSyncMode.On)
            {
                this.VSync = VSyncMode.Off;
            }
            else if (VSync == VSyncMode.Off)
            {
                this.VSync = VSyncMode.On;
            }
        }
    }
}
