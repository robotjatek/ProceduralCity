namespace ProceduralCity.Camera.Movements
{
    interface IMovementHandler
    {
        void HandleStraightMovement(StraightMovement movement, float deltaTime);
        void HandleRotateMovement(RotateMovement movement, float deltaTime);
        void HandleStandMovement(StandMovement movement, float deltaTime);
    }
}
