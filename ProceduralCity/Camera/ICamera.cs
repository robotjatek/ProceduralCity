﻿using OpenTK.Mathematics;

using ProceduralCity.Utils;

namespace ProceduralCity.Camera
{
    public interface ICamera
    {
        void MoveForward(float delta);

        /// <summary>
        /// Moves the camera forward on a vertical plane without considering the vertical orientation.
        /// </summary>
        /// <param name="delta">The elapsed time since the last update</param>
        void MoveForwardOnAPlane(float delta);
        /// <summary>
        /// Move the camera backward on a vertical plan without considering the vertical orientation.
        /// </summary>
        /// <param name="delta">The elapsed time since the last update</param>
        void MoveBackwardOnAPlane(float delta);

        void MoveBackward(float delta);

        void StrafeLeft(float delta);

        void StrafeRight(float delta);

        void SetHorizontal(float horizontal, float delta);

        void SetVertical(float vertical, float delta);

        /// <summary>
        /// Set vertical angle instantly, without considering delta time or the camera speed
        /// </summary>
        /// <param name="vertical">The vertical angle of the camera</param>
        void SetVerticalInstant(float vertical);

        Vector3 Position { get; set; }


        bool IsInViewFrustum(Vector3 point);

        bool IsInViewFrustum(BoundingBox box);

        bool IsInViewFrustum(BoundingSphere sphere);

        public Matrix4 ProjectionMatrix
        {
            get; set;
        }

        public Matrix4 ViewMatrix { get; }
        
        void LookAt(Vector3 target);
    }
}
