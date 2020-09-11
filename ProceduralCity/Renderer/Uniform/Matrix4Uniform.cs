using OpenTK;

namespace ProceduralCity.Renderer.Uniform
{
    struct Matrix4Uniform : IUniformValue
    {
        public Matrix4 Value
        {
            get;
            set;
        }

        public void Visit(int location, UniformHandler uniformHandler)
        {
            uniformHandler.Dispatch(location, this);
        }
    }
}
