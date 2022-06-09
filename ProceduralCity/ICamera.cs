using OpenTK.Mathematics;

namespace ProceduralCity
{
    public interface ICamera
    {
        void MoveForward();

        void MoveBackward();

        void StrafeLeft();

        void StrafeRight();

        void SetHorizontal(float horizontal);

        void SetVertical(float vertical);

        Matrix4 Use();
    }
}
