using System.Collections.Generic;
using ProceduralCity.Renderer;

namespace ProceduralCity.Generators
{
    interface IBillboardTextureGenerator
    {
        void GenerateBillboardTextures(IEnumerable<Texture> textures);
    }
}