using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MorenoSystem.Entities
{
    public class Teacher
    {
       
            public Teacher()
            {
                Students = new HashSet<Student>();
                TeacherBorrowedBooks = new HashSet<TeacherBorrowedBook>();
                Sections = new HashSet<Section>();
            }

            public int Id { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            [Column(TypeName = "Date")]
            public DateTime Birthday { get; set; }
            public string Address { get; set; }
            public byte[] Photo { get; set; }
            public string ContactNo { get; set; }
            public virtual ICollection<Student> Students { get; set; }
            public virtual ICollection<Section> Sections { get; set; }
            public virtual ICollection<TeacherBorrowedBook> TeacherBorrowedBooks { get; set; }
            public virtual User User { get; set; }

            public string FullName
            {
                get { return $"{LastName}, {FirstName} {MiddleName}"; }
                set { }
            }

        
    }
}