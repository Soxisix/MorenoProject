using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Common;

namespace MorenoSystem.ViewModels.Vote.Admin
{
    public class VoteResultViewModel : ViewModelBase
    {
        private MorenoContext _context;

        public VoteResultViewModel(ref MorenoContext context)
        {
            _context = context;
            CouncilPositions = _context.CouncilPositions.ToList();
        }

        public List<CouncilPosition> CouncilPositions
        {
            get { return GetProperty(() => CouncilPositions); }
            set { SetProperty(() => CouncilPositions, value); }
        }

        public CouncilPosition SelectedPosition
        {
            get { return GetProperty(() => SelectedPosition); }
            set { SetProperty(() => SelectedPosition, value, CalculateStudentVotes); }
        }

        public List<VoteStats> StudentVotes
        {
            get { return GetProperty(() => StudentVotes); }
            set { SetProperty(() => StudentVotes, value); }
        }

        private async void CalculateStudentVotes()
        {
            await DialogHost.Show(new PleaseWaitView(), "RootDialog",
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    Task.Run(() =>
                    {
                        var members = _context.CouncilMembers.Where(c => c.CouncilPosition.Position == SelectedPosition.Position).ToList();
                        var votestats = new List<VoteStats>();
                        int votes = 0;
                        foreach (var member in members)
                        {
                            try
                            {
                                votes = _context.StudentVotes.Count(c => c.VotedStudent.Id == member.Student.Id && c.VotedStudent != null);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            votestats.Add(new VoteStats()
                            {
                                Name = member?.Student?.FullName,
                                Count = votes
                            });
                        }
                        return votestats; 
                    }).ContinueWith((t, _) =>
                    {
                        StudentVotes = t.Result;
                        args.Session.Close();
                    }, null, TaskScheduler.FromCurrentSynchronizationContext());
                });
            
        }
    }
}