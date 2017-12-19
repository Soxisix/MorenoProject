using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using DevExpress.DataProcessing;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;

namespace MorenoSystem.ViewModels.Students
{
    public class RequirementsViewModel : ViewModelBase
    {
        private MorenoContext _context;
        private bool _isOkMessageOpen;

        public RequirementsViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            //Requirements = _context.Requirements.ToObservableCollection();
            using (var context = new MorenoContext())
            {
                Requirements = context.Requirements.ToObservableCollection();
            }
        }

        public ObservableCollection<Requirement> Requirements
        {
            get { return GetProperty(() => Requirements); }
            set { SetProperty(() => Requirements, value); }
        }

        public Requirement SelectedRequirement
        {
            get { return GetProperty(() => SelectedRequirement); }
            set { SetProperty(() => SelectedRequirement, value); }
        }

        public DelegateCommand AddRequirementCommand => new DelegateCommand(DoAddRequirement);

        private async void DoAddRequirement()
        {
            await DialogHost.Show(new FieldMessageDialog() {DataContext = "Add New Requirement"}, "RequirementDialog",
                delegate(object sender, DialogClosingEventArgs args)
                {
                    bool result = false;
                    if (Equals(args.Parameter, false))
                    {
                        return;
                    }
                    if (args.Parameter is TextBox)
                    {
                        args.Session.UpdateContent(new PleaseWaitView());
                        TextBox txtName = (TextBox) args.Parameter;
                        string name = txtName.Text.Trim();
                        var requirement = new Requirement()
                        {
                            Name = name
                        };

                        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                            return;
                        }

                        using (var context = new MorenoContext())
                        {
                            var duplicate = context.Requirements.FirstOrDefault(c => c.Name == name);
                            if (duplicate != null)
                            {
                                args.Cancel();
                                args.Session.UpdateContent(new OkMessageDialog(){DataContext = "Duplicate Name"});
                                return;
                            }
                        }

                        Task.Run(() =>
                        {
                            try
                            {
                                //_context.Entry(requirement).State = EntityState.Added;
                                using (var context = new MorenoContext())
                                {
                                    context.Requirements.Add(requirement);

                                    context.SaveChanges();
                                    result = true;
                                }
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
                                args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Failed to add"});
                            }
                            else
                            {
                                Requirements.Add(requirement);
                            }
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }

        public DelegateCommand DeleteRequirementCommand => new DelegateCommand(DoDeleteRequirement);

        private async void DoDeleteRequirement()
        {
            await DialogHost.Show(new OkCancelMessageDialog() {DataContext = $"Delete {SelectedRequirement.Name}?"},
                "RequirementDialog",
                delegate(object sender, DialogClosingEventArgs args)
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
                                //_context.Entry(SelectedRequirement).State = EntityState.Deleted;
                                //var students = _context.Students.ToList();
                                //foreach (var student in students)
                                //{
                                //     student.RequirementStudents.FirstOrDefault(
                                //        c => c.RequirementId == SelectedRequirement.Id);
                                //}
                                //var entity = _context.Requirements.FirstOrDefault(c => c.Id == SelectedRequirement.Id);
                                //if (entity != null) _context.Requirements.Remove(entity);
                                using (var context = new MorenoContext())
                                {
                                    var entity =
                                        context.Requirements.FirstOrDefault(c => c.Id == SelectedRequirement.Id);
                                    if (entity != null) context.Requirements.Remove(entity);        
                                    context.SaveChanges();
                                    result = true;
                                }
                                //_context.SaveChanges();
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
                                args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Failed to Delete"});
                            }
                            else
                            {
                                Requirements.Remove(SelectedRequirement);
                            }
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }

        public DelegateCommand EditRequirementCommand => new DelegateCommand(DoEditRequirement);

        private async void DoEditRequirement()
        {
            await DialogHost.Show(
                new FieldMessageDialog() {DataContext = $"Edit Name of {SelectedRequirement.Name} "},
                "RequirementDialog",
                delegate(object sender, DialogClosingEventArgs args)
                {
                    bool result = false;
                    if (Equals(args.Parameter, false))
                    {
                        return;
                    }
                    if (args.Parameter is TextBox)
                    {
                        args.Session.UpdateContent(new PleaseWaitView());
                        TextBox txtName = (TextBox) args.Parameter;
                        string name = txtName.Text.Trim();

                        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                        {
                            args.Cancel();
                            args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                            return;
                        }
                        using (var context = new MorenoContext())
                        {
                            var duplicate = context.Requirements.FirstOrDefault(c => c.Name == name);
                            if (duplicate != null)
                            {
                                args.Cancel();
                                args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate Name" });
                                return;
                            }
                        }
                        //SelectedRequirement.Name = name;
                        Task.Run(() =>
                        {
                            try
                            {
                                //_context.Entry(SelectedRequirement).State = EntityState.Modified;

                                using (var context = new MorenoContext())
                                {
                                    var entity =
                                        context.Requirements.FirstOrDefault(c => c.Id == SelectedRequirement.Id);
                                    if (entity != null) entity.Name = name;
                                    context.SaveChanges();
                                }

                                //_context.SaveChanges();
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
                                args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Edit Failed"});
                            }
                            else
                            {
                                LoadData();
                            }
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
        }
    }
}