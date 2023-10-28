namespace ProceduralCity.Renderer.Uniform
{
    struct FloatUniform : IUniformValue
    {
        public float Value
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
