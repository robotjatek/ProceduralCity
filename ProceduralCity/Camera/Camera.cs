using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK.Mathematics;

using ProceduralCity.Utils;

namespace ProceduralCity.Camera
{
    public class Camera : ICamera
    {
        private float _yaw; // Horizontal angle
        private float _pitch; // Vertical angle
        private Frustum _frustum;

        private readonly float _velocity = 80f;
        private readonly float _rotationSpeed = 2f;

        public Matrix4 ViewMatrix
        {
            get
            {
                _pitch = MathHelper.Clamp(_pitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
                var view = Matrix4.LookAt(Position, Position + GetFront(), Vector3.UnitY);
                _frustum = Frustum.Create(Matrix4.Transpose(view * ProjectionMatrix)); // I should not need to transpose this result, but somehow this is needed for the correct result

                return view;
            }
        }

        public Vector3 Position { get; set; }
        public Matrix4 ProjectionMatrix { get; set; }

        public Camera(Vector3 startPosition, float startYaw, float startPitch)
        {
            Position = startPosition;
            _yaw = startYaw;
            _pitch = startPitch;

            _frustum = Frustum.Create(Matrix4.Transpose(ViewMatrix * ProjectionMatrix));
        }

        public void MoveForward(float deltaTime)
        {
            Position += GetFront() * _velocity * deltaTime;
        }

        public void MoveForwardOnAPlane(float deltaTime)
        {
            var horizontalFront = new Vector3(GetFront().X, 0, GetFront().Z).Normalized();
            Position += horizontalFront * _velocity * deltaTime;
        }

        public void MoveBackward(float deltaTime)
        {
            Position -= GetFront() * _velocity * deltaTime;
        }

        public void MoveBackwardOnAPlane(float deltaTime)
        {
            var horizontalFront = new Vector3(GetFront().X, 0, GetFront().Z).Normalized();
            Position -= horizontalFront * _velocity * deltaTime;
        }

        public void StrafeRight(float deltaTime)
        {
            Position += GetRight() * _velocity * deltaTime;
        }

        public void StrafeLeft(float deltaTime)
        {
            Position -= GetRight() * _velocity * deltaTime;
        }

        public void SetHorizontal(float horizontal, float delta)
        {
            _yaw += horizontal * _rotationSpeed * delta;
        }

        public void SetVertical(float vertical, float delta)
        {
            _pitch += vertical * _rotationSpeed * delta;
        }

        public void SetVerticalInstant(float vertical)
        {
            _pitch = vertical;
        }

        public void LookAt(Vector3 target)
        {
            var direction = (target - Position).Normalized();

            // Calculate yaw
            _yaw = MathF.Atan2(direction.Z, direction.X);

            // Calculate pitch
            _pitch = MathF.Asin(-direction.Y);

            // Ensure the pitch is within a valid range to prevent looking straight up or down
            const float maxPitch = MathF.PI / 2.0f - 0.01f;
            const float minPitch = -MathF.PI / 2.0f + 0.01f;
            _pitch = MathHelper.Clamp(_pitch, minPitch, maxPitch);
        }

        private Vector3 GetFront()
        {
            return new Vector3(
                (float)(Math.Cos(_yaw) * Math.Cos(_pitch)),
                (float)Math.Sin(_pitch),
                (float)(Math.Sin(_yaw) * Math.Cos(_pitch))
            );
        }

        private Vector3 GetRight()
        {
            return Vector3.Cross(GetFront(), Vector3.UnitY).Normalized();
        }

        public bool IsInViewFrustum(Vector3 point)
        {
            return _frustum.IsInViewFrustum(point);
        }

        public bool IsInViewFrustum(BoundingBox box)
        {
            return _frustum.IsInViewFrustum(box);
        }

        public bool IsInViewFrustum(BoundingSphere sphere)
        {
            return _frustum.IsInViewFrustum(sphere);
        }

        public class Frustum
        {
            public required Vector4 LeftPlane { get; init; }
            public required Vector4 RightPlane { get; init; }
            public required Vector4 TopPlane { get; init; }
            public required Vector4 BottomPlane { get; init; }
            public required Vector4 NearPlane { get; init; }
            public required Vector4 FarPlane { get; init; }

            private Frustum() { }

            public static Frustum Create(Matrix4 matrix)
            {
                var row0 = matrix.Row0;
                var row1 = matrix.Row1;
                var row2 = matrix.Row2;
                var row3 = matrix.Row3;

                var frustum = new Frustum
                {
                    LeftPlane = row3 + row0,
                    RightPlane = row3 - row0,
                    BottomPlane = row3 + row1,
                    TopPlane = row3 - row1,
                    NearPlane = row3 + row2,
                    FarPlane = row3 - row2,
                };
                frustum.NormalizePlanes();

                return frustum;
            }

            public bool IsInViewFrustum(Vector3 point)
            {
                // Dot product is positive if the point is on the same side of the planes normal vector...
                // If all dot products for all the planes are positive, then the point is inside the frustum

                if (PointPlaneDotProduct(point, LeftPlane) > 0 &&
                    PointPlaneDotProduct(point, RightPlane) > 0 &&
                    PointPlaneDotProduct(point, TopPlane) > 0 &&
                    PointPlaneDotProduct(point, BottomPlane) > 0 &&
                    PointPlaneDotProduct(point, FarPlane) > 0 &&
                    PointPlaneDotProduct(point, NearPlane) > 0)
                {
                    return true;
                }

                return false;
            }

            public bool IsInViewFrustum(BoundingBox box)
            {
                // Early discard based on a sphere collision
                var sphere = box.BoundingSphere;
                if (!IsInViewFrustum(sphere))
                    return false;

                // If any of the corners is inside the camera the box considered visible. Fast path for culling test
                if (box.Corners.Any(IsInViewFrustum))
                    return true;

                // If the box intersects with at least one plane, it is considered visible
                return !GetPlanes().Any(plane => box.Corners.All(corner => PointPlaneDotProduct(corner, plane) < 0));

                /*
                * This code does the same as the linq query in the last return statement. I leave it here for clarity reasons:
                *foreach (var plane in GetPlanes())
                *{
                *    var outside = box.Corners.All(corner => PointPlaneDistance(corner, plane) < 0);
                *    if (outside)
                *        return false;
                *}
                * //The box intersects with at least one plane
                * return true;
                */
            }

            public bool IsInViewFrustum(BoundingSphere sphere)
            {
                foreach (var plane in GetPlanes())
                {
                    var dotProduct = PointPlaneDotProduct(sphere.Center, plane);

                    // If the sphere is completely behind the plane, it's outside
                    if (dotProduct < -sphere.Radius)
                        return false;

                    // If the sphere is intersecting with the plane, it's still considered inside
                    if (Math.Abs(dotProduct) < sphere.Radius)
                        return true;
                }

                // The sphere intersects with all planes or is inside the frustum
                return true;
            }

            private static float PointPlaneDotProduct(Vector3 point, Vector4 plane)
            {
                return point.X * plane.X + point.Y * plane.Y + point.Z * plane.Z + plane.W;
            }

            private IEnumerable<Vector4> GetPlanes()
            {
                yield return LeftPlane;
                yield return RightPlane;
                yield return TopPlane;
                yield return BottomPlane;
                yield return NearPlane;
                yield return FarPlane;
            }

            private void NormalizePlanes()
            {
                LeftPlane.Normalize();
                RightPlane.Normalize();
                TopPlane.Normalize();
                BottomPlane.Normalize();
            }
        }
    }
}
