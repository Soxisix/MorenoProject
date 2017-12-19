using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Common.Enums;
using MorenoSystem.Common.Messages;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.ViewModels;
using MorenoSystem.ViewModels.Library;
using MorenoSystem.ViewModels.Students;
using MorenoSystem.ViewModels.Teachers;
using MorenoSystem.ViewModels.Vote.Admin;
using MorenoSystem.ViewModels.Vote.Voters;
using MorenoSystem.Views;
using MorenoSystem.Views.Administrator;
using MorenoSystem.Views.Common;
using MorenoSystem.Views.Library;
using MorenoSystem.Views.Library.Report;
using MorenoSystem.Views.Student;
using MorenoSystem.Views.Student.Report;
using MorenoSystem.Views.Student.StudentList;
using MorenoSystem.Views.Teacher;
using MorenoSystem.Views.Vote.Admin;
using MorenoSystem.Views.Vote.Voters;

namespace MorenoSystem
{
    public class MainViewModel : ViewModelBase
    {
        private MorenoContext _context;
        private bool _serverFound;
        private bool _isOkMessageOpen;
        private bool _isTest;
        private Student _currentStudent;

        public MainViewModel(MorenoContext context)
        {
            _context = context;
            Messenger.Default.Register<StartLoginDialog>(this, OnRenderedMainView);
            Messenger.Default.Register<SetNavigation>(this, SetCurrenNavigation);
            
        }

        protected override void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();
            LoginItems = new List<string>()
            {
                "Information System",
                "Library System",
                "Voting System",
                "Student",
                "Administrator"
            };
            SelectedLoginItem = "Information System";
            IsLeftDrawerOpen = false;

            Username = "c";
            Password = "c";
        }

        public bool IsLeftDrawerOpen
        {
            get { return GetProperty(() => IsLeftDrawerOpen); }
            set { SetProperty(() => IsLeftDrawerOpen, value); }
        }

        public DelegateCommand LogoutCommand => new DelegateCommand(DoLogout);

        private async void DoLogout()
        {
            await DialogHost.Show(new OkCancelMessageDialog() {DataContext = "Are you sure you want to logout?"},
                "RootDialog",
                delegate(object sender, DialogClosingEventArgs args)
                {
                    if (Equals(args.Parameter, "Ok"))
                    {
                        IsLeftDrawerOpen = false;
                        args.Session.UpdateContent(new PleaseWaitView());
                        IsHaveUser = false;
                        NavigationItems = null;
                        ShowLoginView();
                    }
                });
            ;
        }

        public bool IsHaveUser
        {
            get { return GetProperty(() => IsHaveUser); }
            set { SetProperty(() => IsHaveUser, value); }
        }

        public DelegateCommand LoginCommand => new DelegateCommand(UserLogin);

