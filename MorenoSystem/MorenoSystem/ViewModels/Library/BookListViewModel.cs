using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Common.Messages;
using MorenoSystem.Common.Validator;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;
using MorenoSystem.Views.Library;
using MvvmValidation;

namespace MorenoSystem.ViewModels.Library
{
    public class BookListViewModel : ValidatableViewModelBase
    {
        private MorenoContext _context;
        private bool _isOkMessageOpen;
        private bool _result;
        private object _content;

        public BookListViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
            Messenger.Default.Register<NewBookUpdate>(this, OnUpdateRecieve);
        }

        private void OnUpdateRecieve(NewBookUpdate obj)
        {
            LoadData();
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
                Books = _context.Books.Where(c => c.Id == numberId).ToObservableCollection();
            }
            else if (!string.IsNullOrEmpty(searchTxt))
            {
                Books = _context.Books
                    .Where(c => c.Title.ToLower().Contains(searchTxt.ToLower()) || c.Author.ToLower().Contains(searchTxt.ToLower()) ||
                                c.Publisher.ToLower().Contains(searchTxt.ToLower()) || c.Category.Name.ToLower().Contains(searchTxt.ToLower())).ToObservableCollection();
            }
            else if (string.IsNullOrEmpty(searchTxt))
            {
                Books = _context.Books.ToObservableCollection();
            }
        }

        private void UpdateBooks()
        {
            Messenger.Default.Send(new NewBookUpdate());
        }

        private void LoadData()
        {
            Books = _context.Books.ToObservableCollection();
        }

        public ObservableCollection<Book> Books
        {
            get { return GetProperty(() => Books); }
            set { SetProperty(() => Books, value); }
        }

        public Book SelectedBook
        {
            get { return GetProperty(() => SelectedBook); }
            set { SetProperty(() => SelectedBook, value); }
        }

        public List<DateTime> Years
        {
            get
            {
                return GetProperty(() => Years);
                var list = new DateTime();
            }
            set { SetProperty(() => Years, value); }
        }

        public List<Category> Categories
        {
            get { return _context.Categories.ToList(); }
        }

        public DelegateCommand EditCommand => new DelegateCommand(DoEdit);

        private async void DoEdit()
        {
            await DialogHost.Show(new BookEditView() {DataContext = this}, "RootDialog", EditClosing);
        }

        private void EditClosing(object o, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false)) return;
            if (Equals(args.Parameter, "Close"))
            {
                try
                {
                    _context.Entry(SelectedBook).CurrentValues.SetValues(_context.Entry(SelectedBook).OriginalValues);

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
                    var bookList = _context.TeacherBorrowedBooks.Where(c => c.Book.Id == SelectedBook.Id).ToList();
                    int quantityBorrowed = 0;
                    foreach (var book in bookList)
                    {
                        quantityBorrowed += book.QuantityBorrowed;
                    }
                    try
                    {
                        SelectedBook.AvailableQuantity = SelectedBook.Quantity -
                                                         (quantityBorrowed + SelectedBook.Damaged +
                                                          SelectedBook.Outdated);
                        _context.Entry(SelectedBook).State = EntityState.Modified;

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
                        //args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? "Update Successfull" : "Update Failed" });
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Update Successfull"  });
                        return;
                    }
                    LoadData();
                    UpdateBooks();
                    args.Session.Close(false);
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
            
            
        }

        public DelegateCommand DeleteCommand => new DelegateCommand(DoDelete, () =>
        {
            bool result;
            if (SelectedBook == null)
            {
                result = false;
            }
            try
            {
                result = _context.TeacherBorrowedBooks.Any(c => c.Book.Id == SelectedBook.Id);
            }
            catch (Exception )
            {
                result = false;
            }
            return !result;
        });

        private async void DoDelete()
        {
            await DialogHost.Show(new OkCancelMessageDialog(){DataContext = $"Delete {SelectedBook.Title}?"}, "RootDialog",
                delegate(object sender, DialogClosingEventArgs args)
                {
                    _context.Books.Remove(SelectedBook);
                    _context.SaveChanges();
                    Books.Remove(SelectedBook);
                    UpdateBooks();
                });
        }
        
        public DelegateCommand AddCommand => new DelegateCommand(DoAdd);

        private async void DoAdd()
        {
            Title = null;
            Author = null;
            Publisher = null;
            PublisedYear = null;
            Category = null;
            Quantity = null;
            await DialogHost.Show(new BookAddView() {DataContext = this}, "RootDialog", ClosingEventHandler);
        }

        private async void ClosingEventHandler(object o, DialogClosingEventArgs args)
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

                //var date = new DateTime(Int32.Parse(PublisedYear));
                args.Session.UpdateContent(new PleaseWaitView());
                await Task.Run(() =>
                    {
                        Book newBook = new Book();
                        try
                        {
                            newBook = new Book()
                            {
                                Title = Title,
                                Author = Author,
                                Publisher = Publisher,
                                PublishedYear = PublisedYear,
                                Category = Category,
                                Quantity = Int32.Parse(Quantity),
                                AvailableQuantity = Int32.Parse(Quantity)

                            };
                            _context.Books.Add(newBook);
                            _context.SaveChanges();
                            _result = true;
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            _content = args.Session.Content;
                            _result = false;
                        }
                        return newBook;
                    })
                    .ContinueWith((t, _) =>
                    {
                        if (_result)
                        {
                            Books.Add(t.Result);
                            UpdateBooks();
                        }
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = _result ? $"{t.Result.Title} is registered" : $"Failed to register new book" });
                        _isOkMessageOpen = true;
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }



        #region Validators

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public string Author
        {
            get { return GetProperty(() => Author); }
            set { SetProperty(() => Author, value); }
        }

        public string Publisher
        {
            get { return GetProperty(() => Publisher); }
            set { SetProperty(() => Publisher, value); }
        }

        public Category Category
        {
            get { return GetProperty(() => Category); }
            set { SetProperty(() => Category, value); }
        }

        public DateTime? PublisedYear
        {
            get { return GetProperty(() => PublisedYear); }
            set { SetProperty(() => PublisedYear, value); }
        }

        //public int AvailableQuantity
        //{
        //    get { return GetProperty(() => AvailableQuantity); }
        //    set { SetProperty(() => AvailableQuantity, value); }
        //}

        public string Quantity
        {
            get { return GetProperty(() => Quantity); }
            set { SetProperty(() => Quantity, value); }
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

            Validator.AddRequiredRule(() => Title, "Title is required");

            Validator.AddRequiredRule(() => Author, "Author is required");

            Validator.AddRequiredRule(() => Publisher, "Publisher is required");

            Validator.AddRequiredRule(() => Category, "Category is required");

            Validator.AddRequiredRule(() => PublisedYear, "PublisedYear is required : example (2017)");

            //Validator.AddRequiredRule(() => AvailableQuantity, "Available Quantity is required");


            Validator.AddRequiredRule(() => Quantity, "Quantity is required");

            Validator.AddRule(nameof(Title),
                () =>
                {
                    //var _context = new MorenoContext();
                    //string name = $"{LastName}, {FirstName} {MiddleName}";
                    using (var context = new MorenoContext())
                    {
                        var result = context.Books.FirstOrDefault(e => e.Title == Title);
                        bool isAvailable = result == null;
                        return RuleResult.Assert(isAvailable,
                            "Book Title already exisists");
                    }
                    
                    
                    
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