using OpenTK;

namespace ProceduralCity.Renderer.Uniform
{
    struct Vector3Uniform : IUniformValue
    {
        public Vector3 Value
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