        private async void UserLogin()
        {
            string loginType;
            switch (SelectedLoginItem)
            {
                case "Information System":
                    loginType = "Registrar";
                    break;
                case "Library System":
                    loginType = "Library";
                    break;
                case "Voting System":
                    loginType = "Registrar";
                    break;
                case "Student":
                    loginType = "Student";
                    break;
                    
                default:
                    loginType = "Admin";
                    break;
            }

            await DialogHost.Show(new PleaseWaitView(), "RootDialog",
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    Task.Run(() =>
                    {
                        var user = _context.Users.FirstOrDefault(c => c.UserName == Username);
                        if (user == null)
                        {
                            StatusMessage = "Incorrect Username and password!";
                            LoginError = true;
                        }
                        else if (loginType == "Student" && Password == user.Password)
                        {
                            StatusMessage = "Login Successful!";
                            _currentStudent = user.Student;
                            LoginError = false;
                            IsHaveUser = true;
                        }
                        else if (Equals(user?.Password, Password) && Equals(user?.Authorization, "Administrator"))
                        {
                            StatusMessage = "Login Successful! You are logged in as an Administrator";
                            IsHaveUser = true;
                            LoginError = false;
                        }
                        else if (Equals(user?.Password, Password) && Equals(user?.Authorization, loginType))
                        {
                            StatusMessage = "Login Successful!";
                            IsHaveUser = true;
                            LoginError = false;
                        }
                        else if (Equals(user?.Password, Password) && !Equals(user?.Authorization, loginType))
                        {
                            StatusMessage = $"Your account is not authorized for {SelectedLoginItem}";
                            LoginError = true;
                        }
                        else
                        {
                            StatusMessage = "Incorrect Username and password!";
                            LoginError = true;
                        }
                        Thread.Sleep(500);
                    }).ContinueWith((t, _) =>
                        {
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = StatusMessage });
                        }, null,
                        TaskScheduler.FromCurrentSynchronizationContext());
                }, LoginClosing);
        }

        private void LoginClosing(object o, DialogClosingEventArgs eventArgs)
        {
            eventArgs.Session.UpdateContent(new PleaseWaitView());
            Task.Run(() =>
            {
                Thread.Sleep(500);
            }).ContinueWith((t, _) =>
            {
                if (!LoginError)
                {
                    Username = null;
                    Password = null;
                    LoginType loginTo = LoginType.InformationSytem;
                    switch (SelectedLoginItem)
                    {
                        case "Information System":
                            loginTo = LoginType.InformationSytem;
                            break;
                        case "Library System":
                            loginTo = LoginType.LibrarySytem;
                            break;
                        case "Voting System":
                            loginTo = LoginType.VotingSystem;
                            break;
                        case "Student":
                            loginTo = LoginType.StudentLogin;
                            break;
                        case "Administrator":
                            loginTo = LoginType.Administrator;
                            break;
                    }
                    switch (loginTo)
                    {
                        case LoginType.InformationSytem:
                            NavigationItems = InformationSystemItems();
                            break;
                        case LoginType.LibrarySytem:
                            NavigationItems = LibrarySystem();
                            break;
                        case LoginType.VotingSystem:
                            NavigationItems = VotingSystem();
                            break;
                        case LoginType.StudentLogin:
                            NavigationItems = StudentSystem();
                            break;
                        case LoginType.Administrator:
                            NavigationItems = AdminSystem();
                            break;
                    }
                }
            }, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public DelegateCommand ExitCommand => new DelegateCommand(CloseWindow);

        private static void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        public string StatusMessage
        {
            get { return GetProperty(() => StatusMessage); }
            set { SetProperty(() => StatusMessage, value); }
        }

        public bool LoginError { get; set; }

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value); }
        }

        public string Username
        {
            get { return GetProperty(() => Username); }
            set { SetProperty(() => Username, value); }
        }

        public List<string> LoginItems { get; set; }

        public string SelectedLoginItem
        {
            get { return GetProperty(() => SelectedLoginItem); }
            set { SetProperty(() => SelectedLoginItem, value); }
        }

        public Object LoginContent { get; set; }

        private async void OnRenderedMainView(StartLoginDialog obj)
        {
            ///////////// Modified added Task.Run please wait indicator
            await DialogHost.Show(new PleaseWaitView(), "RootDialog",
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            _serverFound = _context.Database.Exists();
                            if (!_serverFound)
                            {
                                var e = _context.Users.FirstOrDefault();
                                _serverFound = _context.Database.Exists();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }).ContinueWith((t, _) =>
                    {
                        if (_serverFound)
                        {
                            ShowLoginView();
                            args.Session.Close();
                        }
                        else
                        {
                            args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Database not found"});
                        }
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
                });

            if (!_serverFound)

            {
                await DialogHost.Show(new ChangeServerView() {DataContext = this}, "RootDialog",
                    ChangeServerClosingEventHandler);
            }
        }

        private void ChangeServerClosingEventHandler(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false))
            {
                CloseWindow();
            }

            if (Equals(args.Parameter, "Test"))
            {
                _isTest = true;
                args.Cancel();
                args.Session.UpdateContent(new PleaseWaitView());
                Task.Run(() =>
                {
                    var conString =
                        $"server={Server};port={Port};database=missms;uid={DatabaseUsername};password={DatabasePassword};";
                    _context.Database.Connection.ConnectionString = conString;
                    try
                    {
                        var result = _context.Users.First();
                        _serverFound = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        _serverFound = false;
                        StatusMessage = $"Server not found {e.Message}";
                    }
                    if (_serverFound)
                    {
                        StatusMessage = "Connection Successful";
                    }}).ContinueWith((t, _) =>
                {
                    args.Session.UpdateContent(new OkMessageDialog() {DataContext = StatusMessage});
                    _isOkMessageOpen = true;
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (Equals(args.Parameter, "Update"))
            {
                args.Cancel();
                args.Session.UpdateContent(new PleaseWaitView());
                Task.Run(() =>
                {
                    var conString =
                        $"server={Server};port={Port};database=missms;uid={DatabaseUsername};password={DatabasePassword};";
                    _context.Database.Connection.ConnectionString = conString;
                    try
                    {
                        var result = _context.Users.First();
                        _serverFound = true;
                        ChacngeConnString(conString);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        _serverFound = false;
                    }
                    if (_serverFound)
                    {
                        StatusMessage = "Update Successful";
                    }
                    else
                    {
                        StatusMessage = "Server not Found";
                    }
                }).ContinueWith((t, _) =>
                {
                    args.Session.UpdateContent(new OkMessageDialog() {DataContext = StatusMessage});
                    _isOkMessageOpen = true;
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (_isOkMessageOpen)
            {
                if (!_serverFound)
                {
                    args.Cancel();
                    args.Session.UpdateContent(new ChangeServerView() {DataContext = this});
                }
                else if (_serverFound && _isTest)
                {
                    args.Cancel();
                    args.Session.UpdateContent(new ChangeServerView() {DataContext = this});
                    _isTest = false;
                }
                else if (_serverFound && !_isTest)
                {
                    ShowLoginView();
                }
                _isOkMessageOpen = false;
            }
        }

        private void ChacngeConnString(String con)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var conString = new ConnectionStringSettings
            {
                ProviderName = "MySql.Data.MySqlClient",
                ConnectionString = con,
                Name = "AbmuConnString"
            };
            config.ConnectionStrings.ConnectionStrings.Remove("AbmuConnString");
            config.ConnectionStrings.ConnectionStrings.Add(conString);
            config.Save(ConfigurationSaveMode.Modified, true);

            _context.Database.Connection.ConnectionString = con;

            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public string Server
        {
            get { return GetProperty(() => Server); }
            set { SetProperty(() => Server, value); }
        }

        public string Port
        {
            get { return GetProperty(() => Port); }
            set { SetProperty(() => Port, value); }
        }

        public string DatabaseUsername
        {
            get { return GetProperty(() => DatabaseUsername); }
            set { SetProperty(() => DatabaseUsername, value); }
        }

        public string DatabasePassword
        {
            get { return GetProperty(() => DatabasePassword); }
            set { SetProperty(() => DatabasePassword, value); }
        }

        public void ShowLoginView()
        {
            SetCurrenNavigation(new SetNavigation{Content = new LoginView() { DataContext = this } });
            ;
            //await DialogHost.Show(new LoginView { DataContext = this }, "RootDialog", LoginDialogClosingEventHandler);
        }

        public Object CurrentNavigationItem
        {
            get { return GetProperty(() => CurrentNavigationItem); }
            set { SetProperty(() => CurrentNavigationItem, value); }
        }

        public NavigationItem SelectedNavigationItem
        {
            get { return GetProperty(() => SelectedNavigationItem); }
            set
            {
                SetProperty(() => SelectedNavigationItem, value);

                CurrentNavigationItem = value?.Content;
            }
        }

        public List<NavigationItem> NavigationItems
        {
            get { return GetProperty(() => NavigationItems); }
            set
            {
                SetProperty(() => NavigationItems, value);
                SelectedNavigationItem = value?.FirstOrDefault();
            }
        }

        public void SetCurrenNavigation(SetNavigation obj)
        {
            CurrentNavigationItem = obj.Content;
        }

        private List<NavigationItem> InformationSystemItems()
        {
            var items = new List<NavigationItem>()
            {
                new NavigationItem("Students Information",
                    new StudentListView() {DataContext = new StudentViewModel(ref _context)}, PackIconKind.School),
                new NavigationItem("Teacher Information",
                    new TeacherListView() {DataContext = new TeacherListViewModel(ref _context)}, PackIconKind.AccountStarVariant),
                new NavigationItem("Manage Advisers",
                    new AdviserView() {DataContext = new AdviserViewModel(ref _context)},
                    PackIconKind.AccountSettings),
                new NavigationItem("Manage Requirements",
                    new RequirementsView() {DataContext = new RequirementsViewModel(ref _context)},
                    PackIconKind.PlaylistCheck),
                new NavigationItem("Manage Year Levels and Sections",
                    new YearSectionView() {DataContext = new YearSectionViewModel(ref _context)},
                    PackIconKind.Stairs),
                new NavigationItem("Student Reports",
                    new StudentReportView() {DataContext = new StudentReportViewModel(ref _context)}, PackIconKind.ChartBar),
                
            };
            return items;
        }

        private List<NavigationItem> LibrarySystem()
        {
            var items = new List<NavigationItem>()
            {
                new NavigationItem("Books Information",
                    new BookListView() {DataContext = new BookListViewModel(ref _context)}, PackIconKind.Book),
                new NavigationItem("Category List",
                    new CategoryView() {DataContext = new CategoryViewModel(ref _context)}, PackIconKind.ViewList),
                new NavigationItem("Book Borrow / Return",
                    new TransactionView() {DataContext = new TransactionViewModel(ref _context)}, PackIconKind.BookOpen),
                new NavigationItem("Book Reports",
                    new LibraryReportView() {DataContext = new LibraryReportViewModel(ref _context)}, PackIconKind.ChartBar),


            };
            return items;
        }

        private List<NavigationItem> VotingSystem()
        {
            var items = new List<NavigationItem>()
            {
                new NavigationItem("Party Lists",
                    new PartyListView() {DataContext = new PartyListViewModel(ref _context)}, PackIconKind.School),
                new NavigationItem("Manage Positions",
                    new ManagePositionView() {DataContext = new ManagePositionViewModel(ref _context)}, PackIconKind.Stairs),
                new NavigationItem("Vote Counts",
                    new VoteResultView() {DataContext = new VoteResultViewModel(ref _context)}, PackIconKind.Numeric),
                new NavigationItem("Election History",
                    new ElectionHistoryView() {DataContext = new ElectionHistoryViewModel(ref _context)}, PackIconKind.ChartHistogram),
            };
            return items;
        }

        private List<NavigationItem> StudentSystem()
        {
            var items = new List<NavigationItem>()
            {
                new NavigationItem("Student Profile",
                    new VoterProfileView() {DataContext = new VoterProfileViewModel(ref _context, _currentStudent)},
                    PackIconKind.School),
            };
            return items;
        }

        private List<NavigationItem> AdminSystem()
        {
            var items = new List<NavigationItem>()
            {
                new NavigationItem("Users Information",
                    new AdminView() {DataContext = new AdministratorViewModel(ref _context)},
                    PackIconKind.School),
            };
            return items;
        }
    }
}