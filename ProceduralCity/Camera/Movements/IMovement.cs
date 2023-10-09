namespace ProceduralCity.Camera.Movements
{
    interface IMovement
    {
        void Handle(IMovementHandler handler, float deltaTime);
    }
}
