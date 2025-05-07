using OpenTK.Mathematics;

namespace ProceduralCity.Camera.Controller.Movements
{
    internal class PathMovement : IMovement
    {
        public Vector3[] Path { get; init; }

        public bool FirstTick { get; set; } = true;

        public int CurrentPathIndex { get; set; } = 0;

        public void Handle(IMovementHandler handler, float deltaTime)
        {
            handler.HandlePathMovement(this, deltaTime);
            FirstTick = false;
        }
    }
}
