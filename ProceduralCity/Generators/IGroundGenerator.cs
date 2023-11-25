using System;
using System.Collections.Generic;
using ProceduralCity.GameObjects;
using ProceduralCity.Renderer;

namespace ProceduralCity.Generators
{
    internal interface IGroundGenerator : IDisposable
    {
        BspTree GenerateSites();
        IRenderable CreateGroundPlane();
        IEnumerable<IRenderable> CreateStreetLights(IEnumerable<GroundNode> sites);
        IEnumerable<TrafficLight> CreateTrafficLights(IEnumerable<GroundNode> sites);
    }
}