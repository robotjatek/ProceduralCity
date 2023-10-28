using OpenTK.Mathematics;

using ProceduralCity.Generators;

using System;
using System.Linq;

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
            PLANE_STRAFE,
            PATH,
        }

        public static IMovement BuildRandomMovement(MovementParams buildParams)
        {
            Random _random = new();
            var enumValues = Enum.GetValues(typeof(MovementType));
            var movementType = (MovementType)enumValues.GetValue(_random.Next(enumValues.Length));
            var distanceToCityCenter = (buildParams.CityCenterPosition - buildParams.CameraPosition).Length;
            
            switch (MovementType.PATH)
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
                        Direction = (MovementDirection)_random.Next(2)
                    };
                case MovementType.STAND:
                    return new StandMovement();
                case MovementType.PLANE:
                    {
                        var verticalAngle = _random.Next(0, 90);
                        return new PlaneMovement
                        {
                            VerticalAngle = verticalAngle,
                            Direction = distanceToCityCenter > buildParams.MaxDistance ? MovementDirection.A : MovementDirection.B
                        };
                    }
                case MovementType.PLANE_STRAFE:
                    {
                        var verticalAngle = _random.Next(0, 90);
                        return new PlaneStrafeMovement
                        {
                            VerticalAngle = verticalAngle,
                            Direction = distanceToCityCenter > buildParams.MaxDistance ? MovementDirection.A : MovementDirection.B
                        };
                    }
                case MovementType.PATH:
                    var levelChilds = buildParams.World.SitesBsp.Root.Children;
                    var allPossibleWaypointsOnTheCurrentLevel = levelChilds.SelectMany(r => new[]
                    {
                        r.TopLeftCorner,
                        r.TopRightCorner,
                        r.BottomLeftCorner,
                        r.BottomRightCorner
                    })
                        .Where(n => NotOnWorldCorners(buildParams.World.SitesBsp.Root, n)) // Exclude the world corners (0,0), (0,MAX_Y), (MAX_X,0), (MAX_X, MAX_Y)
                        .Distinct();

                    var randomWaypoint = allPossibleWaypointsOnTheCurrentLevel.ElementAt(_random.Next(allPossibleWaypointsOnTheCurrentLevel.Count()));
                    var nodes = buildParams.World.SitesBsp.Root.NodesOnPoint(randomWaypoint);
                    var nextPossibleWaypoints  = nodes.SelectMany(n => (new[]
                    {
                        n.TopLeftCorner,
                        n.TopRightCorner,
                        n.BottomLeftCorner,
                        n.BottomRightCorner
                    })).Except(new[] { randomWaypoint })
                    .Where(n => // Only differ in EXACTLY one coordinate (either X or Y) essentially giving back a neighbour AND not on world corners
                    {
                        return DiffersInOneAxisOnly(n, randomWaypoint) && NotOnWorldCorners(buildParams.World.SitesBsp.Root, n);

                    })
                    .Distinct();
                    var randomNextWaypoint = nextPossibleWaypoints.ElementAt(_random.Next(nextPossibleWaypoints.Count()));

                    var nn = buildParams.World.SitesBsp.Root.NodesOnPoint(randomNextWaypoint).ToArray();

                    // Partially completed:
                    // select 1 random out of the 9 possible
                    // Get all neighbouring waypoints for the selected position
                    // Select a neigbouring waypoint
                    // Build a path with the start and the selected neighbour
                    // -----------------------------------------------

                    // TODO: camera errors in some movement cases
                    // TODO: repeat steps to create a path
                    // TODO: in the future: Create bezier curve with the path


                    var startPos = new Vector3(randomWaypoint.X, 10, randomWaypoint.Y);
                    var endPos = new Vector3(randomNextWaypoint.X, 10, randomNextWaypoint.Y);

                    return new PathMovement
                    {
                        StartPosition = startPos,
                        EndPosition = endPos
                    };
                default:
                    throw new NotImplementedException($"Unknown movement type: {nameof(movementType)}");
            }
        }

        private static bool DiffersInOneAxisOnly(Vector2 first, Vector2 second)
        {
            return (first.X == second.X && first.Y != second.Y) ||
                                    (first.X != second.X && first.Y == second.Y);
        }

        /// <summary>
        /// Determines if a given waypoint is on a world corner like (0,0), (0,MAX_Y), (MAX_X, 0), (MAX_X. MAX_Y)
        /// </summary>
        /// <param name="buildParams"></param>
        /// <param name="n"></param>
        /// <returns>Returns true if n is not a world corner point false otherwise</returns>
        private static bool NotOnWorldCorners(GroundNode rootNode, Vector2 n)
        {
            return !((n.X == 0 && n.Y == 0) ||
                (n.X == rootNode.EndPosition.X && n.Y == rootNode.EndPosition.Y) ||
                (n.X == rootNode.EndPosition.X && n.Y == 0) ||
                (n.X == 0 && n.Y == rootNode.EndPosition.Y)
                );
        }
    }
}
