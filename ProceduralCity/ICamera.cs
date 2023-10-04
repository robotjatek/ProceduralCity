using OpenTK.Mathematics;

namespace ProceduralCity
{
    public interface ICamera
    {
        void MoveForward(float delta = 1.0f);

        void MoveBackward(float delta = 1.0f);

        void StrafeLeft(float delta = 1.0f);

        void StrafeRight(float delta = 1.0f);

        void SetHorizontal(float horizontal);

        void SetVertical(float vertical);

        Matrix4 Use();

        void SetPosition(Vector3 position);

        Vector3 GetPosition();

        void LookAt(Vector3 position);
    }
}
