using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using DevExpress.CodeParser;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Common.Messages;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;
using MorenoSystem.Views.Vote.Admin;

namespace MorenoSystem.ViewModels.Vote.Admin
{
    public class PartyListViewModel : ViewModelBase
    {
        private MorenoContext _context;
        private bool _isOkMessageOpen;

        public PartyListViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
            //PartyLists = new List<PartyList>();
        }

        #region PartyListView

        public DelegateCommand AddNewPartyListCommand => new DelegateCommand(DoAddNewPartyList);

        private async void DoAddNewPartyList()
        {
            await DialogHost.Show(new FieldMessageDialog() {DataContext = "Add new Party List"}, "PartyListDialog",
                AddClosingEventHandler);
        }

        private void AddClosingEventHandler(object sender, DialogClosingEventArgs args)
        {
            bool result = false;
            if (Equals(args.Parameter, false))
            {
                return;
            }

            if (args.Parameter is TextBox)
            {
                args.Cancel();
                args.Session.UpdateContent(new PleaseWaitView());
                TextBox txtName = (TextBox) args.Parameter;
                string name = txtName.Text.Trim();
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    args.Cancel();
                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                    return;
                }

                var duplicate = _context.PartyLists.FirstOrDefault(c => c.Name == name);
                if (duplicate != null)
                {
                    args.Session.UpdateContent(new OkMessageDialog(){DataContext = "Duplicate name"});
                    return;
                }
                Task.Run(() =>
                {
                    var newPartyList = new PartyList()
                    {
                        Name = name
                    };
                    try
                    {
                        //_context.Entry(newPartyList).State = EntityState.Added;
                        _context.PartyLists.Add(newPartyList);

                        var cplist = _context.CouncilPositions;
                        var cmlist = new List<CouncilMembers>();
                        foreach (var cp in cplist)
                        {
                            var cm = new CouncilMembers
                            {
                                CouncilPosition = cp,
                                PartyList = newPartyList
                            };
                            cmlist.Add(cm);
                        }
                        foreach (var entity in cmlist)
                        {
                            //_context.Entry(entity).State = EntityState.Added;
                            _context.CouncilMembers.Add(entity);
                        }
                        _context.SaveChanges();
                        result = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);

                        result = false;
                    }
                    Thread.Sleep(500);
                    return newPartyList;
                }).ContinueWith(
                    (t, _) =>
                    {
                        if (result)
                        {
                            PartyLists.Add(t.Result);
                            RaisePropertyChanged(() => PartyLists);
                            args.Session.Close(false);
                        }
                        else
                        {
                            args.Session.UpdateContent(new OkMessageDialog() {DataContext = "Failed to add"});
                        }
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public DelegateCommand EditPartyListCommand => new DelegateCommand(DoEditPartyList);

        private async void DoEditPartyList()
        {
            if (SelectedPartyList != null)
            {
                await DialogHost.Show(
                    new FieldMessageDialog() {DataContext = $"Edit Name of {SelectedPartyList?.Name} Party List"},
                    "PartyListDialog", EditPartyListCEH);
            }
        }

        private void EditPartyListCEH(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, false))
            {
                return;
            }

            bool result = false;

            if (args.Parameter is TextBox)
            {
                args.Cancel();
                args.Session.UpdateContent(new PleaseWaitView());
                TextBox txtName = (TextBox) args.Parameter;
                string name = txtName.Text.Trim();
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    args.Cancel();
                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Null entry" });
                    return;
                }
                var duplicate = _context.PartyLists.FirstOrDefault(c => c.Name == name);
                if (duplicate != null)
                {
                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Duplicate name" });
                    return;
                }
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    try
                    {
                        SelectedPartyList.Name = name;
                        _context.Entry(SelectedPartyList).State = EntityState.Modified;
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
                    if (result)
                    {
                        LoadData();
                        args.Session.Close(false);
                    }
                    else
                    {
                        args.Session.UpdateContent(
                            new OkMessageDialog() {DataContext = "Edit Failed"});
                    }
                }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public DelegateCommand DeletePLCommand => new DelegateCommand(DoDeletePL);

        private async void DoDeletePL()
        {
            await DialogHost.Show(new OkCancelMessageDialog() {DataContext = $"Delete {SelectedPartyList.Name}?"},
                "PartyListDialog", DeleteClosingEventHandler);
        }

        private void DeleteClosingEventHandler(object sender, DialogClosingEventArgs args)
        {
            bool result = false;
            if (Equals(args.Parameter, false)) return;
            if (Equals(args.Parameter, "Cancel")) return;

            if (_isOkMessageOpen)
            {
                _isOkMessageOpen = false;
                return;
            }

            if (Equals(args.Parameter, "Ok"))
            {
                args.Cancel();
                args.Session.UpdateContent(new PleaseWaitView());
                Task.Run(() =>
                {
                    try
                    {
                        //_context.Entry(SelectedPartyList).State = EntityState.Deleted;
                        _context.PartyLists.Remove(SelectedPartyList);
                        foreach (var entity in CouncilMembersList)
                        {
                            //_context.Entry(entity).State = EntityState.Deleted;
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
                    Thread.Sleep(1000);
                    return result;
                }).ContinueWith(
                    (t, _) =>
                    {
                        if (!t.Result)
                        {
                            _isOkMessageOpen = true;
                            args.Session.UpdateContent(new OkMessageDialog()
                            {
                                DataContext = "Failed to Delete"
                            });
                        }
                        else
                        {
                            foreach (var member in CouncilMembersList.ToList())
                            {
                                CouncilMembersList.Remove(member);
                            }

                            PartyLists.Remove(SelectedPartyList);
                            RaisePropertyChanged(() => PartyLists);
                            //PartyLists = new List<PartyList>();
                            args.Session.Close(false);
                        }
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void GeneratePositionsForPartyList(PartyList partyList)
        {
            var cplist = _context.CouncilPositions.Select(c => c.Id);
            var ptlist = _context.PartyLists.Where(c => c.Id == partyList.Id);
        }

        private void LoadData()
        {
            PartyLists = _context.PartyLists.ToObservableCollection();
            CouncilMembersList = SelectedPartyList?.CouncilMembers.ToObservableCollection();
        }

        //private List<PartyList> _partyLists;
        public ObservableCollection<PartyList> PartyLists
        {
            get { return GetProperty(() => PartyLists); }
            set { SetProperty(() => PartyLists, value); }
        }

        public PartyList SelectedPartyList
        {
            get { return GetProperty(() => SelectedPartyList); }
            set
            {
                SetProperty(() => SelectedPartyList, value);
                if (value != null)
                {
                    var councilMembersId = value.CouncilMembers.Select(c => c.CouncilPosition.Id);
                    var listOfPosition = _context.CouncilPositions.Where(c => !councilMembersId.Contains(c.Id));
                    foreach (var position in listOfPosition)
                    {
                        var cm = new CouncilMembers
                        {
                            CouncilPosition = position,
                            PartyList = value,
                            Student = null
                        };
                        SelectedPartyList.CouncilMembers.Add(cm);
                    }
                    _context.SaveChanges();
                }
                CouncilMembersList = value?.CouncilMembers.ToObservableCollection();
            }
        }

        public ObservableCollection<CouncilMembers> CouncilMembersList
        {
            get { return GetProperty(() => CouncilMembersList); }
            set { SetProperty(() => CouncilMembersList, value); }
        }

        public CouncilMembers SelectedCouncilMember
        {
            get { return GetProperty(() => SelectedCouncilMember); }
            set { SetProperty(() => SelectedCouncilMember, value); }
        }

        #endregion

        #region MemberSelectionView

        public List<Student> StudentList
        {
            get { return GetProperty(() => StudentList); }
            set { SetProperty(() => StudentList, value); }
        }

        public Student SelectedStudent
        {
            get { return GetProperty(() => SelectedStudent); }
            set { SetProperty(() => SelectedStudent, value); }
        }

        public DelegateCommand ViewMemberCommand => new DelegateCommand(DoViewMember);

        private async void DoViewMember()
        {
            if (SelectedCouncilMember != null)
            {
                var councilMember = _context.CouncilMembers.Select(c => c.Student);
                _allStudent = _context.Students.Where(c => !councilMember.Contains(c)).ToList();
                StudentList = _allStudent;
                await DialogHost.Show(new MemberSelectionView() {DataContext = this}, "RootDialog",
                    MemberSelectionClosingEventHandler);
            }
        }

        public DelegateCommand DeleteMemberCommand => new DelegateCommand(DoDeleteMemberAsync);

        private async void DoDeleteMemberAsync()
        {
            await DialogHost.Show(new OkCancelMessageDialog() {DataContext = "Are you sure you want to DELETE?"},
                "RootDialog", DeleteMemberClosingEventHandler);
        }

        private void DeleteMemberClosingEventHandler(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, "Ok"))
            {
                SelectedCouncilMember.Student = null;
                _context.SaveChanges();
                LoadData();
            }
        }

        private void MemberSelectionClosingEventHandler(object sender, DialogClosingEventArgs args)
        {
            if (Equals(args.Parameter, true))
            {
                SelectedCouncilMember.Student = SelectedStudent;
                _context.SaveChanges();
                LoadData();
            }
        }

        private List<Student> _allStudent;

        public string SearchStudent
        {
            get { return GetProperty(() => SearchStudent); }
            set
            {
                SetProperty(() => SearchStudent, value);
                if (string.IsNullOrEmpty(value))
                {
                    StudentList = _allStudent;
                }
                else
                {
                    StudentList = _allStudent
                        .Where(c => c.FirstName.ToLower().Contains(value.ToLower()) ||
                                    c.MiddleName.ToLower().Contains(value.ToLower()) ||
                                    c.LastName.ToLower().Contains(value.ToLower()) ||
                                    c.Id.ToString().ToLower().Contains(value.ToLower())).ToList();
                }
            }
        }

        #endregion

        public bool CanStart
        {
            get
            {
                try
                {
                    return !_context.ElectionStatus.Any() && _context.PartyLists.Count() >= 2;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }

        public bool CanEnd
        {
            get
            {
                try
                {
                    return _context.ElectionStatus.Any();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }

        public DelegateCommand StartCommand => new DelegateCommand(DoStart, () => CanStart);

        private async void DoStart()
        {
            bool result = false;

            //var testAny = _context.PartyLists.Any();
            //if (testAny)
            //{
            await DialogHost.Show(new OkCancelMessageDialog() {DataContext = "Do you want to start the election?"},
                "RootDialog", delegate(object sender, DialogClosingEventArgs args)
                {
                    if (Equals(args.Parameter, "Ok"))
                    {
                        //args.Cancel();
                        try
                        {
                            var es = new ElectionStatus()
                            {
                                Start = true,
                                End = false
                            };
                            _context.ElectionStatus.Add(es);
                            _context.SaveChanges();
                            result = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            result = false;
                        }
                        args.Session.UpdateContent(new OkMessageDialog()
                        {
                            DataContext = result ? "Election Started" : "Failed to start"
                        });
                    }
                });
            //}
            //await DialogHost.Show(new OkMessageDialog() {DataContext = "No Candidates for Election"}, "RootDialog");
        }

        private bool IsEndDialog;
        public DelegateCommand EndCommand => new DelegateCommand(DoEnd, () => CanEnd);

        private async void DoEnd()
        {
            await DialogHost.Show(new OkCancelMessageDialog() {DataContext = "Do you want to end the election?"},
                "RootDialog", delegate(object sender, DialogClosingEventArgs args)
                {
                    if (Equals(args.Parameter, false))
                    {
                        return;
                    }
                    if (Equals(args.Parameter, "Ok") && IsEndDialog)
                    {
                        IsEndDialog = false;
                        return;
                    }

                    if (Equals(args.Parameter, "Ok") && !IsEndDialog)
                    {
                        IsEndDialog = true;
                        args.Cancel();
                        args.Session.UpdateContent(new PleaseWaitView());
                        bool result = false;
                        Task.Run(() =>
                        {

                            try
                            {
                                int votes = 0;
                                int cmvotes = 0;
                                //var cmMember = new CouncilMembers();
                                List<ElectionHistory> electHistory = new List<ElectionHistory>();
                                var positions = _context.CouncilPositions.ToList();
                                var members = _context.CouncilMembers.ToList();
                                var hasVotes = _context.StudentVotes.FirstOrDefault();
                                bool resultVotes = hasVotes != null;
                                //if (!positions.Any() && !members.Any())
                                if (!resultVotes)
                                {
                                    IsEndDialog = true;
                                    args.Session.UpdateContent(new OkMessageDialog() { DataContext = "Failed to End" });
                                    return;
                                }
                                //int highestVote = 0;
                                foreach (var position in positions)
                                {
                                    var cmMember = new CouncilMembers();
                                    foreach (var member in members)
                                    {
                                        if (member.CouncilPosition == position)
                                        {
                                            votes = CalculateVotes(member.Student);
                                            cmvotes = CalculateVotes(cmMember?.Student);
                                            if (votes > cmvotes)
                                            {
                                                //highestVote = votes;
                                                cmMember = member;
                                            }
                                        }
                                    }

                                    var eHistory = new ElectionHistory()
                                    {
                                        Position = cmMember.CouncilPosition.Position,
                                        Name = cmMember.Student.FullName,
                                        DateYear = DateTime.Now,
                                        VoteCount = CalculateVotes(cmMember?.Student)
                                };
                                    electHistory.Add(eHistory);
                                }
                                _context.ElectionHistory.AddRange(electHistory);
                                _context.CouncilMembers.RemoveRange(_context.CouncilMembers);
                                _context.PartyLists.RemoveRange(_context.PartyLists);
                                var list = _context.StudentVotes.ToList();
                                _context.StudentVotes.RemoveRange(list);
                                _context.ElectionStatus.RemoveRange(_context.ElectionStatus);
                                _context.SaveChangesAsync();
                                result = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                result = false;
                            }
                            
                        }).ContinueWith((t, _) =>
                        {
                            LoadData();
                            //args.Session.Close(false);
                            Messenger.Default.Send(new EndElectionMessage());
                            args.Session.UpdateContent(new OkMessageDialog(){DataContext = result ? "End Successfull, Check Election History for results." : "Failed to end"});
                        }, null, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                });
            //int votes = 0;
            //int cmvotes = 0;
            ////var cmMember = new CouncilMembers();
            //List<ElectionHistory> electHistory = new List<ElectionHistory>();
            //var positions = _context.CouncilPositions.ToList();
            //var members = _context.CouncilMembers.ToList();
            //if (!positions.Any() && !members.Any())
            //{
            //    await DialogHost.Show(new OkMessageDialog() {DataContext = "Failed to End"}, "RootDialog");
            //    return;
            //}
            //foreach (var position in positions)
            //{
            //    var cmMember = new CouncilMembers();
            //    foreach (var member in members)
            //    {
            //        if (member.CouncilPosition == position)
            //        {
            //            votes = CalculateVotes(member.Student);
            //            cmvotes = CalculateVotes(cmMember?.Student);
            //            if (votes > cmvotes)
            //            {
            //                cmMember = member;
            //            }
            //        }
            //    }

            //    var eHistory = new ElectionHistory()
            //    {
            //        Position = cmMember.CouncilPosition.Position,
            //        Name = cmMember.Student.FullName,
            //        DateYear = DateTime.Now,
            //        VoteCount = cmvotes
            //    };
            //    electHistory.Add(eHistory);
            //}
            //_context.ElectionHistory.AddRange(electHistory);
            //_context.CouncilMembers.RemoveRange(_context.CouncilMembers);
            //_context.PartyLists.RemoveRange(_context.PartyLists);
            //_context.StudentVotes.RemoveRange(_context.StudentVotes);
            //_context.ElectionStatus.RemoveRange(_context.ElectionStatus);
            //_context.SaveChanges();
            //LoadData();
            ////RaisePropertyChanged(() => StartCommand);
            ////RaisePropertyChanged(() => CanStart);

        }

        private int CalculateVotes(Student student)
        {
            try
            {
                return _context.StudentVotes.Count(c => c.VotedStudent.Id == student.Id);
                //var studentVotes = _context.StudentVotes.ToList();
                //foreach (var studentVote in studentVotes)
                //{
                //    studentVote.
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }
    }
}