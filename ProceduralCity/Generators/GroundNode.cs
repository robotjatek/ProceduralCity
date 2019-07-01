using System;
using System.Collections.Generic;
using OpenTK;
using Serilog;

namespace ProceduralCity.Generators
{
    class GroundNode
    {
        public Vector2 StartPosition { get; private set; }
        public Vector2 EndPosition { get; private set; }

        public List<GroundNode> Children { get; private set; } = new List<GroundNode>();

        public GroundNode(Vector2 startPosition, Vector2 endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public IEnumerable<GroundNode> Split(Random random)
        {
            Log.Debug($"Splitting node ({StartPosition}, {EndPosition})");

            var verticalSplit = random.Next(0, (int)EndPosition.X - (int)StartPosition.X);
            var horizontalSplit = random.Next(0, (int)EndPosition.Y - (int)StartPosition.Y);

            var splitPoint = new Vector2(StartPosition.X + verticalSplit, StartPosition.Y + horizontalSplit);

            var children = new[]
            {
                new GroundNode(StartPosition, splitPoint),
                new GroundNode(new Vector2(splitPoint.X,StartPosition.Y), new Vector2(EndPosition.X, splitPoint.Y)),
                new GroundNode(new Vector2(StartPosition.X, splitPoint.Y), new Vector2(splitPoint.X, EndPosition.Y)),
                new GroundNode(splitPoint, EndPosition)
            };

            Children.AddRange(children);
            return children;
        }
    }
}
