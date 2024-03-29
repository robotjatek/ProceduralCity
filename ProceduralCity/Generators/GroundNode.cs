﻿using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Config;
using ProceduralCity.GameObjects;
using ProceduralCity.Utils;

namespace ProceduralCity.Generators
{
    public class GroundNode
    {
        private readonly BoundingBox _boundingBox;
        private readonly IAppConfig _config;
        private readonly List<TrafficLight> _trafficLights = [];

        public BoundingBox BoundingBox => _boundingBox;

        public Vector2 StartPosition { get; private set; }  // Top left

        public Vector2 EndPosition { get; private set; } // Bottom right

        public List<GroundNode> Children { get; private set; } = [];

        public List<TrafficLight> Traffic => _trafficLights;

        /// <summary>
        /// Alias for StartPosition
        /// </summary>
        public Vector2 TopLeftCorner => StartPosition;

        /// <summary>
        /// Alias for EndPosition
        /// </summary>
        public Vector2 BottomRightCorner => EndPosition;

        public Vector2 TopRightCorner => new(EndPosition.X, StartPosition.Y);

        public Vector2 BottomLeftCorner => new(StartPosition.X, EndPosition.Y);

        public GroundNode(Vector2 startPosition, Vector2 endPosition, IAppConfig config)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            _config = config;

            _boundingBox = new BoundingBox(TopLeftCorner, TopRightCorner, BottomLeftCorner, BottomRightCorner, config.MaxBuildingHeight);
        }

        public IEnumerable<GroundNode> Split(RandomService random)
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

        public void AddTrafficLights(IEnumerable<TrafficLight> lights)
        {
            _trafficLights.AddRange(lights);
        }
    }
}
