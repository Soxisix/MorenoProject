using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Administrator;
using MorenoSystem.Views.Common;

namespace MorenoSystem.ViewModels
{
    public class AdministratorViewModel : ViewModelBase
    {
        private readonly MorenoContext _context;

        public AdministratorViewModel(ref MorenoContext context)
        {
            _context = context;

            LoadData();
        }

        private void LoadData()
        {
            Students = _context.Students.ToList();
            Teachers = _context.Teachers.ToList();
        }

        public List<Student> Students
        {
            get { return GetProperty(() => Students); }
            set { SetProperty(() => Students, value); }
        }

        public Student SelectedStudent
        {
            get { return GetProperty(() => SelectedStudent); }
            set { SetProperty(() => SelectedStudent, value); }
        }

        public List<Teacher> Teachers
        {
            get { return GetProperty(() => Teachers); }
            set { SetProperty(() => Teachers, value); }
        }

        public Teacher SelectedTeacher
        {
            get { return GetProperty(() => SelectedTeacher); }
            set { SetProperty(() => SelectedTeacher, value); }
        }

        public DelegateCommand EditStudentCommand => new DelegateCommand(DoEditStudent);

        private async void DoEditStudent()
        {
            await DialogHost.Show(new EditUser() {DataContext = this}, "RootDialog", EditStudentClosing);
        }

        private void EditStudentClosing(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false)) return;


            if (Equals(args.Parameter, "Update"))
            {
                args.Cancel();
                bool result;
                try
                {
                    //var duplicate = _context.Users.FirstOrDefault(c => c.UserName == SelectedStudent.User.UserName);
                    //if (duplicate != null)
                    //{
                    //    var OldValue = _context.Entry(SelectedStudent).OriginalValues;
                    //    SelectedStudent.User.UserName ;
                    //    LoadData();
                    //    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Username already exists" });
                    //    return;
                    //}
                    _context.SaveChanges();
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    result = false;
                }

                //args.Session.UpdateContent(new OkMessageDialog(){DataContext = result ? "Update Success" : "Update Failed"});
                args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Update Success" });
            }
        }

        public DelegateCommand EditTeacherCommand => new DelegateCommand(DoEditTeacher);

        private async void DoEditTeacher()
        {
            await DialogHost.Show(new EditTeacherUser() { DataContext = this }, "RootDialog", EditTeacherClosing);
        }

        private void EditTeacherClosing(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false)) return;


            if (Equals(args.Parameter, "Update"))
            {
                args.Cancel();
                bool result;
                try
                {
                    _context.SaveChanges();
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    result = false;
                }

                //args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? "Update Success" : "Update Failed" });
                args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Update Success"  });
            }
        }

        public string SearchTeacher
        {
            get { return GetProperty(() => SearchTeacher); }
            set
            {
                SetProperty(() => SearchTeacher, value);
                int numberId = 0;
                bool result = false;
                result = Int32.TryParse(value, out numberId);

                if (result)
                {
                    Teachers = _context.Teachers.Where(c => c.Id == numberId).ToList();
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    Teachers = _context.Teachers
                        .Where(c => c.FirstName.ToLower().Contains(value.ToLower()) || c.MiddleName.ToLower().Contains(value.ToLower()) ||
                                    c.LastName.ToLower().Contains(value.ToLower())).ToList();
                }
                else if (string.IsNullOrEmpty(value))
                {
                    Teachers = _context.Teachers.ToList();
                }
            }
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
                    Students = _context.Students.Where(c => c.Id == numberId).ToList();
                }
                else if (SearchStudent is string && !string.IsNullOrEmpty(SearchStudent))
                {
                    Students = _context.Students
                        .Where(c => c.FirstName.ToLower().Contains(SearchStudent.ToLower()) || c.MiddleName.ToLower().Contains(SearchStudent.ToLower()) ||
                                    c.LastName.ToLower().Contains(SearchStudent.ToLower())).ToList();
                }
                else if (string.IsNullOrEmpty(SearchStudent))
                {
                    Students = _context.Students.ToList();
                }
            }
        }

        public string UserName
        {
            get { return GetProperty(() => UserName); }
            set { SetProperty(() => UserName, value); }
        }

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value); }
        }

        public DelegateCommand AdminCommand => new DelegateCommand(DoAdmin);

        private async void DoAdmin()
        {
            await DialogHost.Show(new EditMainUser() {DataContext = this}, "RootDialog",
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    var user = _context.Users.FirstOrDefault(c => c.Authorization == "Administrator");
                    if (user != null)
                    {
                        UserName = user.UserName;
                        Password = user.Password;
                    }

                }, delegate(object sender, DialogClosingEventArgs args)
                {
                    if (Equals(args.Parameter, false))return;
                    if (Equals(args.Parameter, "Update"))
                    {
                        args.Cancel();
                        bool result;
                        try
                        {
                            var user = _context.Users.FirstOrDefault(c => c.Authorization == "Administrator");
                            if (user != null)
                            {
                                user.UserName = UserName;
                                user.Password = Password;
                            }
                            _context.SaveChanges();
                            result = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            result = false;
                        }

                        //args.Session.UpdateContent(new OkMessageDialog(){DataContext = result ? "Update Success" : "Update Failed"});
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Update Success" });
                    }
                });
        }

        public DelegateCommand LibraryCommand => new DelegateCommand(DoLibrary);

        private async void DoLibrary()
        {
            await DialogHost.Show(new EditMainUser() { DataContext = this }, "RootDialog",
                delegate (object sender, DialogOpenedEventArgs args)
                {
                    var user = _context.Users.FirstOrDefault(c => c.Authorization == "Library");
                    if (user != null)
                    {

                        UserName = user.UserName;
                        Password = user.Password;
                    }

                }, delegate (object sender, DialogClosingEventArgs args)
                {
                    if (Equals(args.Parameter, false)) return;
                    if (Equals(args.Parameter, "Update"))
                    {
                        args.Cancel();
                        bool result;
                        try
                        {
                            var user = _context.Users.FirstOrDefault(c => c.Authorization == "Library");
                            if (user != null)
                            {
                                user.UserName = UserName;
                                user.Password = Password;
                            }
                            _context.SaveChanges();
                            result = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            result = false;
                        }

                        //args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? "Update Success" : "Update Failed" });
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext =  "Update Success"  });
                    }
                });
        }

        public DelegateCommand RegistrarCommand => new DelegateCommand(DoRegistrar);

        private async void DoRegistrar()
        {
            await DialogHost.Show(new EditMainUser() { DataContext = this }, "RootDialog",
                delegate (object sender, DialogOpenedEventArgs args)
                {
                    var user = _context.Users.FirstOrDefault(c => c.Authorization == "Registrar");
                    if (user != null)
                    {
                        UserName = user.UserName;
                        Password = user.Password;
                    }

                }, delegate (object sender, DialogClosingEventArgs args)
                {
                    if (Equals(args.Parameter, false)) return;
                    if (Equals(args.Parameter, "Update"))
                    {
                        args.Cancel();
                        bool result;
                        try
                        {
                            var user = _context.Users.FirstOrDefault(c => c.Authorization == "Registrar");
                            if (user != null)
                            {
                                user.UserName = UserName;
                                user.Password = Password;
                            }
                            _context.SaveChanges();
                            result = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            result = false;
                        }

                        //args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? "Update Success" : "Update Failed" });
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Update Success"});
                    }
                });
        }

    }
}