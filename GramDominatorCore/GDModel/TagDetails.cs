namespace GramDominatorCore.GDModel
{
    public class TagDetails
    {
        public TagDetails(string id, int count, string name)
        {
            Id = id;
            Count = count;
            Name = name;
        }

        public int Count { get; }

        public string Id { get; }

        public string Name { get; }
    }
}
