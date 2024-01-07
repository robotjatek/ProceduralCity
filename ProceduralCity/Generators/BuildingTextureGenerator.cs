using OpenTK.Graphics.OpenGL;

using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Generators
{
    class BuildingTextureGenerator
    {
        private readonly RandomService _randomService;

        public BuildingTextureGenerator(RandomService randomService)
        {
            _randomService = randomService;
        }

        // TODO: Use compute shaders to generate textures.
        public Texture GenerateTexture()
        {
            // TODO: grayscale image
            // TODO: Use correct opengl format for grayscale textures (GL_RED seems to be the one)

            const int width = 1024;
            const int height = 1024;
            const int windowSizePx = 8;

            var data = new byte[width * 3 * height];
            // TODO: in dotnet array init to 0 by default. This is useless. For debug purposes I init the whole texture to red to see if parts are missing or not
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width * 3; i += 3)
                {
                    data[j * width * 3 + i] = 255;
                    data[j * width * 3 + i + 1] = 0;
                    data[j * width * 3 + i + 2] = 0;
                }
            }

            var numWindows = width / windowSizePx;
            for (var i = 0; i < numWindows; i++)
            {
                for (var j = 0; j < numWindows; j++)
                {
                    DrawWindow(i, j, windowSizePx, data);
                }
            }

            return new Texture(width, height, data, PixelInternalFormat.Rgb, PixelFormat.Rgb); // TODO: use the static CreateGrayscaleTexture method instead
        }

        private void DrawWindow(int positionX, int positionY, int windowSizePx, byte[] data)
        {
            var windowBackgroundColor = (byte)_randomService.Next(256); // TODO: generate a more deterministic color to create "patches" of lit windows

            const int width = 1024; // TODO: delete

            var startIndex = (positionY * windowSizePx * 3 * width) + (positionX * windowSizePx * 3);

            for (var y = 0; y < windowSizePx; y++)
            {
                for (var x = 0; x < windowSizePx; x++)
                {
                    // Calculate the index of the current pixel in the data array
                    var pixelIndex = startIndex + (y * width * 3) + (x * 3);

                    var isBorderPixel = x == 0 || x == windowSizePx - 1 || y == 0 || y == windowSizePx - 1;
                    if (isBorderPixel)
                    {
                        data[pixelIndex] = 0;
                        data[pixelIndex + 1] = 0;
                        data[pixelIndex + 2] = 0;
                    }
                    else
                    {
                        data[pixelIndex] = windowBackgroundColor;
                        data[pixelIndex + 1] = windowBackgroundColor;
                        data[pixelIndex + 2] = windowBackgroundColor;
                    }
                }
            }
        }
    }
}
