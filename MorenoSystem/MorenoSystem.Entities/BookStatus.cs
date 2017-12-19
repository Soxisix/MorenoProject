using System;

namespace MorenoSystem.Entities
{
    public class BookStatus
    {
        public int Id { get; set; }
        public int QuantityBorrowed { get; set; }
        public int QuantityReturned { get; set; }
        public DateTime DateBorrowed { get; set; }
        public DateTime DateReturned { get; set; }
        public string Status { get; set; }
        public virtual Book Book { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}