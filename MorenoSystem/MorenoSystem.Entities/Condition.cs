using System.Collections.Generic;

namespace MorenoSystem.Entities
{
    public class Condition
    {
        public Condition()
        {
            Books = new HashSet<Book>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}