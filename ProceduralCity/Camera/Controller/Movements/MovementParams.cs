using OpenTK.Mathematics;

namespace ProceduralCity.Camera.Controller.Movements
{
    class MovementParams
    {
        public Vector3 CityCenterPosition { get; init; }
        public Vector3 CameraPosition { get; init; }
        public double MaxDistance { get; init; }
    }
}
