using OpenTK.Mathematics;

using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Generators
{
    class BuildingTextureInfo
    {
        public required Texture Texture { get; init; }
        public required int WindowSizePx { get; init; }
        public int NumWindowsX => Texture.Width / WindowSizePx;
        public int NumWindowsY => Texture.Height / WindowSizePx;
        public float WindowWidth => 1f / NumWindowsX;
        public float WindowHeight => 1f / NumWindowsY;

        private readonly RandomService _randomService;

        public BuildingTextureInfo(RandomService randomService)
        {
            _randomService = randomService;
        }

        /// <summary>
        /// Returns with a random UV coordinate on a window corner.
        /// </summary>
        /// <returns>A vector containing the UV coordinates of a random window</returns>
        public Vector2 RandomWindowUV()
        {
            var windowX = _randomService.Next(0, NumWindowsX);
            var windowY = _randomService.Next(0, NumWindowsY);
            var startX = (float)windowX / NumWindowsX;
            var startY = (float)windowY / NumWindowsY;
            var textureStartPosition = new Vector2(startX, startY);
            return textureStartPosition;
        }
    }
}
