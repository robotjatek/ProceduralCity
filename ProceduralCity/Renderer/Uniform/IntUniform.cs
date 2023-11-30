namespace ProceduralCity.Renderer.Uniform
{
    public readonly struct IntUniform : IUniformValue
    {
        public int Value { get; init; }

        public readonly void Visit(int location, UniformHandler uniformHandler)
        {
            UniformHandler.Dispatch(location, this);
        }
    }
}
