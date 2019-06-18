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
                .CreateLogger();
            Log.Logger = logger;

            using (var g = new Game(800, 600, GraphicsMode.Default, "City"))
            {
                g.Run(60.0f);
            }

            logger.Dispose();
        }
    }
}

