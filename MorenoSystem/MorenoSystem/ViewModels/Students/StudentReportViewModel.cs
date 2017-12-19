using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.XtraReports;
using MorenoSystem.Common.Enums;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Student.Report;

namespace MorenoSystem.ViewModels.Students
{
    public class StudentReportViewModel : ViewModelBase
    {
        private MorenoContext _context;

        public StudentReportViewModel(ref MorenoContext context)
        {
            _context = context;

            //var report = new StudentListReport();
            ////report.DataSource =  _context.Students.ToList();
            //ReportDocument = report;
            //ReportTypes = Enum.GetNames(typeof(StudentReportType)).ToList();
            LoadData();
            
            
        }

        private List<Student> _allStudents;

        private void LoadData()
        {
            //ReportTypes = Enum.GetValues(typeof(StudentReportType)).Cast<StudentReportType>().ToList();
            ReportTypes = Enum.GetNames(typeof(StudentReportType)).ToList();

            
            //using (var context = new MorenoContext())
            //{
                _allStudents = _context.Students.OrderBy(c => c.LastName).ToList();
                YearLevels = _context.YearLevels.ToList();
            //}
        }

        public bool IsYearVisible
        {
            get { return GetProperty(() => IsYearVisible); }
            set { SetProperty(() => IsYearVisible, value); }
        }

        public bool IsSectionVisible
        {
            get { return GetProperty(() => IsSectionVisible); }
            set { SetProperty(() => IsSectionVisible, value); }
        }

        public IReport ReportDocument
        {
            get { return GetProperty(() => ReportDocument); }
            set
            {
                SetProperty(() => ReportDocument, value);
                RaisePropertyChanged(() => ReportDocument);
                ReportDocument.CreateDocument(true);
            }
        }

        public List<string> ReportTypes
        {
            get { return GetProperty(() => ReportTypes); }
            set { SetProperty(() => ReportTypes, value); }
        }

        public string SelectedReport
        {
            get { return GetProperty(() => SelectedReport); }
            set
            {
                SetProperty(() => SelectedReport, value);
                if (value == StudentReportType.Masterlist.ToString())
                {
                    var report = new StudentMasterListReport() {DataSource = _allStudents};
                    ReportDocument = report;
                }
                if (value == StudentReportType.BySection.ToString())
                {
                    IsYearVisible = true;
                    IsSectionVisible = true;
                    return;
                }
                else
                {
                    IsYearVisible = false;
                    IsSectionVisible = false;
                }
                SetProperty(() => SelectedReport, value);
                if (value == StudentReportType.ByYear.ToString())
                {

                    IsYearVisible = true;
                    return;
                }
                else
                {
                    IsYearVisible = false;
                }
            }
        }

        public List<YearLevel> YearLevels
        {
            get { return GetProperty(() => YearLevels); }
            set { SetProperty(() => YearLevels, value); }
        }

        public YearLevel SelectedYearLevel
        {
            get { return GetProperty(() => SelectedYearLevel); }
            set
            {
                SetProperty(() => SelectedYearLevel, value);
                if (value != null)
                {
                    Sections = value.Sections.ToList();
                }
                if (SelectedReport == StudentReportType.ByYear.ToString())
                {
                    if (value != null)
                    {
                        var report = new StudentYearReport() { DataSource = value.Students.OrderBy(c => c.LastName) };
                        ReportDocument = report;
                    }
                }
            }
        }

        public List<Section> Sections
        {
            get { return GetProperty(() => Sections); }
            set { SetProperty(() => Sections, value); }
        }

        public Section SelectedSection
        {
            get { return GetProperty(() => SelectedSection); }
            set
            {
                SetProperty(() => SelectedSection, value);
                if (SelectedReport == StudentReportType.BySection.ToString())
                {
                    if (SelectedSection != null)
                    {
                        var report = new StudentSectionReport() { DataSource = SelectedSection.Students.OrderBy(c => c.LastName) };
                        ReportDocument = report;
                    }
                }
            }
        }

        //public DelegateCommand ShowMasterListCommand => new DelegateCommand(DoShowMasterList);

        //private void DoShowMasterList()
        //{
        //    var report = new StudentListReport();
        //    //report.DataSource =  _context.Students.ToList();
        //    ReportDocument = report;
        //}
    }

    public enum StudentReportType
    {
        Masterlist,
        ByYear,
        BySection
    }
}