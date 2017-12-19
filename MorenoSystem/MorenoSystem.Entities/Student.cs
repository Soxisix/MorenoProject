using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MorenoSystem.Entities
{
    public class Student
    {
        public Student()
        {
            RequirementStudents = new HashSet<RequirementStudents>();
            //if (String.IsNullOrEmpty(User?.UserName)
            //    && String.IsNullOrEmpty(User.Password)
            //    && String.IsNullOrEmpty(User.Authorization))
            //{
            //    var user = new User()
            //    {
            //        UserName = LRN,
            //        Password = $"{FirstName[0]}{MiddleName[0]}{LastName[0]}{BirthDate.Date}",
            //        Authorization = "Student",
            //        Student = this
            //    };
            //}
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public string LastName { get; set; }

        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }

        public string Address { get; set; }
        public string Contact { get; set; }
        public string SchoolYear { get; set; }
        public string LRN { get; set; }
        public byte[] Photo { get; set; }
        public virtual ICollection<RequirementStudents> RequirementStudents { get; set; }
        public virtual YearLevel YearLevel { get; set; }
        public virtual Section Section { get; set; }

        public virtual User User { get; set; }

        public string FullName
        {
            get { return $"{LastName}, {FirstName} {MiddleName}"; }
        }}
}