namespace ProceduralCity.Utils
{
    class Ref<T> where T : new()
    {
        public T Value { get; set; } = new T();
    }
}
