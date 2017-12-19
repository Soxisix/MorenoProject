namespace MorenoSystem.Entities
{
    public class BookSource
    {
        public int Id { get; set; }
        public string SourceType { get; set; }
        public string SourceName { get; set; }
        public int Quantity { get; set; }
        public virtual Book Book { get; set; }
    }
}