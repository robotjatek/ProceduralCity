using System;

namespace ProceduralCity.Camera.Controller.Movements
{
    internal class PlaneStrafeMovement : IMovement
    {
        public MovementDirection Direction { get; init; }
        public float VerticalAngle { get; init; }

        public void Handle(IMovementHandler handler, float deltaTime)
        {
            handler.HandlePlaneStrafeMovement(this, deltaTime);
        }
    }
}
