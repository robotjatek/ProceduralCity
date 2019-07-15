using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Serilog;

namespace ProceduralCity
{
    public class OpenGlContext : GameWindow
    {
        private readonly ILogger _logger;

        public OpenGlContext(int width, int height, GraphicsMode mode, string title, ILogger logger) : base(width, height, mode, title)
        {
            _logger = logger;
            var glVendor = GL.GetString(StringName.Vendor);
            var glRenderer = GL.GetString(StringName.Renderer);
            _logger.Information($"Vendor: {glVendor} | Renderer: {glRenderer}");
        }
    }
}
