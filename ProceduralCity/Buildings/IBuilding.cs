using ProceduralCity.Renderer;

namespace ProceduralCity.Buildings
{
    interface IBuilding : IRenderable
    {
        bool HasBillboard { get; }

        Billboard Billboard { get; }
    }
}
