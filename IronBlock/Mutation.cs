namespace IronBlock
{
    public class Mutation
    {
        public Mutation(string domain, string name, string value)
        {
            Domain = domain;
            Name = name;
            Value = value;
        }

        public string Domain { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}