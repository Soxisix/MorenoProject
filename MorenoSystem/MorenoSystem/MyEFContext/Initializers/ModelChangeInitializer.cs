using System;
using System.Collections.Generic;
using System.Data.Entity;
using MorenoSystem.Common.Enums;
using MorenoSystem.Entities;
using RandomNameGeneratorLibrary;

namespace MorenoSystem.MyEFContext.Initializers
{
    public class ModelChangeInitializer: DropCreateDatabaseIfModelChanges<MorenoContext>
    {
        protected override void Seed(MorenoContext context)
        {
            var personGenerator = new PersonNameGenerator();
            var placeGenerator = new PlaceNameGenerator();
            var randNum = new Random(50);


            //Student Section
            Section section = new Section()
            {
                Name = "Section 1"
            };

            Section section2 = new Section()
            {
                Name = "Section 2"
            };
            Section section3 = new Section()
            {
                Name = "Section 3"
            };
            Section section4 = new Section()
            {
                Name = "Section 4"
            };
            Section section5 = new Section()
            {
                Name = "Section 5"
            };
            Section section6 = new Section()
            {
                Name = "Section 6"
            };
            Section section7 = new Section()
            {
                Name = "Section 7"
            };
            Section section8 = new Section()
            {
                Name = "Section 8"
            };
            Section section9 = new Section()
            {
                Name = "Section 9"
            };
            Section section10 = new Section()
            {
                Name = "Section 10"
            };

            //Student Year Level
            YearLevel yearLvl = new YearLevel()

            {
                Name = "Grade 1",
                Sections = new List<Section>()
                {
                    section,
                    section2
                }
            };
            YearLevel yearLvl2 = new YearLevel()

            {
                Name = "Grade 2",
                Sections = new List<Section>
                {
                    section3,
                    section4
                },
            };
            YearLevel yearLvl3 = new YearLevel()

            {
                Name = "Grade 3",
                Sections = new List<Section>
                {
                    section5,
                    section6
                },
            };
            YearLevel yearLvl4 = new YearLevel()

            {
                Name = "Grade 4",
                Sections = new List<Section>
                {
                    section7,
                    section8
                },
            };
            YearLevel yearLvl5 = new YearLevel()

            {
                Name = "Grade 5",
                Sections = new List<Section>()
                {
                    section9,
                    section10
                },
            };

            //Book Conditions
            IList<Condition> conditions = new List<Condition>();
            conditions.Add(new Condition()
            {
                Name = "Good",
            });
            conditions.Add(new Condition()
            {
                Name = "Outdated",
            });
            conditions.Add(new Condition()
            {
                Name = "Damaged",
            });

            //Book Sources
            IList<Source> sources = new List<Source>();
            sources.Add(new Source()
            {
                Name = "Purchased",
            });
            sources.Add(new Source()
            {
                Name = "Donated",
            });

            //Student Requirements
            var requirements = new List<Requirement>()
            {
                new Requirement()
                {
                    Name = "NSO",
                },
                new Requirement()
                {
                    Name = "2 x 2 Picture",
                },
                new Requirement()
                {
                    Name = "Good Moral Certificate",
                },
            };

            IList<Student> students = new List<Student>();

            

            for (int i = 1; i < 100; i++)
            {
                var newSection = i > 51 ? section10 : section9;
                students.Add(new Student()
                    {
                        FirstName = personGenerator.GenerateRandomFirstName(),
                        MiddleName = personGenerator.GenerateRandomLastName(),
                        Gender = randNum.Next(1, 3) == 1 ? "Male" : "Female",
                        LastName = personGenerator.GenerateRandomLastName(),
                        BirthDate = DateTime.Now.Date,
                        Address = placeGenerator.GenerateRandomPlaceName(),
                        Contact = "09179496408",
                        SchoolYear = "2016-2017",
                        LRN = i.ToString(),
                        YearLevel = yearLvl5,
                        Section = newSection,
                    }
                );
            }
            for (int i = 101; i < 200; i++)
            {
                var newSection = i > 151 ? section7 : section8;
                students.Add(new Student()
                    {
                        FirstName = personGenerator.GenerateRandomFirstName(),
                        MiddleName = personGenerator.GenerateRandomLastName(),
                        Gender = randNum.Next(1, 3) == 1 ? "Male" : "Female",
                        LastName = personGenerator.GenerateRandomLastName(),
                        BirthDate = DateTime.Now.Date,
                        Address = placeGenerator.GenerateRandomPlaceName(),
                        Contact = "09179496408",
                        SchoolYear = "2016-2017",
                        LRN = i.ToString(),
                        YearLevel = yearLvl4,
                        Section = newSection,
                    }
                );
            }
            
            for (int i = 301; i < 400; i++)
            {
                var newSection = i > 351 ? section5 : section6;
                students.Add(new Student()
                    {
                        FirstName = personGenerator.GenerateRandomFirstName(),
                        MiddleName = personGenerator.GenerateRandomLastName(),
                        Gender = randNum.Next(1, 3) == 1 ? "Male" : "Female",
                        LastName = personGenerator.GenerateRandomLastName(),
                        BirthDate = DateTime.Now.Date,
                        Address = placeGenerator.GenerateRandomPlaceName(),
                        Contact = "09179496408",
                        SchoolYear = "2016-2017",
                        LRN = i.ToString(),
                        YearLevel = yearLvl3,
                        Section = newSection,
                    }
                );
            }
            for (int i = 401; i < 500; i++)
            {
                var newSection = i > 451 ? section3 : section4;
                students.Add(new Student()
                    {
                        FirstName = personGenerator.GenerateRandomFirstName(),
                        MiddleName = personGenerator.GenerateRandomLastName(),
                        Gender = randNum.Next(1, 3) == 1 ? "Male" : "Female",
                        LastName = personGenerator.GenerateRandomLastName(),
                        BirthDate = DateTime.Now.Date,
                        Address = placeGenerator.GenerateRandomPlaceName(),
                        Contact = "09179496408",
                        SchoolYear = "2016-2017",
                        LRN = i.ToString(),
                        YearLevel = yearLvl2,
                        Section = newSection,
                    }
                );
            }
            for (int i = 501; i < 600; i++)
            {
                var newSection = i > 551 ? section : section2;
                students.Add(new Student()
                    {
                        FirstName = personGenerator.GenerateRandomFirstName(),
                        MiddleName = personGenerator.GenerateRandomLastName(),
                        Gender = randNum.Next(1, 3) == 1 ? "Male" : "Female",
                        LastName = personGenerator.GenerateRandomLastName(),
                        BirthDate = DateTime.Now.Date,
                        Address = placeGenerator.GenerateRandomPlaceName(),
                        Contact = "09179496408",
                        SchoolYear = "2016-2017",
                        LRN = i.ToString(),
                        YearLevel = yearLvl,
                        Section = newSection,
                    }
                );
            }


            context.Students.AddRange(students);

            var studentUsers = new List<User>();

            foreach (var student in students)
            {
                var newUser = new User()
                {
                    UserName = student.LRN,
                    Password = $"{student.FirstName[0]}{student.MiddleName[0]}{student.LastName[0]}{student.BirthDate.Date.ToString("dMyyyy")}".ToLower(),
                    Authorization = "Student",
                    Student = student
                };
                studentUsers.Add(newUser);
            }
            context.Users.AddRange(studentUsers);

            //Teachers
            List<Teacher> teachers = new List<Teacher>();

            for (int i = 1001; i < 1011; i++)
            {
                teachers.Add(new Teacher()
                {
                    Id = i,
                    FirstName = personGenerator.GenerateRandomFirstName(),
                    MiddleName = personGenerator.GenerateRandomLastName(),
                    Gender = randNum.Next(1, 3) == 1 ? "Male" : "Female",
                    LastName = personGenerator.GenerateRandomLastName(),
                    Birthday = DateTime.Now.Date,
                    Address = placeGenerator.GenerateRandomPlaceName(),
                    ContactNo = "09179496408",
                });
            }

            context.Teachers.AddRange(teachers);

            var user2 = new List<User>();

            foreach (var teacher in teachers)
            {
                var newUser = new User()
                {
                    UserName = teacher.Id.ToString(),
                    Password = $"{teacher.FirstName[0]}{teacher.MiddleName[0]}{teacher.LastName[0]}{teacher.Birthday.Date.ToString("dMyyyy")}".ToLower(),
                    Authorization = "Teacher",
                    Student = null,
                    Teacher = teacher
                    
                };
                user2.Add(newUser);
            }

            context.Users.AddRange(user2);

            //var user1 = new List<User>();

            //foreach (var student in students)
            //{
            //    var newUser = new User()
            //    {
            //        UserName = student.LRN,
            //        Password = $"{student.FirstName[0]}{student.MiddleName[0]}{student.LastName[0]}{student.BirthDate.Date.ToString("dMyyyy")}".ToLower(),
            //        Authorization = "Student",
            //        Student = student
            //    };
            //    user1.Add(newUser);
            //}


            //User Login Details
            IList<User> user = new List<User>();
            user.Add(new User()
            {
                UserName = "library",
                Password = "library",
                Authorization = "Library",
                Student = null
            });
            user.Add(new User()
            {UserName = "admin",
                Password = "admin",
                Authorization = "Administrator",
                Student = null
            });
            user.Add(new User()
            {
                UserName = "registrar",
                Password = "registrar",
                Authorization = "Registrar",
                Student = null
            });


            //Council Positions
            IList<CouncilPosition> councilPositions = new List<CouncilPosition>
            {
                new CouncilPosition()
                {
                    Position = "President"
                },
                new CouncilPosition()
                {
                    Position = "Vice President"
                },
                new CouncilPosition()
                {
                    Position = "Secretary"
                },
                new CouncilPosition()
                {
                    Position = "Treasurer"
                },
                new CouncilPosition()
                {
                    Position = "Auditor"
                }
            };


            //Sections to List
            var sectionList = new List<Section>()
            {
                section,
                section2,
                section3,
                section4,
                section5,
                section6,
                section7,
                section8,
                section9,
                section10
            };

            context.Sections.AddRange(sectionList);


            //Grade to List
            var gradeList = new List<YearLevel>()
            {
                yearLvl,
                yearLvl2,
                yearLvl3,
                yearLvl4,
                yearLvl5,

            };

            context.YearLevels.AddRange(gradeList);

            

            for (int i = 0; i < 10; i++)
            {
                foreach (var councilPosition in councilPositions)
                {
                    var number = randNum.Next(1, 50);
                    var dateTime = new DateTime(2001 + i, 1, 1);
                    var electionHistory = new ElectionHistory()
                    {
                        Position = councilPosition.Position,
                        Name = personGenerator.GenerateRandomFirstAndLastName(),
                        DateYear = dateTime,
                        VoteCount = number
                    };
                    context.ElectionHistory.Add(electionHistory);
                }
            }

            //Book Category

            //Book
            var booklist = new List<Book>
            {
                new Book()
                {
                    Title = "Florante at Laura",
                    Author = "Francisco Balagtas ",
                    PublishedYear = DateTime.Now.Date,
                    Publisher = "Rex Books",
                    Quantity = 295,
                    AvailableQuantity = 295,
                    Category = new Category() {Name = "Romance"},
                },
                new Book()
                {
                    Title = "El Filibusterismo",
                    Author = "José Rizal ",
                    PublishedYear = DateTime.Now.Date,
                    Publisher = "National Books",
                    Quantity = 295,
                    AvailableQuantity = 295,
                    Category = new Category() {Name = "Romance"},
                },
                new Book()
                {
                    Title = "Luha ng Buwaya",
                    Author = "José Rizal ",
                    PublishedYear = DateTime.Now.Date,
                    Publisher = "Manok Books",
                    Quantity = 295,
                    AvailableQuantity = 295,
                    Category = new Category() {Name = "Fiction"},
                }
            };


            context.Books.AddRange(booklist);


            context.Students.AddRange(students);
            context.Requirements.AddRange(requirements);
            context.CouncilPositions.AddRange(councilPositions);
            context.Users.AddRange(user);
            //context.Users.AddRange(user1);
            context.Conditions.AddRange(conditions);
            context.Sources.AddRange(sources);
            base.Seed(context);
        }

    }
}