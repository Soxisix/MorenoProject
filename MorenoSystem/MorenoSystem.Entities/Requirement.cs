using System.Collections.Generic;

namespace MorenoSystem.Entities
{
    public class Requirement
    {
        public Requirement()
        {
            RequirementStudents = new HashSet<RequirementStudents>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Status { get; set; }
        //[Required]
        public virtual ICollection<RequirementStudents> RequirementStudents { get; set; }
    }
}