using OpenTK.Mathematics;

namespace ProceduralCity.Renderer.Uniform
{
    struct Matrix4Uniform : IUniformValue
    {
        public Matrix4 Value
        {
            get;
            set;
        }

        public readonly void Visit(int location, UniformHandler uniformHandler)
        {
            UniformHandler.Dispatch(location, this);
        }
    }
}
