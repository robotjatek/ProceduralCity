using OpenTK.Mathematics;

namespace ProceduralCity.Renderer.Uniform
{
    readonly struct Vector3Uniform : IUniformValue
    {
        public readonly Vector3 Value
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
