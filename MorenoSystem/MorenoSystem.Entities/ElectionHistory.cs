using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MorenoSystem.Entities
{
    public class ElectionHistory
    {
        public int Id { get; set; }
        public string Position { get; set; }public string Name { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DateYear { get; set; }
        public int VoteCount { get; set; }}
}