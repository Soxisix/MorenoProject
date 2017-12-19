using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MorenoSystem.Entities
{
    public class Book
    {
        public Book()
        {
            BookSources = new HashSet<BookSource>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? PublishedYear { get; set; }
        public string Publisher { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime? DateAcquired { get; set; }
        public string SourceName { get; set; }
        public int Damaged { get; set; }
        public int Outdated { get; set; }
        public virtual Condition Condition { get; set; }
        public virtual Category Category { get; set; }
        public virtual Source Source { get; set; }
        public virtual ICollection<BookSource> BookSources { get; set; }
    }
}