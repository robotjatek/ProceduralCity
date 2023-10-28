using OpenTK.Graphics.OpenGL;

namespace ProceduralCity.Renderer.Uniform
{
    class UniformHandler
    {
        public static void Dispatch(int location, IntUniform value)
        {
            GL.Uniform1(location, value.Value);
        }

        public static void Dispatch(int location, FloatUniform value)
        {
            GL.Uniform1(location, value.Value);
        }

        public static void Dispatch(int location, Matrix4Uniform value)
        {
            var matrix = value.Value;
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public static void Dispatch(int location, Vector2Uniform value)
        {
            GL.Uniform2(location, value.Value);
        }

        public static void Dispatch(int location, Vector3Uniform value)
        {
            GL.Uniform3(location, value.Value);
        }
    }
}
