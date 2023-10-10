namespace ProceduralCity.Camera.Controller.Movements
{
    internal class PlaneMovement : IMovement
    {
        public float VerticalAngle { get; init; }
        public MovementDirection Direction { get; init; }

        public void Handle(IMovementHandler handler, float deltaTime)
        {
            handler.HandlePlaneMovement(this, deltaTime);
        }
    }
}
