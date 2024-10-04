using ProceduralCity.Utils;

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

        public static IMovement BuildRandomMovement(MovementParams buildParams, RandomService randomService)
        {
            var enumValues = Enum.GetValues(typeof(MovementType));
            var movementType = (MovementType)enumValues.GetValue(randomService.Next(enumValues.Length));
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
                case MovementType.PATH:
                    // TODO: this is a dummy implementation. Replace this with a real one

                    // This dummy implementation does the following:
                    // Select a random waypoint on the current level
                    // Get all neighbouring waypoints for the selected position
                    // Select a neigbouring waypoint
                    // Build a path with the start and the selected neighbour

                    var levelChilds = buildParams.World.BspTree.Root.Children;
                    var allPossibleWaypointsOnTheCurrentLevel = levelChilds.SelectMany(r => new[]
                    {
                        r.TopLeftCorner,
                        r.TopRightCorner,
                        r.BottomLeftCorner,
                        r.BottomRightCorner
                    })
                        .Where(n => NotOnWorldCorners(buildParams.World.BspTree.Root, n)) // Exclude the world corners (0,0), (0,MAX_Y), (MAX_X,0), (MAX_X, MAX_Y)
                        .Distinct();

                    var randomWaypoint = allPossibleWaypointsOnTheCurrentLevel.ElementAt(randomService.Next(allPossibleWaypointsOnTheCurrentLevel.Count()));

                    var nodes = buildParams.World.BspTree.Root.NodesOnPoint(randomWaypoint);
                    var nextPossibleWaypoints = nodes.SelectMany(n => (new[]
                    {
                        n.TopLeftCorner,
                        n.TopRightCorner,
                        n.BottomLeftCorner,
                        n.BottomRightCorner
                    })).Except(new[] { randomWaypoint })
                    .Where(n => // Only differ in EXACTLY one coordinate (either X or Y) essentially giving back a neighbour AND not on world corners
                    {
                        return DiffersInOneAxisOnly(n, randomWaypoint) && NotOnWorldCorners(buildParams.World.BspTree.Root, n);

                    })
                    .Distinct();
                    var randomNextWaypoint = nextPossibleWaypoints.ElementAt(randomService.Next(nextPossibleWaypoints.Count()));

                    var nn = buildParams.World.BspTree.Root.NodesOnPoint(randomNextWaypoint).ToArray();

                    // Partially completed:
                    // select 1 random out of the 9 possible
                    // Get all neighbouring waypoints for the selected position
                    // Select a neigbouring waypoint
                    // Build a path with the start and the selected neighbour
                    // -----------------------------------------------

                    // TODO: repeat steps to create a path
                    // TODO: consider all leafs of the tree when creating a path
                    // TODO: in the future: Create bezier curve with the path


                    var startPos = new Vector3(randomWaypoint.X, 10, randomWaypoint.Y);
                    var endPos = new Vector3(randomNextWaypoint.X, 10, randomNextWaypoint.Y);

                    return new PathMovement
                    {
                        Path = new[] { startPos, endPos },
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
                (n.X == 0 && n.Y == rootNode.EndPosition.Y));
        }
    }
}
