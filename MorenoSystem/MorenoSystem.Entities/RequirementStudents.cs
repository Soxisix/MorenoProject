using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MorenoSystem.Entities
{
    public class RequirementStudents
    {
        [Key, Column(Order = 0)]
        public int RequirementId { get; set; }
        [Key, Column(Order = 1)]
        public int StudentId { get; set; }
        public bool Status { get; set; }

        public virtual Student Student { get; set; }
        public virtual Requirement Requirement { get; set; }}
}