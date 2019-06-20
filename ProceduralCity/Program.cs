using OpenTK.Graphics;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace ProceduralCity
{
    public static class Program
    {
        public static void Main()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();
            Log.Logger = logger;

            using (var g = new Game(800, 600, GraphicsMode.Default, "City"))
            {
                var frameRate = 60.0f;
                g.Run(frameRate);
            }

            logger.Dispose();
        }
    }
}

