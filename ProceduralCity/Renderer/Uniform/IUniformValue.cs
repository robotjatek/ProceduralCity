namespace ProceduralCity.Renderer.Uniform
{
    public interface IUniformValue
    {
        void Visit(int location, UniformHandler uniformHandler);
    }
}
