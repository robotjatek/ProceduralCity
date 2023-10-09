namespace ProceduralCity.Camera.Movements
{
    class StandMovement : IMovement
    {
        public void Handle(IMovementHandler controller, float deltaTime)
        {
            controller.HandleStandMovement(this, deltaTime);
        }
    }
}
