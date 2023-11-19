namespace ProceduralCity.Renderer.Uniform
{
    readonly struct FloatUniform : IUniformValue
    {
        public float Value
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
