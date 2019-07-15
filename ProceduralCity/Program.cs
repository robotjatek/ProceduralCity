using OpenTK.Graphics;
using Serilog;

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

            var config = new AppConfig();

            using (var g = new Game(config.ResolutionWidth, config.ResolutionHeight, GraphicsMode.Default, config.WindowTitle))
            {
                g.Run(config.FrameRate);
            }

            logger.Dispose();
        }
    }
}

