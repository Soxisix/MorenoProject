using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Common.Messages;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;
using MorenoSystem.Views.Vote.Voters;

namespace MorenoSystem.ViewModels.Vote.Voters
{
    public class ElectionVoteViewModel:ViewModelBase
    {
        private  MorenoContext _context;
        private CouncilPosition _oldPosition;
        private bool _isVoteChange;
        private bool _changeOnly;

        public ElectionVoteViewModel(MorenoContext context, Student student)
        {
            _context = context;
            CurrentStudent = student;
            CouncilPositions = _context.CouncilPositions.ToList();
            CurrentPosition = CouncilPositions.FirstOrDefault();
            StudentVotes = new ObservableCollection<StudentVote>();
            IsNotFinish = true;
        }

        public DelegateCommand SubmitCommand => new DelegateCommand(DoSubmit, () => _changeOnly && !IsNotFinish);

        private async void DoSubmit()
        {
            
            bool result = false;
            await DialogHost.Show(new PleaseWaitView(), "RootDialog",
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    Task.Run(() =>
                    {

                        //try
                        //{
                            foreach (var sv in StudentVotes)
                            {
                                //_context.Entry(sv).State = EntityState.Added;
                                _context.StudentVotes.Add(sv);
                            }
                            _context.SaveChangesAsync();
                            result = true;//}
                        //catch (Exception e)
                        //{
                        //    Console.WriteLine(e);
                        //    result = false;
                        //}
                        Thread.Sleep(500);
                    }).ContinueWith((t, _) =>
                    {
                        Messenger.Default.Send(new SubmitVoteMessage());
                        Messenger.Default.Send(new SetNavigation() { Content = new VoterProfileView() { DataContext = new VoterProfileViewModel(ref _context, CurrentStudent) } });
                        args.Session.UpdateContent(new OkMessageDialog() { DataContext = result ? "Vote Success" : "Vote Failed, Please inform your teacher" });
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
                });
        }

        public DelegateCommand ChangeVoteCommand => new DelegateCommand(DoChangeVote);

        private void DoChangeVote()
        {
            IsNotFinish = true;
            _isVoteChange = true;
            _oldPosition = CurrentPosition;
            CurrentPosition = CouncilPositions.FirstOrDefault(c => c.Id == SelectedVote.Position.Id);
        }

        public DelegateCommand ShowNextCommand => new DelegateCommand(DoShowNext, () => SelectedCouncilMember != null);

        private void DoShowNext()
        {
            
            if (StudentVotes != null)
            {
                var studentVote = StudentVotes.FirstOrDefault(c => c.Position.Id == CurrentPosition.Id);
                if (studentVote != null)
                {
                    studentVote.VotedStudent = SelectedCouncilMember?.Student;
                    StudentVotes.Remove(studentVote);
                    AddVote();
                    StudentVotes = StudentVotes.OrderBy(c => c.Position.Id).ToObservableCollection();
                }
                else
                {
                    AddVote();
                }
            }
            else
            {
                AddVote();
            }

            if (_changeOnly)
            {
                IsNotFinish = false;
            }

            if (_isVoteChange)
            {
                _isVoteChange = false;
                CurrentPosition = _oldPosition;
                RaisePropertyChanged(() => NextOrFinish);
                return;
            }

            if (CouncilPositions.IndexOf(CurrentPosition) == CouncilPositions?.IndexOf(CouncilPositions.Last()))
            {
                IsNotFinish = false;
                _changeOnly = true;
            }
            else
            {
                NextPosition();
            }
            RaisePropertyChanged(() => NextOrFinish);
        }

        private void NextPosition()
        {
            CurrentPosition = CouncilPositions?.NextOf(CurrentPosition);
            //CouncilMembersList = _context.CouncilMembers.AsNoTracking().Where(c => c.CouncilPosition.Id == CurrentPosition.Id)
            //    .ToList();
        }

        private void AddVote()
        {
            var newStudentVote = new StudentVote
            {
                Position = CurrentPosition,
                VotedStudent = SelectedCouncilMember?.Student,
                Student = CurrentStudent,
                PartyList = SelectedCouncilMember?.PartyList
            };
            StudentVotes.Add(newStudentVote);
        }

        //public bool CanPrev
        //{
        //    get
        //    {
        //        if (CouncilPositions?.IndexOf(CurrentPosition) == 0)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //public DelegateCommand ShowPrevCommand => new DelegateCommand(DoShowPrev, () => CanPrev);

        //private void DoShowPrev()
        //{
        //    PreviousPosition();

        //    var student = StudentVotes?.FirstOrDefault(e => e.Position.Id == CurrentPosition.Id);


        //    if (student != null)
        //    {
        //        SelectedCouncilMember = _context.CouncilMembers.FirstOrDefault(c => c.Student.Id == student.VotedStudent.Id);
        //    }

        //    RaisePropertyChanged(() => NextOrFinish);
        //}

        //private void PreviousPosition()
        //{
        //    CurrentPosition = CouncilPositions.PreviousOf(CurrentPosition);
        //    CouncilMembersList = _context.CouncilMembers.AsNoTracking().Where(c => c.CouncilPosition.Id == CurrentPosition.Id)
        //        .ToList();
        //}

        public bool IsNotFinish
        {
            get { return GetProperty(() => IsNotFinish); }
            set { SetProperty(() => IsNotFinish, value); }
        }

        public List<CouncilPosition> CouncilPositions
        {
            get { return GetProperty(() => CouncilPositions); }
            set { SetProperty(() => CouncilPositions, value); }
        }

        public CouncilPosition CurrentPosition
        {
            get { return GetProperty(() => CurrentPosition); }
            set
            {
                SetProperty(() => CurrentPosition, value);
                CouncilMembersList = _context.CouncilMembers.Where(c => c.CouncilPosition.Id == CurrentPosition.Id)
                    .ToList();
            }
        }

        public List<CouncilMembers> CouncilMembersList
        {
            get { return GetProperty(() => CouncilMembersList); }
            set { SetProperty(() => CouncilMembersList, value); }
        }

        public CouncilMembers SelectedCouncilMember
        {
            get { return GetProperty(() => SelectedCouncilMember); }
            set { SetProperty(() => SelectedCouncilMember, value); }
        }
        public Student CurrentStudent
        {
            get { return GetProperty(() => CurrentStudent); }
            set { SetProperty(() => CurrentStudent, value); }
        }

        public string NextOrFinish
        {
            get
            {
                if (_changeOnly)
                {
                    return "Finish";
                }
                if (StudentVotes?.Count == CouncilPositions.Count - 1)
                {
                    return "Finish";
                }
                return "Next";
            }
            set { SetProperty(() => NextOrFinish, value); }
        }




        public ObservableCollection<StudentVote> StudentVotes
        {
            get { return GetProperty(() => StudentVotes); }
            set { SetProperty(() => StudentVotes, value); }
        }

        public StudentVote SelectedVote
        {
            get { return GetProperty(() => SelectedVote); }
            set { SetProperty(() => SelectedVote, value); }
        }
    }
}