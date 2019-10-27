using OpenTK.Graphics.OpenGL;

namespace ProceduralCity.Renderer.Uniform
{
    class UniformHandler
    {
        public void Dispatch(int location, IntUniform value)
        {
            GL.Uniform1(location, value.Value);
        }

        public void Dispatch(int location, FloatUniform value)
        {
            GL.Uniform1(location, value.Value);
        }
    }
}
