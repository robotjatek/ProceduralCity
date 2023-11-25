using OpenTK.Mathematics;

namespace ProceduralCity.Utils
{
    public class BoundingBox
    {
        private readonly Vector3[] _corners;
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        public Vector3[] Corners => _corners;

        public BoundingSphere BoundingSphere { get; private set; }

        public BoundingBox(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, float height)
        {
            var topLeft3D = new Vector3(topLeft.X, 0, topLeft.Y);
            var topRight3D = new Vector3(topRight.X, 0, topRight.Y);
            var bottomLeft3D = new Vector3(bottomLeft.X, 0, bottomLeft.Y);
            var bottomRight3D = new Vector3(bottomRight.X, 0, bottomRight.Y);

            Min = Vector3.ComponentMin(topLeft3D, Vector3.ComponentMin(topRight3D, Vector3.ComponentMin(bottomLeft3D, bottomRight3D)));
            Max = new Vector3(Min.X, height, Min.Z) +
                  Vector3.ComponentMax(topLeft3D, Vector3.ComponentMax(topRight3D, Vector3.ComponentMax(bottomLeft3D, bottomRight3D))) - Min;

            _corners =
            [
                new Vector3(Min.X, Min.Y, Min.Z), // 0: Min
                new Vector3(Max.X, Min.Y, Min.Z), // 1
                new Vector3(Min.X, Max.Y, Min.Z), // 2
                new Vector3(Max.X, Max.Y, Min.Z), // 3
                new Vector3(Min.X, Min.Y, Max.Z), // 4
                new Vector3(Max.X, Min.Y, Max.Z), // 5
                new Vector3(Min.X, Max.Y, Max.Z), // 6
                new Vector3(Max.X, Max.Y, Max.Z)  // 7: Max
            ];

            BoundingSphere = CalculateBoundingSphere(this);
        }

        private static BoundingSphere CalculateBoundingSphere(BoundingBox box)
        {
            var center = (box.Min + box.Max) / 2.0f;
            var radius = Vector3.Distance(center, box.Max);

            return new BoundingSphere
            {
                Center = center,
                Radius = radius,
            };
        }
    }
}
