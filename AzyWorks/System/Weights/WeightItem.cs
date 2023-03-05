namespace AzyWorks.System.Weights
{
    public class WeightItem<T> 
    {
        public int Weight { get; set; }

        public T Value { get; }

        public WeightItem(int weight, T value)
        {
            Weight = weight;
            Value = value;
        }
    }
}