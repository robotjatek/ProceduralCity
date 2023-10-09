using ProceduralCity.Camera.Controller;

namespace ProceduralCity.Camera.Controller.Movements
{
    interface IMovement
    {
        void Handle(IMovementHandler handler, float deltaTime);
    }
}
