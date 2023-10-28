using OpenTK.Mathematics;

namespace ProceduralCity.Camera.Controller.Movements
{
    internal class PathMovement : IMovement
    {
        public Vector3 StartPosition { get; init; }

        public Vector3 EndPosition { get; init; }

        public bool FirstTick { get; private set; } = true;

        public void Handle(IMovementHandler handler, float deltaTime)
        {
            handler.HandlePathMovement(this, deltaTime);
            FirstTick = false;
        }
    }
}
