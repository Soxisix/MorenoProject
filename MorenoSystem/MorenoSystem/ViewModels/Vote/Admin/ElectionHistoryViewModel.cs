using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using MorenoSystem.Common.Messages;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;

namespace MorenoSystem.ViewModels.Vote.Admin
{
    public class ElectionHistoryViewModel:ViewModelBase
    {
        private MorenoContext _context;

        public ElectionHistoryViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
            Messenger.Default.Register<EndElectionMessage>(this, EndElection);
        }

        private void EndElection(EndElectionMessage obj)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = _context.ElectionHistory.Select(c => c.DateYear.Year).ToList();
                DateYear = list.Distinct().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public int SelectedDateYear
        {
            get { return GetProperty(() => SelectedDateYear); }
            set { SetProperty(() => SelectedDateYear, value, OnDateYearChanged); }
        }

        public List<int> DateYear
        {
            get { return GetProperty(() => DateYear); }
            set { SetProperty(() => DateYear, value); }
        }

        private void OnDateYearChanged()
        {
            try
            {
                History = _context.ElectionHistory.Where(c => c.DateYear.Year == SelectedDateYear).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public List<ElectionHistory> History
        {
            get { return GetProperty(() => History); }
            set { SetProperty(() => History, value); }
        }
    }
}