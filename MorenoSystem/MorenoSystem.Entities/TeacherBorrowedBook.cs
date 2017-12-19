using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MorenoSystem.Entities
{
    public class TeacherBorrowedBook
    {
        [Key, Column(Order = 0)]
        public int TeacherId { get; set; }
        [Key, Column(Order = 1)]
        public int BookId { get; set; }

        public int QuantityBorrowed { get; set; }
        public int QuantityReturned { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DateBorrowed { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DateReturned { get; set; }
        public string Status { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Book Book { get; set; }
    }
}