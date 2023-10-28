using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Config;

namespace ProceduralCity.Generators
{
    public class GroundNode
    {
        private readonly IAppConfig _config;

        public Vector2 StartPosition { get; private set; } // Top left

        public Vector2 EndPosition { get; private set; } // Bottom right

        /// <summary>
        /// Alias for StartPosition
        /// </summary>
        public Vector2 TopLeftCorner
        {
            get
            {
                return StartPosition;
            }
        }


        /// <summary>
        /// Alias for EndPosition
        /// </summary>
        public Vector2 BottomRightCorner
        {
            get
            {
                return EndPosition;
            }
        }

        public Vector2 TopRightCorner
        {
            get
            {
                return new Vector2(EndPosition.X, StartPosition.Y);
            }
        }

        public Vector2 BottomLeftCorner
        {
            get
            {
                return new Vector2(StartPosition.X, EndPosition.Y);
            }
        }

        public List<GroundNode> Children { get; private set; } = new List<GroundNode>();

        public GroundNode(Vector2 startPosition, Vector2 endPosition, IAppConfig config)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            _config = config;
        }

        public IEnumerable<GroundNode> Split(Random random)
        {
            var verticalLength = (int)VerticalLength();
            var horizontalLength = (int)HorizontalLength();

            var verticalSliceLength = verticalLength / 3;
            var horizontalSliceLength = horizontalLength / 3;


            var verticalSplit = random.Next(verticalSliceLength, verticalSliceLength * 2);
            var horizontalSplit = random.Next(horizontalSliceLength, horizontalSliceLength * 2);

            var splitPoint = new Vector2(StartPosition.X + verticalSplit, StartPosition.Y + horizontalSplit);

            /*
             * 0 | 1
             * -----
             * 2 | 3
             */

            var children = new[]
            {
                    new GroundNode(StartPosition, splitPoint, _config),
                    new GroundNode(new Vector2(splitPoint.X,StartPosition.Y), new Vector2(EndPosition.X, splitPoint.Y), _config),
                    new GroundNode(new Vector2(StartPosition.X, splitPoint.Y), new Vector2(splitPoint.X, EndPosition.Y), _config),
                    new GroundNode(splitPoint, EndPosition, _config)
            };

            if (children.All(n => n.VerticalLength() > _config.MinVerticalBlockLength && n.HorizontalLength() > _config.MinHorizontalBlockLength))
            {
                Children.AddRange(children);
            }

            return Children;
        }

        public float VerticalLength()
        {
            return EndPosition.X - StartPosition.X;
        }

        public float HorizontalLength()
        {
            return EndPosition.Y - StartPosition.Y;
        }

        /// <summary>
        /// Returns all nodes on the current level that are on the given coordinate.
        /// Returns all colliding nodes. Eg. the "split point" of the node returns all children because this is an overlapping point that all child has.
        /// </summary>
        /// <param name="point">The coordinate where we look for a colliding node</param>
        /// <returns>An enumeration of all colliding nodes on the current level.</returns>
        public IEnumerable<GroundNode> NodesOnPoint(Vector2 point)
        {
            return Children.Where(c => c.IsInsideCurrentNode(point));
        }

        public bool IsInsideCurrentNode(Vector2 point)
        {
            return point.X >= StartPosition.X && point.Y >= StartPosition.Y
                && point.X <= EndPosition.X && point.Y <= EndPosition.Y;
        }
    }
}
