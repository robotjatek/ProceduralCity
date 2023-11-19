using OpenTK.Mathematics;

namespace ProceduralCity.Renderer.Uniform
{
    readonly struct Vector2Uniform : IUniformValue
    {
        public Vector2 Value
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
