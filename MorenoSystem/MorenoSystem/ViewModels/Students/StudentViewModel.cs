using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.XtraEditors.DXErrorProvider;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Common.Enums;
using MorenoSystem.Common.Validator;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;
using MorenoSystem.Views.Student;
using MorenoSystem.Views.Student.StudentList;
using MvvmValidation;

namespace MorenoSystem.ViewModels.Students
{
    public class StudentViewModel : ValidatableViewModelBase
    {
        #region StudentListView Members


        private List<Student> _allStudent;
        private bool _isOkMessageOpen;
        private MorenoContext _context;

        public StudentViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadStudents();

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
        }

        private void LoadStudents()
        {
            //    using (var context = new MorenoContext())
            //    {
            //        _allStudent = context.Students.ToList();

            //        StudentList = _allStudent.ToObservableCollection();
            //    }
            _allStudent = _context.Students.ToList();

            StudentList = _allStudent.ToObservableCollection();

        }



        public ObservableCollection<Student> StudentList
        {
            get { return GetProperty(() => StudentList); }
            set { SetProperty(() => StudentList, value); }
        }

        public Student SelectedStudent
        {
            get { return GetProperty(() => SelectedStudent); }
            set { SetProperty(() => SelectedStudent, value); }
        }

        public string SearchStudent
        {
            get { return GetProperty(() => SearchStudent); }
            set
            {
                SetProperty(() => SearchStudent, value);
                int numberId = 0;
                bool result = false;
                result = Int32.TryParse(SearchStudent, out numberId);

                if (result)
                {
                    StudentList = _allStudent.Where(c => c.Id == numberId).ToObservableCollection();
                }
                else if (SearchStudent is string && !string.IsNullOrEmpty(SearchStudent))
                {
                    StudentList = _allStudent
                        .Where(c => c.FirstName.ToLower().Contains(SearchStudent.ToLower()) || c.MiddleName.ToLower().Contains(SearchStudent.ToLower()) ||
                                    c.LastName.ToLower().Contains(SearchStudent.ToLower())).ToObservableCollection();
                }
                else if (string.IsNullOrEmpty(SearchStudent))
                {
                    StudentList = _allStudent.ToObservableCollection();
                }
            }
        }

        public DelegateCommand ViewStudentCommand => new DelegateCommand(ViewStudent);

