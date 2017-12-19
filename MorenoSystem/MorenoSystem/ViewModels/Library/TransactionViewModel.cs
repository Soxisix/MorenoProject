using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
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
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace MorenoSystem.ViewModels.Library
{
    public class TransactionViewModel : ValidatableViewModelBase
    {
        private MorenoContext _context;
        private Teacher _oldSelectedTeacher;
        private object _oldSession;

        public TransactionViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
            Messenger.Default.Register<NewBookUpdate>(this, OnBookUpdate);
        }

        public string SearchBook
        {
            get { return GetProperty(() => SearchBook); }
            set
            {
                SetProperty(() => SearchBook, value);
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
                    .Where(c => c.Title.ToLower().Contains(searchTxt.ToLower())).ToObservableCollection();
            }
            else if (string.IsNullOrEmpty(searchTxt))
            {
                Books = _context.Books.ToObservableCollection();
            }
        }

        public string SearchTeacher
        {
            get { return GetProperty(() => SearchTeacher); }
            set
            {
                SetProperty(() => SearchTeacher, value);
                OnSearchTeacherChanged(value);
            }
        }

        private void OnSearchTeacherChanged(string searchTxt)
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

        private void OnBookUpdate(NewBookUpdate obj)
        {
            LoadData();
        }

        private void LoadData()
        {
            Teachers = _context.Teachers.ToObservableCollection();
            Books = _context.Books.ToObservableCollection();
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

        public DelegateCommand BorrowCommand => new DelegateCommand(DoBorrow, () => SelectedTeacher != null);

        private async void DoBorrow()
        {
            await DialogHost.Show(
                new FieldMessageDialog() {DataContext = $"How many {SelectedBook.Title} book to borrow?"}, "RootDialog", OnBorrowClosing);
        }

        private void OnBorrowClosing(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false))
            {
                return;
            }

            if (args.Parameter is TextBox)
            {
                args.Session.UpdateContent(new PleaseWaitView());
                TextBox txtName = (TextBox)args.Parameter;
                int number;
                var name = txtName.Text.Trim();
                var tryParse = Int32.TryParse(name, out number);
                if (tryParse)
                {
                    if (number > SelectedBook.AvailableQuantity)
                    {
                        args.Cancel();
                        args.Session.UpdateContent(new OkMessageDialog(){DataContext = "Borrow quantity can't exceed available quantity."});
                        return;
                    }
                    if (number < 1)
                    {
                        args.Cancel();
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Minimum quantity is 1." });
                        return;
                    }
                }
                Task.Run(() =>
                {
                    if (tryParse)
                    {
                        _oldSelectedTeacher = SelectedTeacher;
                        var bookSearch =
                            _context.TeacherBorrowedBooks.FirstOrDefault(c => c.Teacher.Id == SelectedTeacher.Id &&
                                                                   c.Book.Id == SelectedBook.Id);
                        bool isFind = bookSearch != null;

                        if (!isFind)
                        {
                            var newBorrowedbook = new TeacherBorrowedBook
                            {
                                Teacher = SelectedTeacher,
                                Book = SelectedBook,
                                QuantityBorrowed = number,
                                DateBorrowed = DateTime.Now.Date,
                            };

                            //SelectedBook.AvailableQuantity = 
                            _context.TeacherBorrowedBooks.Add(newBorrowedbook);
                            _context.SaveChanges();
                        }
                        else
                        {
                            var borBook = _context.TeacherBorrowedBooks.FirstOrDefault(c => c.Teacher.Id == SelectedTeacher.Id &&
                                                                                            c.Book.Id == SelectedBook.Id);
                            if (borBook != null)
                            {
                                borBook.QuantityBorrowed = borBook.QuantityBorrowed + number;
                                //SelectedBook.AvailableQuantity -= number;
                                //_unitOfWork.TeacherBorrowedBook.ModifyBorrowedBook(borBook);
                            }
                        }
                        var newBorrowedBook = new TeacherBorrowedBook();
                        //var borrewdBook = _context.TeacherBorrowedBooks.


                        var bookList = _context.TeacherBorrowedBooks.Where(c => c.Book.Id == SelectedBook.Id).ToList();
                        int quantityBorrowed = 0;
                        foreach (var book in bookList)
                        {
                            quantityBorrowed += book.QuantityBorrowed;
                        }

                        SelectedBook.AvailableQuantity = SelectedBook.Quantity -
                                                         (quantityBorrowed + SelectedBook.Damaged +
                                                          SelectedBook.Outdated);

                        _context.SaveChangesAsync();
                        
                    }
                }).ContinueWith((t, _) =>
                {
                    LoadData();
                    Messenger.Default.Send(new NewBookUpdate());
                    SelectedTeacher = _oldSelectedTeacher;
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public DelegateCommand ViewDetailCommand => new DelegateCommand(DoViewDetail);

        private async void DoViewDetail()
        {
            var result =  _context.TeacherBorrowedBooks.Any(c => c.Teacher.Id == SelectedTeacher.Id);
            if (!result)
            {
                await DialogHost.Show(
                    new OkMessageDialog() {DataContext = $"{SelectedTeacher.FullName} has no borrowed book"}, "RootDialog");
            }
            else
            {
                BorrowedBooks = _context.TeacherBorrowedBooks.Where(c => c.Teacher.Id == SelectedTeacher.Id)
                    .ToObservableCollection();
                await DialogHost.Show(new TransactionDetailView() {DataContext = this}, "RootDialog", OnViewClosing);
            }
        }
        
        private void OnViewClosing(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false))
            {
                return;
            }

            bool result = false;

            if (Equals(args.Parameter, "RETURN"))
            {
                args.Cancel();
                _oldSession = args.Session.Content;


                //////MODIFIED
                ReturnedQuantity = SelectedBorrow.QuantityBorrowed.ToString();
                args.Session.UpdateContent(new ReturnDialog(){DataContext = this});
            }

            if (Equals(args.Parameter, "SUBMIT"))
            {
                args.Cancel();
                if (HasErrors)
                {
                    return;
                }

                try
                {
                    int num;
                    var resultParse = int.TryParse(ReturnedQuantity, out num);
                    
                    SelectedBorrow.QuantityBorrowed = SelectedBorrow.QuantityBorrowed - num;
                    if (SelectedBorrow.QuantityBorrowed < 1)
                    {
                        _context.TeacherBorrowedBooks.Remove(SelectedBorrow);
                    }

                    var bookList = _context.TeacherBorrowedBooks.Where(c => c.Book.Id == SelectedBorrow.BookId).ToList();
                    int quantityBorrowed = 0;
                    foreach (var book in bookList)
                    {
                        quantityBorrowed += book.QuantityBorrowed;
                    }

                    var book1 = _context.Books.FirstOrDefault(c => c.Id == SelectedBorrow.BookId);
                    if (book1 != null)
                    {
                        book1.AvailableQuantity = SelectedBook.Quantity -
                                                 (quantityBorrowed + SelectedBook.Damaged +
                                                  SelectedBook.Outdated);
                    }
                    
                    _context.SaveChanges();
                    LoadData();
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    result = false;
                }

                //args.Session.UpdateContent(new OkMessageDialog(){DataContext = result ? $"{ReturnedQuantity} books returned" : "Failed to return book"});
                args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? $"{ReturnedQuantity} books returned" : "books returned" });
            }

            if (Equals(args.Parameter, "Ok"))
            {
                args.Cancel();
                args.Session.UpdateContent(_oldSession);
                BorrowedBooks = _context.TeacherBorrowedBooks.ToObservableCollection();
            }
        }

        public DelegateCommand ReturnCommand => new DelegateCommand(DoReturn);

        private async void DoReturn()
        {
            await ValidateAsync();
            RaisePropertyChanged(() => IsValid);

            if (!HasErrors)
            {
                DialogHost.CloseDialogCommand.Execute("RETURN", null);
            }
        }

        public string ReturnedQuantity
        {
            get
            {
                return GetProperty(() => ReturnedQuantity);
                
            }
            set
            {
                
                SetProperty(() => ReturnedQuantity, value, Validate);
                RaisePropertyChanged(() => IsValid);
            }
        }

        public ObservableCollection<TeacherBorrowedBook> BorrowedBooks
        {
            get { return GetProperty(() => BorrowedBooks); }
            set { SetProperty(() => BorrowedBooks, value); }
        }

        public TeacherBorrowedBook SelectedBorrow
        {
            get { return GetProperty(() => SelectedBorrow); }
            set { SetProperty(() => SelectedBorrow, value); }
        }

        #region Validators

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
            
            Validator.AddRule(nameof(ReturnedQuantity), () =>
            {
                int num;
                var result = int.TryParse(ReturnedQuantity, out num);
                return RuleResult.Assert(!(num > SelectedBorrow.QuantityBorrowed),
                        $"Quantity is greater than borrowed book. {SelectedBorrow.QuantityBorrowed} borrowed books.");
                

            });
            
            Validator.AddRule(nameof(ReturnedQuantity), () =>
            {
                int num;
                var result = int.TryParse(ReturnedQuantity, out num);
                return RuleResult.Assert(result,
                    $"Quantity must be a number.");


            });

            Validator.AddRule(nameof(ReturnedQuantity), () =>
            {
                int num;
                var result = int.TryParse(ReturnedQuantity, out num);
                if (result)
                {
                    if (num < 1)
                    {
                        result = false;
                    }
                }
                return RuleResult.Assert(result,
                    $"Minimum quantity is 1.");


            });
            Validator.AddRequiredRule(() => ReturnedQuantity, "Quantity is required");
        }
        //$"Quantity is greater than borrowed book. {SelectedBorrow.QuantityBorrowed} borrowed books"
        private void OnValidationResultChanged(object sender, ValidationResultChangedEventArgs e)
        {
            if (HasErrors)
            {
                MvvmValidation.ValidationResult validationResult = Validator.GetResult();

                UpdateValidationSummary(validationResult);
            }
        }

        private void UpdateValidationSummary(MvvmValidation.ValidationResult validationResult)
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