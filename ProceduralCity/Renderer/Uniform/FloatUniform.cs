namespace ProceduralCity.Renderer.Uniform
{
    class FloatUniform : IUniformValue
    {
        public float Value
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
