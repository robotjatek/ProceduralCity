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
        }

        public static IMovement BuildRandomMovement(MovementParams buildParams)
        {
            Random _random = new();
            var enumValues = Enum.GetValues(typeof(MovementType));
            var movementType = (MovementType)enumValues.GetValue(_random.Next(enumValues.Length));

            switch (movementType)
            {
                case MovementType.STRAIGHT:
                    {
                        var distanceToCityCenter = (buildParams.CityCenterPosition - buildParams.CameraPosition).Length;
                        return new StraightMovement
                        {
                            Direction = distanceToCityCenter > buildParams.MaxDistance ? MovementDirection.A : MovementDirection.B
                        };
                    }

                case MovementType.ROTATE:
                    return new RotateMovement
                    {
                        Direction = (MovementDirection)_random.Next(2)
                    };
                case MovementType.STAND:
                    return new StandMovement();
                default:
                    throw new NotImplementedException($"Unknown movement type: {nameof(movementType)}");
            }
        }
    }
}
