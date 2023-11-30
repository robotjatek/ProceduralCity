using OpenTK.Mathematics;

namespace ProceduralCity.Renderer.Uniform
{
    public readonly struct Matrix4Uniform : IUniformValue
    {
        public Matrix4 Value
        {
            get;
            init;
        }

        public readonly void Visit(int location, UniformHandler uniformHandler)
        {
            UniformHandler.Dispatch(location, this);
        }
    }
}
