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

namespace MorenoSystem.ViewModels.Students
{
    public class YearSectionViewModel : ViewModelBase
    {
        private readonly MorenoContext _context;
        private bool _isOkMessageOpen;

        public YearSectionViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            //using (var context = new MorenoContext())
            //{
            //    YearLevels = context.YearLevels.OrderBy(c => c.Name).ToObservableCollection();
            //}
            YearLevels = _context.YearLevels.OrderBy(c => c.Name).ToObservableCollection();

        }

        //private void SortYear()
        //{
        //    var orderedEnumerable = YearLevels.OrderBy(c => c.Name);
        //}

        public ObservableCollection<YearLevel> YearLevels
        {
            get { return GetProperty(() => YearLevels); }
            set { SetProperty(() => YearLevels, value); }
        }

        public YearLevel SelectedYear
        {
            get { return GetProperty(() => SelectedYear); }
            set
            {
                SetProperty(() => SelectedYear, value);
                if (value != null)
                {
                    OnYearChanged(value);
                }
                
            }
        }

        private void OnYearChanged(YearLevel yearLevel)
        {
            Sections = yearLevel?.Sections?.ToObservableCollection();
            //using (var context = new MorenoContext())
            //{
            //Sections = _context.Sections.Where(c => c.YearLevel.Id == yearLevel.Id).ToObservableCollection();
            //}
        }

        public ObservableCollection<Section> Sections
        {
            get { return GetProperty(() => Sections); }
            set { SetProperty(() => Sections, value); }
        }

        public Section SelectedSection
        {
            get { return GetProperty(() => SelectedSection); }
            set { SetProperty(() => SelectedSection, value); }
        }

        public DelegateCommand AddYearCommand => new DelegateCommand(AddYear);

        private async void AddYear()
        {
            await DialogHost.Show(new FieldMessageDialog() { DataContext = "Add New Year Level" }, "YearDialog",
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
                        var yearLevel = new YearLevel()
                        {
                            Name = name
                        };

                        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                            return;
                        }

                        var duplicate = _context.YearLevels.FirstOrDefault(c => c.Name == name);
                        if (duplicate != null)
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog(){DataContext = "Duplicate Year Level"});
                            return;
                        }



                        Task.Run(() =>
                        {
                            try
                            {
                                using (var context = new MorenoContext())
                                {
                                    context.YearLevels.Add(yearLevel);
                                    context.SaveChanges();
                                }

                                //_context.Entry(yearLevel).State = EntityState.Added;

                                //_context.SaveChanges();
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
                                LoadData();
                            }

                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }

        public DelegateCommand EditYearCommand => new DelegateCommand(EditYear);

        private async void EditYear()
        {
            await DialogHost.Show(new FieldMessageDialog() { DataContext = $"Edit Name of {SelectedYear.Name}" }, "YearDialog",
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
                        var duplicate = _context.YearLevels.FirstOrDefault(c => c.Name == name);
                        if (duplicate != null)
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate Year Level" });
                            return;
                        }
                        //SelectedYear.Name = name;
                        Task.Run(() =>
                        {
                            try
                            {
                                //var entity = _context.YearLevels.Find(SelectedYear.Id);
                                //if (entity != null) entity.Name = name;
                                //_context.SaveChanges();
                                //using (var context = new MorenoContext())
                                //{
                                    var entity =
                                        _context.YearLevels.FirstOrDefault(c => c.Id == SelectedYear.Id);
                                    if (entity != null) entity.Name = name;
                                    _context.SaveChanges();
                                //}
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
                                //SortYear();
                            }
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());

                    }
                });
        }

        public DelegateCommand DeleteYearCommand => new DelegateCommand(DeleteYear);

        private async void DeleteYear()
        {
            await DialogHost.Show(new OkCancelMessageDialog() { DataContext = $"Delete {SelectedYear.Name}?" }, "YearDialog",
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
                            //try
                            //{
                            //_context.Entry(SelectedYear).State = EntityState.Deleted;

                            _context.YearLevels.Remove(SelectedYear);
                            _context.SaveChanges();
                            //using (var context = new MorenoContext())
                            //{
                            //    //var entity = _context.YearLevels.FirstOrDefault(c => c.Id == SelectedYear.Id);

                            //    //if (entity != null) context.YearLevels.Remove(entity);
                            //    context.YearLevels.Remove(SelectedYear);
                            //    context.SaveChanges();
                            //}
                            result = true;
                            //}
                            //catch (Exception e)
                            //{
                            //    Console.WriteLine(e.Message);

                                //result = false;
                            //}
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
                                Sections = null;
                                LoadData();
                                //SortYear();
                            }

                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }






        public DelegateCommand AddSectionCommand => new DelegateCommand(AddSection);

        private async void AddSection()
        {
            await DialogHost.Show(new FieldMessageDialog() { DataContext = "Add New Section" }, "SectionDialog",
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
                        var section = new Section()
                        {
                            Name = name,
                            YearLevel = SelectedYear
                        };


                        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                            return;
                        }
                        var duplicate = _context.Sections.FirstOrDefault(c => c.Name == name);
                        if (duplicate != null)
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate Section Name" });
                            return;
                        }

                        Task.Run(() =>
                        {
                            try
                            {
                                //using (var context = new MorenoContext())
                                //{
                                    _context.Sections.Add(section);
                                    _context.SaveChanges();
                                //}
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
                                Sections.Add(section);
                            }

                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }

        public DelegateCommand EditSectionCommand => new DelegateCommand(EditSection);

        private async void EditSection()
        {
            await DialogHost.Show(new FieldMessageDialog() { DataContext = $"Edit Name of {SelectedSection.Name}" }, "SectionDialog",
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
                        var duplicate = _context.Sections.FirstOrDefault(c => c.Name == name);
                        if (duplicate != null)
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate Section Name" });
                            return;
                        }
                        //SelectedYear.Name = name;
                        Task.Run(() =>
                        {
                            try
                            {
                                
                                    var entity =
                                        _context.Sections.FirstOrDefault(c => c.Id == SelectedSection.Id);
                                    if (entity != null) entity.Name = name;
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
                                //Sections = _context.Sections.Where(c => c.YearLevel == SelectedYear)
                                //    .ToObservableCollection();
                                Sections = SelectedYear.Sections.ToObservableCollection();
                            }
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());

                    }
                });
        }

        public DelegateCommand DeleteSectionCommand => new DelegateCommand(DeleteSection);

        private async void DeleteSection()
        {
            await DialogHost.Show(new OkCancelMessageDialog() { DataContext = $"Delete {SelectedSection.Name}?" }, "SectionDialog",
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
                                //_context.Entry(SelectedSection).State = EntityState.Deleted;


                                //_context.SaveChanges();
                                //using (var context = new MorenoContext())
                                //{
                                    _context.Sections.Remove(SelectedSection);
                                    _context.SaveChanges();
                                //}
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
                                Sections.Remove(SelectedSection);
                            }

                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }
    }

}