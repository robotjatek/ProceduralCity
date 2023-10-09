namespace ProceduralCity.Camera.Controller.Movements
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
