using OpenTK.Graphics;

namespace ProceduralCity
{
    public static class Program
    {
        public static void Main()
        {
            using (var g = new Game(800, 600, GraphicsMode.Default, "City"))
            {
                g.Run(60.0f);
            }
        }
    }
}

