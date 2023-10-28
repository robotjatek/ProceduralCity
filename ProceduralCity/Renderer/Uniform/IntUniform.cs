namespace ProceduralCity.Renderer.Uniform
{
    struct IntUniform : IUniformValue
    {
        public int Value { get; set; }

        public readonly void Visit(int location, UniformHandler uniformHandler)
        {
            UniformHandler.Dispatch(location, this);
        }
    }
}
