using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Config;
using Serilog;

namespace ProceduralCity.Generators
{
    public class GroundNode
    {
        private readonly IAppConfig _config;

        public Vector2 StartPosition { get; private set; }

        public Vector2 EndPosition { get; private set; }

        public List<GroundNode> Children { get; private set; } = new List<GroundNode>();

        public GroundNode(Vector2 startPosition, Vector2 endPosition, IAppConfig config)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            _config = config;
        }

        public IEnumerable<GroundNode> Split(Random random)
        {
            Log.Debug($"Splitting node ({StartPosition}, {EndPosition})");

            var verticalLength = (int)VerticalLenght();
            var horizontalLength = (int)HorizontalLenght();

            var verticalSliceLength = verticalLength / 3;
            var horizontalSliceLength = horizontalLength / 3;


            var verticalSplit = random.Next(verticalSliceLength, verticalSliceLength * 2);
            var horizontalSplit = random.Next(horizontalSliceLength, horizontalSliceLength * 2);

            var splitPoint = new Vector2(StartPosition.X + verticalSplit, StartPosition.Y + horizontalSplit);

            var children = new[]
            {
                    new GroundNode(StartPosition, splitPoint, _config),
                    new GroundNode(new Vector2(splitPoint.X,StartPosition.Y), new Vector2(EndPosition.X, splitPoint.Y), _config),
                    new GroundNode(new Vector2(StartPosition.X, splitPoint.Y), new Vector2(splitPoint.X, EndPosition.Y), _config),
                    new GroundNode(splitPoint, EndPosition, _config)
            };

            if (children.All(n => n.VerticalLenght() > _config.MinVerticalBlockLength && n.HorizontalLenght() > _config.MinHorizontalBlockLength))
            {
                Children.AddRange(children);
            }

            return Children;
        }

        public float VerticalLenght()
        {
            return EndPosition.X - StartPosition.X;
        }

        public float HorizontalLenght()
        {
            return EndPosition.Y - StartPosition.Y;
        }
    }
}
