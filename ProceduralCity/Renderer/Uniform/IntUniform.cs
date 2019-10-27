namespace ProceduralCity.Renderer.Uniform
{
    class IntUniform : IUniformValue
    {
        public int Value { get; set; }

        public void Visit(int location, UniformHandler uniformHandler)
        {
            uniformHandler.Dispatch(location, this);
        }
    }
}
