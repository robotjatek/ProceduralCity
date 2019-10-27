namespace ProceduralCity.Renderer.Uniform
{
    interface IUniformValue
    {
        void Visit(int location, UniformHandler uniformHandler);
    }
}
