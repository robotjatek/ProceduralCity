using System;
using System.Collections.Generic;
using OpenTK;
using Serilog;

namespace ProceduralCity.Generators
{
    class GroundNode
    {
        private readonly Vector2 _startPosition;
        private readonly Vector2 _endPosition;

        public List<GroundNode> Children { get; private set; } = new List<GroundNode>();

        public GroundNode(Vector2 startPosition, Vector2 endPosition)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
        }

        public IEnumerable<GroundNode> Split()
        {
            Log.Debug($"Splitting node ({_startPosition}, {_endPosition})");
            var random = new Random();
            var verticalSplit = random.Next(0, (int)_endPosition.X - (int)_startPosition.X);
            var horizontalSplit = random.Next(0, (int)_endPosition.Y - (int)_startPosition.Y);

            var splitPoint = new Vector2(_startPosition.X + verticalSplit, _startPosition.Y + horizontalSplit);

            var children = new[]
            {
                new GroundNode(_startPosition, splitPoint),
                new GroundNode(new Vector2(splitPoint.X,_startPosition.Y), new Vector2(_endPosition.X, splitPoint.Y)),
                new GroundNode(new Vector2(_startPosition.X, splitPoint.Y), new Vector2(splitPoint.X, _endPosition.Y)),
                new GroundNode(splitPoint, _endPosition)
            };

            Children.AddRange(children);
            return children;
        }
    }
}
