using System;
using System.Collections.Generic;

namespace ProceduralCity.Generators
{
    public interface IBuildingGenerator : IDisposable
    {
        IEnumerable<Building> GenerateBuildings(IEnumerable<GroundNode> sites);
    }
}