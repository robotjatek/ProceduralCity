using OpenTK.Mathematics;

using System.Collections.Generic;

namespace ProceduralCity.Camera.Controller.Movements
{
    internal class PathMovement : IMovement
    {
        public IEnumerable<Vector3> Path { get; init; }

        public bool FirstTick { get; private set; } = true;

        public void Handle(IMovementHandler handler, float deltaTime)
        {
            handler.HandlePathMovement(this, deltaTime);
            FirstTick = false;
        }
    }
}
