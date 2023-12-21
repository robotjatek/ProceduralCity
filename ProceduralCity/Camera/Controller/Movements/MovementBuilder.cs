using ProceduralCity.Utils;

using System;

namespace ProceduralCity.Camera.Controller.Movements
{
    static class MovementBuilder
    {
        enum MovementType
        {
            STRAIGHT,
            STAND,
            ROTATE,
            PLANE,
            PLANE_STRAFE
        }

        public static IMovement BuildRandomMovement(MovementParams buildParams, RandomService randomService)
        {
            var enumValues = Enum.GetValues(typeof(MovementType));
            var movementType = (MovementType)enumValues.GetValue(randomService.Next(enumValues.Length));
            var distanceToCityCenter = (buildParams.CityCenterPosition - buildParams.CameraPosition).Length;

            switch (movementType)
            {
                case MovementType.STRAIGHT:
                    {
                        return new StraightMovement
                        {
                            Direction = distanceToCityCenter > buildParams.MaxDistance ? MovementDirection.A : MovementDirection.B
                        };
                    }
                case MovementType.ROTATE:
                    return new RotateMovement
                    {
                        Direction = (MovementDirection)randomService.Next(2)
                    };
                case MovementType.STAND:
                    return new StandMovement();
                case MovementType.PLANE:
                    {
                        var verticalAngle = randomService.Next(-90, 0); // -90 straight down, 0 staight up (in deg)
                        return new PlaneMovement
                        {
                            VerticalAngle = verticalAngle,
                            Direction = distanceToCityCenter > buildParams.MaxDistance ? MovementDirection.A : MovementDirection.B
                        };
                    }
                case MovementType.PLANE_STRAFE:
                    {
                        var verticalAngle = randomService.Next(-90, 0); // -90 straight down, 0 staight up (in deg)
                        return new PlaneStrafeMovement
                        {
                            VerticalAngle = verticalAngle,
                            Direction = distanceToCityCenter > buildParams.MaxDistance ? MovementDirection.A : MovementDirection.B
                        };
                    }
                default:
                    throw new NotImplementedException($"Unknown movement type: {nameof(movementType)}");
            }
        }
    }
}
