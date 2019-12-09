using System;
using OpenTK;
using ProceduralCity.GameObjects;

namespace ProceduralCity.Generators
{
    interface IBillboardBuilder : IDisposable
    {
        float CalculateBillboardWidth(float height);
        Billboard CreateNorthFacingBillboard(Vector3 position, Vector2 area, float height);
        Billboard CreateSouthFacingBillboard(Vector3 position, Vector2 area, float height);
        Billboard CreateWestFacingBillboard(Vector3 position, Vector2 area, float height);
        Billboard CreateEastFacingBillboard(Vector3 position, Vector2 area, float height);
        bool HasBillboards();
    }
}