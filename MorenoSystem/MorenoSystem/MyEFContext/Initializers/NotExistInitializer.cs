using System;
using System.Collections.Generic;
using System.Data.Entity;
using MorenoSystem.Entities;
using RandomNameGeneratorLibrary;

namespace MorenoSystem.MyEFContext.Initializers
{
    public class NotExistInitializer: CreateDatabaseIfNotExists<MorenoContext>
    {
        protected override void Seed(MorenoContext context)
        {
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

            students.Add(new Student()
            {
                FirstName = "Carl",
                MiddleName = "Bacuno",
                Gender = "Male",
                LastName = "Baisa",
                BirthDate = DateTime.Now,
                Address = "Daet",
                Contact = "091212121243421",
                SchoolYear = "2016-2017",
                LRN = "201712121",
                YearLevel = yearLvl,
                Section = section,
            });

            students.Add(new Student()
            {
                FirstName = "Carlo",
                MiddleName = "Abnoy",
                Gender = "Female",
                LastName = "Ubana",
                BirthDate = DateTime.Now,
                Address = "Lag-on",
                Contact = "09121212125653",
                SchoolYear = "2016-2017",
                LRN = "201712122",
                YearLevel = yearLvl2,
                Section = section2

            });

            students.Add(new Student()
            {
                FirstName = "Marlsen",
                MiddleName = "Bacalla",
                Gender = "Male",
                LastName = "Abando",
                BirthDate = DateTime.Now,
                Address = "Hotape",
                Contact = "09121212123421",
                SchoolYear = "2016-2017",
                LRN = "201712123",
                YearLevel = yearLvl3,
                Section = section3
            });

            students.Add(new Student()
            {
                FirstName = "Nikko",
                MiddleName = "Ace",
                Gender = "Female",
                LastName = "Moreno",
                BirthDate = DateTime.Now,
                Address = "Harmony Village",
                Contact = "09121213432121",
                SchoolYear = "2016-2017",
                LRN = "201712124",
                YearLevel = yearLvl4,
                Section = section4
            });
            students.Add(new Student()
            {
                FirstName = "Arminna",
                MiddleName = "Vasquez",
                Gender = "Female",
                LastName = "Jaucian",
                BirthDate = DateTime.Now,
                Address = "Pandan",
                Contact = "09121214552121",
                SchoolYear = "2016-2017",
                LRN = "201712125",
                YearLevel = yearLvl5,
                Section = section5
            });
            students.Add(new Student()
            {
                FirstName = "Carlo",
                MiddleName = "Asor",
                Gender = "Male",
                LastName = "Ubana",
                BirthDate = DateTime.Now,
                Address = "lagon",
                Contact = "09121218972331",
                SchoolYear = "2016-2017",
                LRN = "1",
                YearLevel = yearLvl5,
                Section = section5
            });
            students.Add(new Student()
            {
                FirstName = "Jabo",
                MiddleName = "Santos",
                Gender = "Male",
                LastName = "Santino",
                BirthDate = DateTime.Now,
                Address = "basud",
                Contact = "09121256713211",
                SchoolYear = "2016-2017",
                LRN = "2",
                YearLevel = yearLvl5,
                Section = section5
            });
            students.Add(new Student()
            {
                FirstName = "Alexandra",
                MiddleName = "Amanda",
                Gender = "Female",
                LastName = "Carlo",
                BirthDate = DateTime.Now,
                Address = "Amy",
                Contact = "0912329532211",
                SchoolYear = "2016-2017",
                LRN = "3",
                YearLevel = yearLvl5,
                Section = section5
            });
            students.Add(new Student()
                {
                    FirstName = "Xandra",
                    MiddleName = "Donna",
                    Gender = "Female",
                    LastName = "Emily",
                    BirthDate = DateTime.Now,
                    Address = "Amty",
                    Contact = "0921398731313",
                    SchoolYear = "2016-2017",
                    LRN = "4",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Rebecca",
                    MiddleName = "Donna",
                    Gender = "Female",
                    LastName = "Don",
                    BirthDate = DateTime.Now,
                    Address = "Amgy",
                    Contact = "09213313123434",
                    SchoolYear = "2016-2017",
                    LRN = "5",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Yvonne",
                    MiddleName = "Donnat",
                    Gender = "Female",
                    LastName = "Done",
                    BirthDate = DateTime.Now,
                    Address = "Amqy",
                    Contact = "0921467331334",
                    SchoolYear = "2016-2017",
                    LRN = "6",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Frank",
                    MiddleName = "Francus",
                    Gender = "Female",
                    LastName = "Donet",
                    BirthDate = DateTime.Now,
                    Address = "Amey",
                    Contact = "092561331334",
                    SchoolYear = "2016-2017",
                    LRN = "7",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Lucas",
                    MiddleName = "Francis",
                    Gender = "Male",
                    LastName = "Donet",
                    BirthDate = DateTime.Now,
                    Address = "Amys",
                    Contact = "0921332341334",
                    SchoolYear = "2016-2017",
                    LRN = "7",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Paul",
                    MiddleName = "Frans",
                    Gender = "Male",
                    LastName = "Donett",
                    BirthDate = DateTime.Now,
                    Address = "Amys",
                    Contact = "092231331334",
                    SchoolYear = "2016-2017",
                    LRN = "8",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );    
            students.Add(new Student()
                {
                    FirstName = "William",
                    MiddleName = "Franst",
                    Gender = "Male",
                    LastName = "Donetgt",
                    BirthDate = DateTime.Now,
                    Address = "Amyyr",
                    Contact = "0921334551334",
                    SchoolYear = "2016-2017",
                    LRN = "9",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Mark",
                    MiddleName = "Fransts",
                    Gender = "Male",
                    LastName = "Dontrt",
                    BirthDate = DateTime.Now,
                    Address = "Amnnyr",
                    Contact = "092123234424",
                    SchoolYear = "2016-2017",
                    LRN = "10",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Evan",
                    MiddleName = "Evans",
                    Gender = "Male",
                    LastName = "Deb",
                    BirthDate = DateTime.Now,
                    Address = "Ambwyr",
                    Contact = "0921233233432",
                    SchoolYear = "2016-2017",
                    LRN = "11",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Robert",
                    MiddleName = "Roberts",
                    Gender = "Male",
                    LastName = "Debe",
                    BirthDate = DateTime.Now,
                    Address = "Ambhjr",
                    Contact = "09212434432",
                    SchoolYear = "2016-2017",
                    LRN = "12",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Trevor",
                    MiddleName = "Trevorr",
                    Gender = "Male",
                    LastName = "Debpe",
                    BirthDate = DateTime.Now,
                    Address = "Trevff",
                    Contact = "09212477432",
                    SchoolYear = "2016-2017",
                    LRN = "13",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Jason",
                    MiddleName = "Jasons",
                    Gender = "Male",
                    LastName = "Debpev",
                    BirthDate = DateTime.Now,
                    Address = "Tareff",
                    Contact = "09267124432",
                    SchoolYear = "2016-2017",
                    LRN = "14",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Christian",
                    MiddleName = "Christians",
                    Gender = "Male",
                    LastName = "Debperv",
                    BirthDate = DateTime.Now,
                    Address = "lopps",
                    Contact = "092123124432",
                    SchoolYear = "2016-2017",
                    LRN = "15",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Alexander",
                    MiddleName = "Alexanderaer",
                    Gender = "Male",
                    LastName = "mall",
                    BirthDate = DateTime.Now,
                    Address = "lorpps",
                    Contact = "09212455432",
                    SchoolYear = "2016-2017",
                    LRN = "16",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "alans",
                    MiddleName = "alanse",
                    Gender = "Male",
                    LastName = "mall",
                    BirthDate = DateTime.Now,
                    Address = "malls",
                    Contact = "09244124432",
                    SchoolYear = "2016-2017",
                    LRN = "17",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
            {
                FirstName = "Dans",
                MiddleName = "dan",
                Gender = "Male",
                LastName = "mallsa",
                BirthDate = DateTime.Now,
                Address = "mekks",
                Contact = "09212441332",
                SchoolYear = "2016-2017",
                LRN = "18",
                YearLevel = yearLvl5,
                Section = section5,
            }
           );
            students.Add(new Student()
                {
                    FirstName = "Lucas",
                    MiddleName = "Lucasd",
                    Gender = "Male",
                    LastName = "Lucasr",
                    BirthDate = DateTime.Now,
                    Address = "mekksd",
                    Contact = "0921243432",
                    SchoolYear = "2016-2017",
                    LRN = "19",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Richard",
                    MiddleName = "Richards",
                    Gender = "Male",
                    LastName = "Richardss",
                    BirthDate = DateTime.Now,
                    Address = "lagns",
                    Contact = "09212432432",
                    SchoolYear = "2016-2017",
                    LRN = "20",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Ryan",
                    MiddleName = "Ryans",
                    Gender = "Male",
                    LastName = "Richardss",
                    BirthDate = DateTime.Now,
                    Address = "wqwe",
                    Contact = "09212432",
                    SchoolYear = "2016-2017",
                    LRN = "21",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Ryan",
                    MiddleName = "Ryans",
                    Gender = "Male",
                    LastName = "Richardss",
                    BirthDate = DateTime.Now,
                    Address = "wqwe",
                    Contact = "09212432",
                    SchoolYear = "2016-2017",
                    LRN = "21",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Victor",
                    MiddleName = "Victors",
                    Gender = "Male",
                    LastName = "Victorsas",
                    BirthDate = DateTime.Now,
                    Address = "frg",
                    Contact = "0921242232",
                    SchoolYear = "2016-2017",
                    LRN = "22",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Dominic",
                    MiddleName = "Dominicsa",
                    Gender = "Male",
                    LastName = "Dominicgh",
                    BirthDate = DateTime.Now,
                    Address = "ertg",
                    Contact = "09123521",
                    SchoolYear = "2016-2017",
                    LRN = "23",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Connor",
                    MiddleName = "Connorsq",
                    Gender = "Male",
                    LastName = "DomiConnordfsnicgh",
                    BirthDate = DateTime.Now,
                    Address = "3tye",
                    Contact = "0912352221",
                    SchoolYear = "2016-2017",
                    LRN = "24",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Austin",
                    MiddleName = "Austinsa",
                    Gender = "Male",
                    LastName = "Austinswq",
                    BirthDate = DateTime.Now,
                    Address = "qwerty",
                    Contact = "09123522321",
                    SchoolYear = "2016-2017",
                    LRN = "25",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Andrew",
                    MiddleName = "Andrews",
                    Gender = "Male",
                    LastName = "Andrewqwe",
                    BirthDate = DateTime.Now,
                    Address = "qwertys",
                    Contact = "02342412",
                    SchoolYear = "2016-2017",
                    LRN = "26",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Cameron",
                    MiddleName = "Camerons",
                    Gender = "Male",
                    LastName = "Cameronhyh",
                    BirthDate = DateTime.Now,
                    Address = "qwertysr",
                    Contact = "09232342412",
                    SchoolYear = "2016-2017",
                    LRN = "27",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Isaac",
                    MiddleName = "Isaacs",
                    Gender = "Male",
                    LastName = "Isaacjr",
                    BirthDate = DateTime.Now,
                    Address = "qwertyssr",
                    Contact = "093232342412",
                    SchoolYear = "2016-2017",
                    LRN = "28",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Isaac",
                    MiddleName = "Isaacs",
                    Gender = "Male",
                    LastName = "Isaacjr",
                    BirthDate = DateTime.Now,
                    Address = "qwertyssr",
                    Contact = "093232342412",
                    SchoolYear = "2016-2017",
                    LRN = "28",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Matt",
                    MiddleName = "Matts",
                    Gender = "Male",
                    LastName = "Mattss",
                    BirthDate = DateTime.Now,
                    Address = "qwertyqr",
                    Contact = "083232342412",
                    SchoolYear = "2016-2017",
                    LRN = "29",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            students.Add(new Student()
                {
                    FirstName = "Michael",
                    MiddleName = "Michaelsa",
                    Gender = "Male",
                    LastName = "Michaelfd",
                    BirthDate = DateTime.Now,
                    Address = "qwerty3qr",
                    Contact = "0832323142412",
                    SchoolYear = "2016-2017",
                    LRN = "30",
                    YearLevel = yearLvl5,
                    Section = section5,
                }
            );
            context.Students.AddRange(students);
            var user1 = new List<User>();

            foreach (var student in students)
            {
                var newUser = new User()
                {
                    UserName = student.LRN,
                    Password = $"{student.FirstName[0]}{student.MiddleName[0]}{student.LastName[0]}{student.BirthDate.Date.ToString("dMyyyy")}".ToLower(),
                    Authorization = "Student",
                    Student = student
                };
                user1.Add(newUser);
            }
            //User Login Details
            IList<User> user = new List<User>();
            user.Add(new User()
            {
                UserName = "library",
                Password = "library",
                Authorization = "Library"
            });
            user.Add(new User()
            {
                UserName = "c",
                Password = "c",
                Authorization = "Administrator"
            });
            user.Add(new User()
            {
                UserName = "a",
                Password = "a",
                Authorization = "Registrar"
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

            var personGenerator = new PersonNameGenerator();
            var placeGenerator = new PlaceNameGenerator();
            var randNum = new Random(50);
            
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


            context.Requirements.AddRange(requirements);
            context.CouncilPositions.AddRange(councilPositions);
            context.Users.AddRange(user);
            context.Users.AddRange(user1);
            context.Conditions.AddRange(conditions);
            context.Sources.AddRange(sources);
            base.Seed(context);
        }

    }
}