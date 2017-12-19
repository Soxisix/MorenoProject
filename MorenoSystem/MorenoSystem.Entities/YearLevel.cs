using System.Collections.Generic;

namespace MorenoSystem.Entities
{
    public class YearLevel
    {
        public YearLevel()
        {
            Students = new HashSet<Student>();
            Sections = new HashSet<Section>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
    }
}