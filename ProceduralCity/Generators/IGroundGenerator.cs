using System;
using System.Collections.Generic;
using ProceduralCity.Renderer;

namespace ProceduralCity.Generators
{
    internal interface IGroundGenerator : IDisposable
    {
        IEnumerable<GroundNode> GenerateSites();
        IRenderable CreateGroundPlane();
        IEnumerable<IRenderable> CreateStreetLights(IEnumerable<GroundNode> sites);
    }
}