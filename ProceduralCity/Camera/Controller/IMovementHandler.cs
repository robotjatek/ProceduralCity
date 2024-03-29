﻿using ProceduralCity.Camera.Controller.Movements;

namespace ProceduralCity.Camera.Controller
{
    interface IMovementHandler
    {
        void HandleStraightMovement(StraightMovement movement, float deltaTime);
        void HandleRotateMovement(RotateMovement movement, float deltaTime);
        void HandleStandMovement(StandMovement movement, float deltaTime);
        void HandlePlaneMovement(PlaneMovement movement, float deltaTime);
        void HandlePlaneStrafeMovement(PlaneStrafeMovement movement, float deltaTime);
    }
}
