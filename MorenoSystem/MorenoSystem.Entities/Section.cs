using System.Collections.Generic;

namespace MorenoSystem.Entities
{
    public class Section
    {
        public Section()
        {
            Students = new HashSet<Student>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public virtual YearLevel YearLevel { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}