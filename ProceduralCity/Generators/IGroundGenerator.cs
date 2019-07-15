using System.Collections.Generic;

namespace ProceduralCity.Generators
{
    internal interface IGroundGenerator
    {
        IEnumerable<GroundNode> Generate();
    }
}