using OpenTK.Mathematics;

namespace ProceduralCity.Camera
{
    public interface ICamera
    {
        void MoveForward(float delta);

        void MoveBackward(float delta);

        void StrafeLeft(float delta);

        void StrafeRight(float delta);

        void SetHorizontal(float horizontal, float delta);

        void SetVertical(float vertical, float delta);

        Matrix4 Use();

        void SetPosition(Vector3 position);

        Vector3 GetPosition();

        void LookAt(Vector3 position);
    }
}
