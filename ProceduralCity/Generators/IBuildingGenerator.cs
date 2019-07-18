using System;
using System.Collections.Generic;
using ProceduralCity.Buildings;

namespace ProceduralCity.Generators
{
    public interface IBuildingGenerator : IDisposable
    {
        IEnumerable<IBuilding> GenerateBuildings(IEnumerable<GroundNode> sites);
    }
}