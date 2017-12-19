using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Common;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;
using MorenoSystem.Views.Vote.Voters;
using MorenoSystem.Common.Messages;

namespace MorenoSystem.ViewModels.Vote.Voters
{
    public class VoterProfileViewModel : ViewModelBase
    {
        private readonly MorenoContext _context;

        public VoterProfileViewModel(ref MorenoContext context, Student student)
        {
            _context = context;
            CurrentStudent = student;
            _studentVotes = new List<StudentVote>();
            
            CouncilPositions = _context.CouncilPositions.ToList();
            StudentVotes = new ObservableCollection<StudentVote>();
            AddRequirementsToStudent(student);
            Requirements = student?.RequirementStudents.ToObservableCollection();
            Messenger.Default.Register<SubmitVoteMessage>(this, OnSubmitVote);
        }

        public void AddRequirementsToStudent(Student student)
        {
            var listOfId = _context.RequirementStudents
                .Where(c => c.StudentId == student.Id)
                .Select(c => c.RequirementId);
            var listRequirement = _context.Requirements
                .Where(c => !listOfId.Contains(c.Id));
            if (!listRequirement.Any())
            {
                return;
            }
            foreach (var req in listRequirement)
            {
                var newReq = new RequirementStudents
                {
                    RequirementId = req.Id,
                    StudentId = student.Id,
                    Status = false,
                };
                _context.RequirementStudents.Add(newReq);
            }
            _context.SaveChanges();
            //using (var context = new MorenoContext())
            //{
            //    var listOfId = context.RequirementStudents
            //        .Where(c => c.StudentId == student.Id)
            //        .Select(c => c.RequirementId);
            //    var listRequirement = context.Requirements
            //        .Where(c => !listOfId.Contains(c.Id));
            //    if (!listRequirement.Any())
            //    {
            //        return;
            //    }
            //    foreach (var req in listRequirement)
            //    {
            //        var newReq = new RequirementStudents
            //        {
            //            RequirementId = req.Id,
            //            StudentId = student.Id,
            //            Status = false,
            //        };
            //        context.RequirementStudents.Add(newReq);
            //    }
            //    context.SaveChangesAsync();
            //}
        }

        private void OnSubmitVote(SubmitVoteMessage obj)
        {
            RaisePropertyChanged(() => VoteCommand);
        }

        public bool ShowPassword
        {
            get { return GetProperty(() => ShowPassword); }
            set { SetProperty(() => ShowPassword, value); }
        }

        public Student CurrentStudent
        {
            get { return GetProperty(() => CurrentStudent); }
            set { SetProperty(() => CurrentStudent, value); }
        }

        public User UserDetails
        {
            get { return CurrentStudent?.User; }
            set
            {
                SetProperty(() => CurrentStudent.User, value);
            }
        }

        public ObservableCollection<RequirementStudents> Requirements
        {
            get { return GetProperty(() => Requirements); }
            set { SetProperty(() => Requirements, value); }
        }

        public bool HasElection
        {
            get { return _context.ElectionStatus.Any(); }
        }
        public bool CanVote
        {
            get
            {
                try
                {
                    if (!HasElection)
                    {
                        return false;
                    }
                    int list = _context.StudentVotes.Count(c => c.Student.Id == CurrentStudent.Id);
                    if (list > 0)
                    {
                        return false;
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                }
                return true;
            }
        }
        public string VoteStatusMessage
        {
            get
            {
                if (HasElection)
                {
                    if (CanVote)
                    {
                        return "Vote now!";
                    }
                    return "You've already voted!";
                }
                return "No Election";
            }
        }

        public string NextOrFinish
        {
            get {
                if (StudentVotes?.Count == CouncilPositions.Count -1)
                {
                    return "Finish";
                }
                return "Next";
            }
            set { SetProperty(() => NextOrFinish, value); }
        }

        public DelegateCommand VoteCommand => new DelegateCommand(DoVote, () => CanVote);

        private void DoVote()
        {

            //await DialogHost.Show(new ElectionVoteView() {DataContext = this}, "RootDialog",
            //    delegate(object sender, DialogOpenedEventArgs args)
            //    {
            //        CurrentPosition = CouncilPositions.FirstOrDefault();
            //        CouncilMembersList = _context.CouncilMembers.AsNoTracking().Where(c => c.CouncilPosition.Id == CurrentPosition.Id)
            //            .ToList();
            //    },
            //    delegate(object sender, DialogClosingEventArgs args)
            //    {
            //        if (Equals(args.Parameter, false))
            //        {
            //            StudentVotes?.Clear();
            //            return;
            //        }
            //        if (Equals(args.Parameter, "Finish"))
            //        {
            //            args.Cancel();
            //            args.Session.UpdateContent(new OkMessageDialog(){DataContext = "OK FINISH"});
            //        }
            //    });
            CurrentPosition = CouncilPositions.FirstOrDefault();
            CouncilMembersList = _context.CouncilMembers.AsNoTracking().Where(c => c.CouncilPosition.Id == CurrentPosition.Id)
                .ToList();
            Messenger.Default.Send(new SetNavigation {Content = new ElectionVoteView {DataContext = new ElectionVoteViewModel(_context, CurrentStudent)}});
        }


        public ObservableCollection<StudentVote> StudentVotes
        {
            get { return GetProperty(() => StudentVotes); }
            set { SetProperty(() => StudentVotes, value); }
        }

        private List<StudentVote> _studentVotes;

        public DelegateCommand ShowNextCommand => new DelegateCommand(DoShowNext, () => SelectedCouncilMember != null);

        private void DoShowNext()
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
                 
        
            
            if (CouncilPositions.IndexOf(CurrentPosition) == CouncilPositions?.IndexOf(CouncilPositions.Last()))
            {
                DialogHost.CloseDialogCommand.Execute("Finish", null); 

                RaisePropertyChanged(() => VoteCommand);
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
            CouncilMembersList = _context.CouncilMembers.AsNoTracking().Where(c => c.CouncilPosition.Id == CurrentPosition.Id)
                .ToList();
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

        public bool CanPrev
        {
            get
            {
                if (CouncilPositions?.IndexOf(CurrentPosition) == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public DelegateCommand ShowPrevCommand => new DelegateCommand(DoShowPrev, () => CanPrev);

        private void DoShowPrev()
        {
            PreviousPosition();

            var student = StudentVotes?.FirstOrDefault(e => e.Position.Id == CurrentPosition.Id);


            if (student != null)
            {
                SelectedCouncilMember = _context.CouncilMembers.FirstOrDefault(c => c.Student.Id == student.VotedStudent.Id);
            }

            RaisePropertyChanged(() => NextOrFinish);
        }

        private void PreviousPosition()
        {
            CurrentPosition = CouncilPositions.PreviousOf(CurrentPosition);
            CouncilMembersList = _context.CouncilMembers.AsNoTracking().Where(c => c.CouncilPosition.Id == CurrentPosition.Id)
                .ToList();
        }

        public List<CouncilPosition> CouncilPositions
        {
            get { return GetProperty(() => CouncilPositions); }
            set { SetProperty(() => CouncilPositions, value); }
        }

        public CouncilPosition CurrentPosition
        {
            get { return GetProperty(() => CurrentPosition); }
            set { SetProperty(() => CurrentPosition, value); }
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
    }
}