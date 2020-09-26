namespace AudioLighting.Models
{
    public partial struct Value
    {
        public long? Integer;
        public string String;

        public static implicit operator Value(long Integer)
        {
            return new Value { Integer = Integer };
        }

        public static implicit operator Value(string String)
        {
            return new Value { String = String };
        }
    }
}
