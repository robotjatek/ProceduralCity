namespace ProceduralCity.Camera.Controller.Movements
{
    class RotateMovement : IMovement
    {
        public MovementDirection Direction { get; init; }

        public void Handle(IMovementHandler controller, float deltaTime)
        {
            controller.HandleRotateMovement(this, deltaTime);
        }
    }
}
