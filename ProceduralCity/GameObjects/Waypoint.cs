using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

namespace ProceduralCity.GameObjects
{
    class Waypoint
    {
        public Vector3 Position
        {
            get; set;
        }

        public Waypoint Next
        {
            get; set;
        }

        public static Waypoint CreateCircle(IEnumerable<Vector3> positions)
        {
            var waypoints = new List<Waypoint>();

            foreach (var p in positions)
            {
                waypoints.Add(new Waypoint
                {
                    Position = p
                });
            }

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                waypoints[i].Next = waypoints[i + 1];
            }

            waypoints.Last().Next = waypoints.First();

            return waypoints.First();
        }
    }
}
