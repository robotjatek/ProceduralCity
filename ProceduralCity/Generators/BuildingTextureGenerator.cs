using ProceduralCity.Renderer;
using ProceduralCity.Utils;

using System;

namespace ProceduralCity.Generators
{
    class BuildingTextureGenerator
    {
        private readonly RandomService _randomService;
        private const int width = 1024;
        private const int height = 1024;
        private const int windowSizePx = 8;

        public BuildingTextureGenerator(RandomService randomService)
        {
            _randomService = randomService;
        }

        public Texture GenerateTexture()
        {
            var litProbability = _randomService.Next(40, 100) / 100f;

            var data = new byte[width * height];
            var numWindows = width / windowSizePx;
            for (var i = 0; i < numWindows; i++)
            {
                for (var j = 0; j < numWindows; j++)
                {
                    DrawWindow(i, j, windowSizePx, data, litProbability);
                }
            }

            return Texture.CreateGrayscaleTexture(width, height, data);
        }

        private void DrawWindow(int positionX, int positionY, int windowSizePx, byte[] data, float litProbability)
        {
            var regionSizeX = windowSizePx * 4;
            var regionSizeY = windowSizePx * 4;
            var regionX = positionX / regionSizeX;
            var regionY = positionY / regionSizeY;

            var shouldLightWindow = ShouldLightWindow(regionX, regionY, litProbability);
            if (!shouldLightWindow)
                return;

            var windowBackgroundColor = (byte)_randomService.Next(40, 256);

            var startIndex = (positionY * windowSizePx * width) + (positionX * windowSizePx);

            for (var y = 0; y < windowSizePx; y++)
            {
                for (var x = 0; x < windowSizePx; x++)
                {
                    // Calculate the index of the current pixel in the data array
                    var pixelIndex = startIndex + (y * width) + x;

                    var isBorderPixel = x == 0 || x == windowSizePx - 1 || y == 0 || y == windowSizePx - 1;
                    if (isBorderPixel)
                    {
                        data[pixelIndex] = 0;
                    }
                    else
                    {
                        // Calculate a simple gradient based on the window's vertical position
                        var relativeYWithinWindow = (float)y / (windowSizePx - 1);
                        var gradient = 1.0f - relativeYWithinWindow;

                        var blendedColor = (byte)(windowBackgroundColor + (gradient * 80));

                        var dithering = _randomService.Next(-20, 30);
                        blendedColor = (byte)Math.Max(20, Math.Min(255, blendedColor + dithering));

                        data[pixelIndex] = blendedColor;
                    }
                }
            }
        }

        private bool ShouldLightWindow(int regionX, int regionY, float litProbability)
        {
            // Calculate a probability based on the region's position
            var regionProbability = litProbability * (1.0f - (regionX * regionY) / ((float)(regionX + 1) * (regionY + 1)));

            return _randomService.NextDouble() < regionProbability;
        }
    }
}
