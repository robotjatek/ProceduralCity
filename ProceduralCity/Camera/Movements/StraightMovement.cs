namespace ProceduralCity.Camera.Movements
{
    class StraightMovement : IMovement
    {
        public MovementDirection Direction { get; init; }

        public void Handle(IMovementHandler controller, float deltaTime)
        {
            controller.HandleStraightMovement(this, deltaTime);
        }
    }
}
