namespace ProceduralCity.Utils
{
    public class Ref<T> where T : new()
    {
        public T Value { get; set; } = new T();
    }
}
