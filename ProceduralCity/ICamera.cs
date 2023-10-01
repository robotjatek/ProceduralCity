using OpenTK.Mathematics;

namespace ProceduralCity
{
    public interface ICamera
    {
        void MoveForward(float delta = 1.0f);

        void MoveBackward();

        void StrafeLeft();

        void StrafeRight();

        void SetHorizontal(float horizontal);

        void SetVertical(float vertical);

        Matrix4 Use();

        void SetPosition(Vector3 position);

        void LookAt(Vector3 position);
    }
}
