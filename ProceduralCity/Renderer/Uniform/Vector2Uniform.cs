using OpenTK.Mathematics;

namespace ProceduralCity.Renderer.Uniform
{
    struct Vector2Uniform : IUniformValue
    {
        public Vector2 Value
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
