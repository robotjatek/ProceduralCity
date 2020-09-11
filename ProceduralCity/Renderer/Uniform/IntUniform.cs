namespace ProceduralCity.Renderer.Uniform
{
    struct IntUniform : IUniformValue
    {
        public int Value { get; set; }

        public void Visit(int location, UniformHandler uniformHandler)
        {
            uniformHandler.Dispatch(location, this);
        }
    }
}
