using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using DevExpress.DataProcessing;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;

namespace MorenoSystem.ViewModels.Vote.Admin
{
    public class ManagePositionViewModel : ViewModelBase
    {
        private MorenoContext _context;
        private bool _isOkMessageOpen;

        public ManagePositionViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            PositionList = _context.CouncilPositions.ToObservableCollection();
        }

        public DelegateCommand AddPositionCommand => new DelegateCommand(DoAddPosition);

        private async void DoAddPosition()
        {
            await DialogHost.Show(new FieldMessageDialog() {DataContext = "Add New Position"}, "PositionDialog",
                AddPositionClosing);
        }

        private void AddPositionClosing(object sender, DialogClosingEventArgs args)
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
                var newPosition = new CouncilPosition()
                {
                    Position = name
                };
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    args.Cancel();
                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                    return;
                }
                var duplicate = _context.CouncilPositions.FirstOrDefault(c => c.Position == name);
                if (duplicate != null)
                {
                    args.Cancel();
                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate Name" });
                    return;
                }
                Task.Run(() =>
                {
                    Thread.Sleep(1000);

                    try
                    {
                        _context.Entry(newPosition).State = EntityState.Added;

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
                        args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Failed to add"});
                    }
                    else
                    {
                        PositionList.Add(newPosition);
                    }
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public DelegateCommand DeletePositionCommad => new DelegateCommand(DoDeletePosition);

        private async void DoDeletePosition()
        {
            await DialogHost.Show(new OkCancelMessageDialog() {DataContext = $"Delete {SelectedPosition.Position}?"},
                "PositionDialog", DeletePositionClosing);
        }

        private void DeletePositionClosing(object sender, DialogClosingEventArgs args)
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
                    Thread.Sleep(1000);

                    try
                    {
                        _context.Entry(SelectedPosition).State = EntityState.Deleted;
                        var councilMembers =
                            _context.CouncilMembers.Where(c => c.CouncilPosition.Id == SelectedPosition.Id);
                        foreach (var entity in councilMembers)
                        {
                            _context.CouncilMembers.Remove(entity);
                        }
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
                    if (result)
                    {
                        PositionList.Remove(SelectedPosition);
                    }
                    else
                    {
                        args.Cancel();
                        _isOkMessageOpen = true;
                        args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Failed to Delete"});
                    }
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public DelegateCommand EditPositionCommand => new DelegateCommand(DoEditPosition);

        private async void DoEditPosition()
        {
            await DialogHost.Show(
                new FieldMessageDialog() {DataContext = $"Edit Name of {SelectedPosition.Position} Party List"},
                "PositionDialog", EditPositionClosing);
        }

        private void EditPositionClosing(object sender, DialogClosingEventArgs args)
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
                var duplicate = _context.CouncilPositions.FirstOrDefault(c => c.Position == name);
                if (duplicate != null)
                {
                    args.Cancel();
                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate Name" });
                    return;
                }
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    try
                    {
                        SelectedPosition.Position = name;
                        _context.Entry(SelectedPosition).State = EntityState.Modified;
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
                        args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Edit Failed"});
                    }
                    else
                    {
                        LoadData();
                    }
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public ObservableCollection<CouncilPosition> PositionList
        {
            get { return GetProperty(() => PositionList); }
            set { SetProperty(() => PositionList, value); }
        }

        public CouncilPosition SelectedPosition
        {
            get { return GetProperty(() => SelectedPosition); }
            set { SetProperty(() => SelectedPosition, value); }
        }
    }
}