using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;

namespace MorenoSystem.ViewModels.Library
{
    public class CategoryViewModel : ViewModelBase
    {
        private MorenoContext _context;
        private bool _isOkMessageOpen;

        public CategoryViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            Categories = _context.Categories.ToObservableCollection();
        }

        public ObservableCollection<Category> Categories
        {
            get { return GetProperty(() => Categories); }
            set { SetProperty(() => Categories, value); }
        }

        public Category SelectedCategory
        {
            get { return GetProperty(() => SelectedCategory); }
            set { SetProperty(() => SelectedCategory, value); }
        }

        public DelegateCommand AddCommand => new DelegateCommand(DoAdd);

        private async void DoAdd()
        {
            await DialogHost.Show(new FieldMessageDialog() { DataContext = "Add New Category" }, "CategoryDialog",
                delegate (object sender, DialogClosingEventArgs args)
                {
                    bool result = false;
                    if (Equals(args.Parameter, false))
                    {
                        return;
                    }
                    if (args.Parameter is TextBox){
                        args.Session.UpdateContent(new PleaseWaitView());
                        TextBox txtName = (TextBox)args.Parameter;
                        string name = txtName.Text.Trim();
                        var category = new Category()
                        {
                            Name = name
                        };

                        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                            return;
                        }
                        var duplicate = _context.Categories.FirstOrDefault(c => c.Name == name);
                        if (duplicate != null)
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog(){DataContext = "Duplicate Name"});
                            return;
                        }
                        Task.Run(() =>
                        {
                            try
                            {
                                _context.Entry(category).State = EntityState.Added;

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
                                args.Cancel();
                                args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Failed to add" });
                            }
                            else
                            {
                                Categories.Add(category);
                            }

                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }

        public DelegateCommand EditCommand => new DelegateCommand(DoEit);

        private async void DoEit()
        {
            await DialogHost.Show(new FieldMessageDialog() { DataContext = $"Edit Name of {SelectedCategory.Name} " }, "CategoryDialog",
                delegate (object sender, DialogClosingEventArgs args)
                {
                    bool result = false;
                    if (Equals(args.Parameter, false))
                    {
                        return;
                    }
                    if (args.Parameter is TextBox)
                    {
                        args.Session.UpdateContent(new PleaseWaitView());
                        TextBox txtName = (TextBox)args.Parameter;
                        string name = txtName.Text.Trim();

                        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                            return;
                        }
                        var duplicate = _context.Categories.FirstOrDefault(c => c.Name == name);
                        if (duplicate != null)
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate Name" });
                            return;
                        }
                        SelectedCategory.Name = name;
                        Task.Run(() =>
                        {
                            try
                            {
                                _context.Entry(SelectedCategory).State = EntityState.Modified;
                                _context.SaveChanges();
                                result = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                result = false;
                            }
                        }).ContinueWith((t, _) =>
                        {
                            if (!result)
                            {
                                args.Cancel();
                                args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Edit Failed" });
                            }
                            else
                            {
                                LoadData();
                            }
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());

                    }
                });
        }

        public DelegateCommand DeleteCommand => new DelegateCommand(DoDelete);

        private async void DoDelete()
        {
            await DialogHost.Show(new OkCancelMessageDialog() { DataContext = $"Delete {SelectedCategory.Name}?" }, "CategoryDialog",
                delegate (object sender, DialogClosingEventArgs args)
                {
                    bool result = false;
                    if (Equals(args.Parameter, "Cancel")) return;

                    if (_isOkMessageOpen)
                    {
                        _isOkMessageOpen = false;
                        return;
                    }

                    if (Equals(args.Parameter, "Ok"))
                    {
                        args.Session.UpdateContent(new PleaseWaitView());
                        Task.Run(() =>
                        {
                            try
                            {
                                _context.Entry(SelectedCategory).State = EntityState.Deleted;

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
                                args.Cancel();
                                _isOkMessageOpen = true;
                                args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Failed to Delete" });
                            }
                            else
                            {
                                Categories.Remove(SelectedCategory);
                            }

                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }
    }
}