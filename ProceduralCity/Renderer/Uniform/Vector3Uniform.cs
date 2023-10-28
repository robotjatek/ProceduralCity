using OpenTK.Mathematics;

namespace ProceduralCity.Renderer.Uniform
{
    struct Vector3Uniform : IUniformValue
    {
        public Vector3 Value
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