        private async void ViewStudent()
        {
            if (SelectedStudent == null)
            {
                await DialogHost.Show(new OkMessageDialog() { DataContext = "Please select a student!" });
                return;
            }
            await DialogHost.Show(new PleaseWaitView(), "RootDialog",
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    Task.Run(() =>
                    {
                        //Requirements = _context.RequirementStudents.AsNoTracking().Where(c => c.StudentId == SelectedStudent.Id)
                        //    .ToList();
                        AddRequirementsToStudent(SelectedStudent);
                        Requirements = _context.RequirementStudents.AsNoTracking()
                            .Where(c => c.StudentId == SelectedStudent.Id)
                            .ToList();

                    }).ContinueWith((t, _) =>
                    {
                        //using (var context = new MorenoContext())
                        //{
                        //LoadStudents();
                        //Requirements = SelectedStudent.RequirementStudents.ToList();
                        //Requirements = context.RequirementStudents.Where(c => c.Student.Id == SelectedStudent.Id)
                        //    .ToList();
                        //}
                        args.Session.UpdateContent(new StudentView() { DataContext = this });
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
                });
            

            //Requirements = _context.RequirementStudents.AsNoTracking().Where(c => c.StudentId == SelectedStudent.Id)
            //    .ToList();
            //AddRequirementsToStudentAsync(SelectedStudent);
            //await DialogHost.Show(new StudentView() {DataContext = this});
            
        }

        #endregion

        #region StudentView

        public List<RequirementStudents> Requirements
        {
            get { return GetProperty(() => Requirements); }
            set { SetProperty(() => Requirements, value); }
        }

        public RequirementStudents SelectedRequirement
        {
            get { return GetProperty(() => SelectedRequirement); }
            set { SetProperty(() => SelectedRequirement, value); }
        }

        public void AddRequirementsToStudent(Student student)
        {
            //var listOfId = _context.RequirementStudents
            //    .Where(c => c.StudentId == student.Id)
            //    .Select(c => c.RequirementId);
            //var listRequirement = _context.Requirements
            //    .Where(c => !listOfId.Contains(c.Id));
            //if (!listRequirement.Any())
            //{
            //    return;
            //}
            //foreach (var req in listRequirement)
            //{
            //    var newReq = new RequirementStudents
            //    {
            //        RequirementId = req.Id,
            //        StudentId = student.Id,
            //        Status = false,
            //    };
            //    _context.RequirementStudents.Add(newReq);
            //}
            //_context.SaveChangesAsync();
            using (var context = new MorenoContext())
            {
                var listOfId = context.RequirementStudents
                    .Where(c => c.StudentId == student.Id)
                    .Select(c => c.RequirementId);
                var listRequirement = context.Requirements
                    .Where(c => !listOfId.Contains(c.Id));
                if (!listRequirement.Any())
                {
                    return;
                }
                foreach (var req in listRequirement)
                {
                    var newReq = new RequirementStudents
                    {
                        RequirementId = req.Id,
                        StudentId = student.Id,
                        Status = false,
                    };
                    context.RequirementStudents.Add(newReq);
                }
                context.SaveChangesAsync();
            }
        }

        #endregion

        #region StudentEditView

        private List<RequirementStudents> _oldRequirementStudents = new List<RequirementStudents>();

        public DelegateCommand ViewStudentEditCommand => new DelegateCommand(ViewStudentEdit);

        private async void ViewStudentEdit()
        {
            if (SelectedStudent == null)
            {
                await DialogHost.Show(new OkMessageDialog() { DataContext = "Please select a student!" });
                return;
            }
            await DialogHost.Show(new PleaseWaitView(), "RootDialog",
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    


                        Task.Run(() =>
                        {

                            //_oldRequirementStudents = Requirements;
                            AddRequirementsToStudent(SelectedStudent);
                            //Requirements = _context.RequirementStudents.AsNoTracking()
                            //    .Where(c => c.StudentId == SelectedStudent.Id)
                            //    .ToList();
                            Requirements = _context.RequirementStudents
                                .Where(c => c.StudentId == SelectedStudent.Id)
                                .ToList();
                            Thread.Sleep(1000);
                        }).ContinueWith((t, _) =>
                        {
                            //Requirements = SelectedStudent.RequirementStudents.ToList();
                            _oldRequirementStudents = Requirements;

                            SelectedYearLevel = SelectedStudent.YearLevel;
                            SelectedSection = SelectedStudent.Section;
                            YearLevels = _context.YearLevels.ToObservableCollection();
                            GenderList = Enum.GetNames(typeof(EnumGender)).ToList();

                            args.Session.UpdateContent(new StudentEditView() {DataContext = this});
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    
                }, delegate(object sender, DialogClosingEventArgs args)
                {
                    bool result = false;
                    if (Equals(args.Parameter, false))
                    {
                        return;
                    }
                    if (Equals(args.Parameter, "Close"))
                    {
                        try
                        {
                            _context.Entry(SelectedStudent).CurrentValues.SetValues(_context.Entry(SelectedStudent).OriginalValues);
                            foreach (var requirement in Requirements)
                            {
                                _context.Entry(requirement).CurrentValues.SetValues(_context.Entry(requirement).OriginalValues);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        Requirements = _oldRequirementStudents;
                        return;
                    }
                    if (_isOkMessageOpen && Equals(args.Parameter, "Cancel"))
                    {
                        _isOkMessageOpen = false;
                        args.Cancel();
                        args.Session.UpdateContent(_content);
                    }
                    if (Equals(args.Parameter, "Update"))
                    {
                        args.Cancel();
                        args.Session.UpdateContent(new PleaseWaitView());
                        Task.Run(() =>
                        {
                            Student student = null;
                            try
                            {
                                //using (var context = new MorenoContext())
                                //{
                                //    foreach (var item in Requirements)
                                //    {
                                //        context.Entry(item).State = EntityState.Modified;
                                //    }
                                //    context.SaveChanges();
                                //}
                                ////foreach (var requirement in Requirements)
                                ////{
                                ////    //_context.Entry(requirement).State = EntityState.Modified;
                                ////    _context.RequirementStudents.Attach(requirement);
                                ////}
                                ////_context.SaveChanges();
                                SelectedStudent.YearLevel = SelectedYearLevel;
                                SelectedStudent.Section = SelectedSection;
                                student = StudentList.FirstOrDefault(c => c == SelectedStudent);

                                _context.SaveChanges();
                                result = true;
                            }

                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                result = false;
                            }
                            return student; 
                        }).ContinueWith((t, _) =>
                        {
                            if (t.Result != null) t.Result.RequirementStudents = Requirements;
                            LoadStudents();
                            var message = result ? "Update Successfull" : "Update Failed";
                            if (result)
                            {
                                LoadStudents();
                            }
                            
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = message });
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    if (Equals(args.Parameter, "Delete"))
                    {
                        _content = args.Session.Content;
                        args.Cancel();
                        _isOkMessageOpen = true;
                        args.Session.UpdateContent(new OkCancelMessageDialog() { DataContext = $"Are you sure you want to delete {SelectedStudent.FullName}?" });
                    }
                    if (Equals(args.Parameter, "Ok") && _isOkMessageOpen)
                    {
                        _isOkMessageOpen = false;
                        args.Cancel();
                        args.Session.UpdateContent(new PleaseWaitView());
                        Task.Run(() =>
                        {
                            try
                            {
                                _context.Students.Remove(SelectedStudent);
                                _context.SaveChanges();
                                result = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                result = false;
                            }


                        }).ContinueWith((t, _) =>
                        {
                            LoadStudents();
                            var message = result ? "Delete Successfull" : "Delete Failed";
                            _isOkMessageOpen = false;
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = message });
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });

            
            //Requirements = _context.RequirementStudents.AsNoTracking().Where(c => c.StudentId == SelectedStudent.Id)
            //    .ToList();
            //_oldRequirementStudents = Requirements;
            //await AddRequirementsToStudentAsync(SelectedStudent);
            //await DialogHost.Show(new StudentEditView() { DataContext = this }, "RootDialog", StudentEditClosingEventHandler);

        }

        //private void StudentEditClosingEventHandler(object sender, DialogClosingEventArgs args)
        //{
        //    bool result = false;
        //    if (Equals(args.Parameter, "Close"))
        //    {
        //        _context.Entry(SelectedStudent).CurrentValues.SetValues(_context.Entry(SelectedStudent).OriginalValues);
        //        Requirements = _oldRequirementStudents;
        //        return;
        //    }
        //    if (_isOkMessageOpen && Equals(args.Parameter, "Cancel"))
        //    {
        //        _isOkMessageOpen = false;
        //        args.Cancel();
        //        args.Session.UpdateContent(_content);
        //    }
        //    if (Equals(args.Parameter, "Update"))
        //    {
        //        args.Cancel();
        //        args.Session.UpdateContent(new PleaseWaitView());
        //        Task.Run(() =>
        //        {
        //            try
        //            {
        //                using (var context = new MorenoContext())
        //                {
        //                    foreach (var item in Requirements)
        //                    {
        //                        context.Entry(item).State = EntityState.Modified;
        //                    }
        //                    context.SaveChanges();
        //                }
        //                    SelectedStudent.YearLevel = SelectedYearLevel;
        //                    SelectedStudent.Section = SelectedSection;
        //                    _context.SaveChanges();
        //                    result = true;
        //            }

        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.Message);
        //                result = false;
        //            }
        //        }).ContinueWith((t, _) =>
        //        {
        //            LoadStudents();
        //            var message = result ? "Update Successfull" : "Update Failed";
        //            if (result)
        //            {
        //                LoadStudents();
        //            }
        //            args.Session.UpdateContent(new OkMessageDialog() {DataContext = message});
        //        }, null, TaskScheduler.FromCurrentSynchronizationContext());
        //    }
        //    if (Equals(args.Parameter, "Delete"))
        //    {
        //        _content = args.Session.Content;
        //        args.Cancel();
        //        _isOkMessageOpen = true;
        //        args.Session.UpdateContent(new OkCancelMessageDialog(){DataContext = $"Are you sure you want to delete {SelectedStudent.FullName}?"});
        //    }
        //    if (Equals(args.Parameter, "Ok") && _isOkMessageOpen)
        //    {
        //        _isOkMessageOpen = false;
        //        args.Cancel();
        //        args.Session.UpdateContent(new PleaseWaitView());
        //        Task.Run(() =>
        //        {
        //            try
        //            {
        //                _context.Students.Remove(SelectedStudent);
        //                _context.SaveChanges();
        //                result = true;
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.Message);

        //                result = false;
        //            }


        //        }).ContinueWith((t, _) =>
        //        {
        //            LoadStudents();
        //            var message = result ? "Delete Successfull" : "Delete Failed";
        //            _isOkMessageOpen = false;
        //            args.Session.UpdateContent(new OkMessageDialog() { DataContext = message });
        //        }, null, TaskScheduler.FromCurrentSynchronizationContext());
        //    }
        //}

        public DelegateCommand ChangeRequirementStatusCommand => new DelegateCommand(ChangeRequirement);

        private void ChangeRequirement()
        {
            SelectedRequirement.Status = !SelectedRequirement.Status;
            RaisePropertyChanged("Requirements");
        }
        public ObservableCollection<Section> Sections
        {
            get { return SelectedYearLevel?.Sections.ToObservableCollection(); }
        }

        public ObservableCollection<YearLevel> YearLevels
        {
            get { return GetProperty(() => YearLevels); }
            set { SetProperty(() => YearLevels, value); }
        }

        public List<String> GenderList
        {
            get { return GetProperty(() => GenderList); }
            set { SetProperty(() => GenderList, value); }
        }

        #endregion

        #region StudentAddView

        private object _content;
        private bool _result;

        public DelegateCommand ViewStudentAddCommand => new DelegateCommand(ViewStudentAdd);

        public ObservableCollection<Requirement> NewStudentRequirements
        {
            get { return GetProperty(() => NewStudentRequirements); }
            set { SetProperty(() => NewStudentRequirements, value); }
        }

        public Requirement SelectedNewRequirement
        {
            get { return GetProperty(() => SelectedNewRequirement); }
            set { SetProperty(() => SelectedNewRequirement, value); }
        }

        private async void ViewStudentAdd()
        {
            FirstName = null;
            MiddleName = null;
            LastName = null;
            Gender = null;
            Contact = null;
            Address = null;
            SchoolYear = null;
            Birthdate = null;
            LRN = null;
            SelectedYearLevel = null;
            SelectedSection = null;
            Photo = null;
            NewStudentRequirements = _context.Requirements.ToObservableCollection();
            YearLevels = _context.YearLevels.ToObservableCollection();
            GenderList = Enum.GetNames(typeof(EnumGender)).ToList();
            await DialogHost.Show(new StudentAddView() {DataContext = this}, "RootDialog",
                AddStudentClosing);
            
        }

        public DelegateCommand ChangeNewRequirementStatusCommand => new DelegateCommand(ChangeNewRequirement);

        private void ChangeNewRequirement()
        {
            SelectedNewRequirement.Status = !SelectedNewRequirement.Status;
            RaisePropertyChanged(() => NewStudentRequirements);
        }

        private async void AddStudentClosing(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, "Exit"))
            {
                return;
            }
            if (_isOkMessageOpen && _result)
            {
                _isOkMessageOpen = false;
                return;
            }
            else if (_isOkMessageOpen && !_result)
            {
                _isOkMessageOpen = false;
                args.Session.UpdateContent(_content);
            }
            args.Cancel();

            await ValidateAsync();
            RaisePropertyChanged(() => IsValid);

            if (Equals(args.Parameter, "Add") && !HasErrors)
            {
                args.Cancel();

                args.Session.UpdateContent(new PleaseWaitView());
                await Task.Run(() =>
                    {
                        try
                        {
                            Student NewStudent;

                            //using (var context = new MorenoContext())
                            //{
                            NewStudent = new Student()
                            {
                                FirstName = FirstName,
                                MiddleName = MiddleName,
                                LastName = LastName,
                                Gender = Gender,
                                Contact = Contact,
                                Address = Address,
                                SchoolYear = SchoolYear,
                                BirthDate = Birthdate.GetValueOrDefault(),
                                LRN = LRN,
                                YearLevel = SelectedYearLevel,
                                Section = SelectedSection,
                                Photo = Photo
                            };
                            _context.Students.Add(NewStudent);

                            _context.SaveChanges();
                            var user = new User()
                            {
                                UserName = LRN,
                                Password =
                                    $"{FirstName[0]}{MiddleName[0]}{LastName[0]}{Birthdate?.Date.ToString("dMyyyy")}"
                                        .ToLower(),
                                Authorization = "Student",
                                Student = NewStudent
                            };
                            _context.Users.Add(user);
                            _context.SaveChanges();
                            foreach (var requirement in NewStudentRequirements)
                            {
                                var studentRequirements = new RequirementStudents
                                {
                                    RequirementId = requirement.Id,
                                    Status = requirement.Status,
                                    StudentId = NewStudent.Id
                                };
                                NewStudent.RequirementStudents.Add(studentRequirements);
                                _context.SaveChanges();
                            }
                            

                            //_context.Students.Add(NewStudent);
                            //_context.Users.Add(user);

                            StatusMessage = $"{NewStudent.FullName} is registered";
                            _result = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);

                            StatusMessage = $"Failed to register Student";

                            _result = false;
                        }
                    })
                    .ContinueWith((t, _) =>
                    {
                        if (!_result)
                        {
                            _content = args.Session.Content;
                        }
                        LoadStudents();
                        args.Session.UpdateContent(new OkMessageDialog() {DataContext = StatusMessage});
                        _isOkMessageOpen = true;
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

    

        public string StatusMessage
        {
            get { return GetProperty(() => StatusMessage); }
            set { SetProperty(() => StatusMessage, value); }
        }

        #endregion

        #region Validator Members

        public string FirstName
        {
            get { return GetProperty(() => FirstName); }
            set { SetProperty(() => FirstName, value); }
        }

        public string MiddleName
        {
            get { return GetProperty(() => MiddleName); }
            set { SetProperty(() => MiddleName, value); }
        }

        public string LastName
        {
            get { return GetProperty(() => LastName); }
            set { SetProperty(() => LastName, value); }
        }

        public string Gender
        {
            get { return GetProperty(() => Gender); }
            set { SetProperty(() => Gender, value); }
        }

        public DateTime? Birthdate
        {
            get { return GetProperty(() => Birthdate); }
            set { SetProperty(() => Birthdate, value); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value); }
        }

        public string Contact
        {
            get { return GetProperty(() => Contact); }
            set { SetProperty(() => Contact, value); }
        }

        public string SchoolYear
        {
            get { return GetProperty(() => SchoolYear); }
            set { SetProperty(() => SchoolYear, value); }
        }

        public string LRN
        {
            get { return GetProperty(() => LRN); }
            set { SetProperty(() => LRN, value); }
        }

        public Section SelectedSection
        {
            get { return GetProperty(() => SelectedSection); }
            set { SetProperty(() => SelectedSection, value); }
        }

        public YearLevel SelectedYearLevel
        {
            get { return GetProperty(() => SelectedYearLevel); }
            set
            {
                SetProperty(() => SelectedYearLevel, value);
                RaisePropertyChanged(() => Sections);
            }
        }

        public byte[] Photo
        {
            get { return GetProperty(() => Photo); }
            set { SetProperty(() => Photo, value); }
        }

        private async void Validate()
        {
            await ValidateAsync();
        }

        public async Task ValidateAsync()
        {
            var result = await Validator.ValidateAllAsync();

            UpdateValidationSummary(result);
        }

        private void ConfigureValidationRules()
        {
            Validator.AddRequiredRule(() => LRN, "LRN is required");

            Validator.AddAsyncRule(nameof(LRN),
                async () =>
                {
                    var _context = new MorenoContext();
                    var result = await _context.Students.FirstOrDefaultAsync(e => e.LRN == LRN);
                    bool isAvailable = result == null;
                    return RuleResult.Assert(isAvailable,
                        string.Format("LRN {0} is taken. Please choose a different one.", LRN));
                });

            Validator.AddRequiredRule(() => FirstName, "First Name is required");

            Validator.AddRequiredRule(() => LastName, "Last Name is required");

            Validator.AddRequiredRule(() => MiddleName, "Middle Name is required");

            Validator.AddRequiredRule(() => Gender, "Gender is required");

            Validator.AddRequiredRule(() => Contact, "Contact is required");

            Validator.AddRequiredRule(() => Birthdate, "Birthdate is required");


            Validator.AddRequiredRule(() => Address, "Address is required");

            Validator.AddRequiredRule(() => SchoolYear, "School Year is required");

            Validator.AddRequiredRule(() => SelectedSection, "Section is required");

            Validator.AddRequiredRule(() => SelectedYearLevel, "Year Level is required");
        }

        private void OnValidationResultChanged(object sender, ValidationResultChangedEventArgs e)
        {
            if (HasErrors)
            {
                ValidationResult validationResult = Validator.GetResult();

                UpdateValidationSummary(validationResult);
            }
        }

        private void UpdateValidationSummary(ValidationResult validationResult)
        {
            //IsValid = validationResult.IsValid;
            ValidationErrorsString = validationResult.ToString();
        }

        public string ValidationErrorsString
        {
            get { return GetProperty(() => ValidationErrorsString); }
            private set { SetProperty(() => ValidationErrorsString, value); }
        }

        public bool IsValid
        {
            get { return HasErrors; }
            set { SetProperty(() => IsValid, HasErrors); }
        }


        #endregion

    }
}