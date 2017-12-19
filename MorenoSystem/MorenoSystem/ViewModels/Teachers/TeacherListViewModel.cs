using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using MorenoSystem.Common;
using MorenoSystem.Common.Validator;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common.Enums;
using MorenoSystem.Common.Messages;
using MorenoSystem.Views.Common;
using MorenoSystem.Views.Teacher;
using MvvmValidation;

namespace MorenoSystem.ViewModels.Teachers
{
    public class TeacherListViewModel : ValidatableViewModelBase
    {
        private MorenoContext _context;
        private object _content;

        private bool _result;

        private bool _isOkMessageOpen;

        public TeacherListViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
            GenderList = Enum.GetNames(typeof(EnumGender)).ToList();
        }

        private void LoadData()
        {
            Teachers = _context.Teachers.ToObservableCollection();
        }

        public ObservableCollection<Teacher> Teachers
        {
            get { return GetProperty(() => Teachers); }
            set { SetProperty(() => Teachers, value); }
        }

        public Teacher SelectedTeacher
        {
            get { return GetProperty(() => SelectedTeacher); }
            set { SetProperty(() => SelectedTeacher, value); }
        }

        public string Search
        {
            get { return GetProperty(() => Search); }
            set
            {
                SetProperty(() => Search, value);
                OnSearchChanged(value);
            }
        }

        private void OnSearchChanged(string searchTxt)
        {
            int numberId = 0;
            bool result = false;
            result = Int32.TryParse(searchTxt, out numberId);

            if (result)
            {
                Teachers = _context.Teachers.Where(c => c.Id == numberId).ToObservableCollection();
            }
            else if (!string.IsNullOrEmpty(searchTxt))
            {
                Teachers = _context.Teachers
                    .Where(c => c.FirstName.ToLower().Contains(searchTxt.ToLower()) || c.MiddleName.ToLower().Contains(searchTxt.ToLower()) ||
                                c.LastName.ToLower().Contains(searchTxt.ToLower())).ToObservableCollection();
            }
            else if (string.IsNullOrEmpty(searchTxt))
            {
                Teachers = _context.Teachers.ToObservableCollection();
            }
        }

        public List<String> GenderList
        {
            get { return GetProperty(() => GenderList); }
            set { SetProperty(() => GenderList, value); }
        }

        //public DelegateCommand ViewCommand => new DelegateCommand(ShowTeacher, () => SelectedTeacher != null);

        //private void ShowTeacher()
        //{

        //}

        private void SendUpdateMessage()
        {
            Messenger.Default.Send(new UpdateTeacherMessage());
        }

        public DelegateCommand EditViewCommand => new DelegateCommand(ShowEditTeacher, () => SelectedTeacher != null);

        private async void ShowEditTeacher()
        {
            await DialogHost.Show(new TeacherEditView() {DataContext = this}, "RootDialog", OnEditClosing);
        }

        private void OnEditClosing(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false)) return;
            if (Equals(args.Parameter, "Close"))
            {
                try
                {
                    _context.Entry(SelectedTeacher).CurrentValues.SetValues(_context.Entry(SelectedTeacher).OriginalValues);
                    LoadData();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return;
            }
            bool result = false;

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
                    
                    try
                    {
                        _context.Entry(SelectedTeacher).State = EntityState.Modified;
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

                    
                    if (!result)
                    {
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? "Update Successfull" : "Update Failed" });
                        return;
                    }
                    LoadData();
                    SendUpdateMessage();
                    args.Session.Close(false);


                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
            if (Equals(args.Parameter, "Delete"))
            {
                _content = args.Session.Content;
                args.Cancel();
                _isOkMessageOpen = true;
                args.Session.UpdateContent(new OkCancelMessageDialog() { DataContext = $"Are you sure you want to delete {SelectedTeacher.FullName}?" });
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
                        _context.Teachers.Remove(SelectedTeacher);
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
                    LoadData();
                    _isOkMessageOpen = false;
                    SendUpdateMessage();
                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? "Delete Successfull" : "Delete Failed" });
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }

        }

        public DelegateCommand RegisterViewCommand => new DelegateCommand(ShowRegisterTeacher);

        
        //private bool _isOkMessageOpen;

        private async void ShowRegisterTeacher()
        {
            //GenderList = Enum.GetNames(typeof(EnumGender)).ToList();

            FirstName = null;
            MiddleName = null;
            LastName = null;
            Gender = null;
            Contact = null;
            Address = null;
            Birthdate = null;
            Photo = null;
            await DialogHost.Show(new TeacherAddView() {DataContext = this}, "RootDialog", OnRegisterClosing);
        }

        private async void OnRegisterClosing(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false)) return;

            if (_isOkMessageOpen && _result)
            {
                _isOkMessageOpen = false;
                return;
            }
            if (_isOkMessageOpen && !_result)
            {
                args.Session.UpdateContent(_content);
            }

            args.Cancel();
            await ValidateAsync();
            RaisePropertyChanged(() => IsValid);

            if (Equals(args.Parameter, "Add") && !HasErrors)
            {
                args.Session.UpdateContent(new PleaseWaitView());
                await Task.Run(() =>
                    {
                        try
                        {
                            var newTeacher = new Teacher()
                            {
                                FirstName = FirstName,
                                MiddleName = MiddleName,
                                LastName = LastName,
                                Gender = Gender,
                                ContactNo = Contact,
                                Birthday = Birthdate.GetValueOrDefault(),
                                Address = Address,
                                Photo = Photo,

                            };
                            _context.Teachers.Add(newTeacher);
                            _context.SaveChanges();
                            _result = true;
                            return $"{newTeacher.FullName} is registered";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            _content = args.Session.Content;
                            _result = false;
                            return $"Failed to register Student";
                        }
                    })
                    .ContinueWith((t, _) =>
                    {
                        LoadData();
                        SendUpdateMessage();
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = t.Result });
                        _isOkMessageOpen = true;
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        #region Validators

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
            
            Validator.AddRequiredRule(() => FirstName, "First Name is required");

            Validator.AddRequiredRule(() => LastName, "Last Name is required");

            Validator.AddRequiredRule(() => MiddleName, "Middle Name is required");

            Validator.AddRequiredRule(() => Gender, "Gender is required");

            Validator.AddRequiredRule(() => Contact, "Contact is required");

            Validator.AddRequiredRule(() => Birthdate, "Birthdate is required");


            Validator.AddRequiredRule(() => Address, "Address is required");

            Validator.AddRule(nameof(FirstName),
                 () =>
                {
                    //var _context = new MorenoContext();
                    string name = $"{LastName}, {FirstName} {MiddleName}";
                    var result =  _context.Teachers.FirstOrDefault(e => e.FullName == name);
                    bool isAvailable = result == null;
                    return RuleResult.Assert(isAvailable,
                        "Existing Name has found!");
                });
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
            //IsValid  = validationResult.IsValid;
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