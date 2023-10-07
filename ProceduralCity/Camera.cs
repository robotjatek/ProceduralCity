using System;
using OpenTK.Mathematics;

namespace ProceduralCity
{
    class Camera : ICamera
    {
        private const int SPEED_MAGIC = 2;
        private Vector3 _position;
        private float _horizontalAngle;
        private float _verticalAngle;
        private readonly float _velocity;

        public Camera(Vector3 position, float horizontalAngle, float verticalAngle)
        {
            _position = position;
            _horizontalAngle = horizontalAngle;
            _verticalAngle = verticalAngle;
            _velocity = 30f;
        }

        public void MoveForward(float delta)
        {
            _position.X += (float)Math.Sin(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
            _position.Y -= (float)Math.Sin(_verticalAngle * Math.PI / 180.0f) * _velocity * delta;
            _position.Z -= (float)Math.Cos(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
        }

        public void MoveBackward(float delta)
        {
            _position.X -= (float)Math.Sin(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
            _position.Y += (float)Math.Sin(_verticalAngle * Math.PI / 180.0f) * _velocity * delta;
            _position.Z += (float)Math.Cos(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
        }

        public void StrafeLeft(float delta)
        {
            _position.X -= (float)Math.Cos(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
            _position.Z -= (float)Math.Sin(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
        }

        public void StrafeRight(float delta)
        {
            _position.X += (float)Math.Cos(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
            _position.Z += (float)Math.Sin(_horizontalAngle * Math.PI / 180.0f) * _velocity * delta;
        }

        public void SetHorizontal(float horizontal, float delta)
        {
            _horizontalAngle += horizontal * _velocity * SPEED_MAGIC * delta;
        }

        public void SetVertical(float vertical, float delta)
        {
            _verticalAngle += vertical * _velocity * SPEED_MAGIC * delta;
        }

        public Matrix4 Use()
        {
            var rot1 = Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(_verticalAngle));
            var rot2 = Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(_horizontalAngle));
            if (_position.Y < 0.1f)
            {
                _position.Y = 0.1f;
            }
            var translation = Matrix4.CreateTranslation(new Vector3(-_position.X, -_position.Y, -_position.Z));

            return translation * rot2 * rot1;
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        public void LookAt(Vector3 position)
        {
            Matrix4.LookAt(_position, position, Vector3.UnitY).ExtractRotation().ToEulerAngles(out Vector3 rotation);

            /*
             * NOTE: this is a little hacky way to ensure that the rotation is always correct.
             * It does the job for now, but it needs to be revised in the future.
             * Also extacting rotation from the lookat matrix seems a little wasteful. 
            */
            var direction = _position - position;
            _horizontalAngle = direction.Z < 0 ? 180 - MathHelper.RadiansToDegrees(rotation.Y) : MathHelper.RadiansToDegrees(rotation.Y);
            // TODO: implement vertical angle lookAt
        }

        public Vector3 GetPosition()
        {
            return _position;
        }
    }
}
