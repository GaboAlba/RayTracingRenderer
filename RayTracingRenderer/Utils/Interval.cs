namespace RayTracingRenderer.Utils
{
    public class Interval
    {
        public float MinValue { get; init; }

        public float MaxValue { get; init; }

        public Interval()
        {
            // Empty default interval
            this.MinValue = float.PositiveInfinity;
            this.MaxValue = float.NegativeInfinity;
        }

        public Interval(float minValue, float maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public float GetSize()
        {
            return this.MaxValue - this.MinValue;
        }

        public bool Contains(float value)
        {
            return this.MinValue <= value && this.MaxValue >= value;
        }

        public bool Surrounds(float value)
        {
            return this.MinValue < value && this.MaxValue > value;
        }

        public float Clamp(float value)
        {
            if (value < this.MinValue)
            {
                return this.MinValue;
            }

            if (value > this.MaxValue)
            {
                return this.MaxValue;
            }

            return value;
        }
    }
}
