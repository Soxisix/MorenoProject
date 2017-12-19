using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.XtraReports;
using MorenoSystem.Common;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;
using MorenoSystem.ViewModels.Students;
using MorenoSystem.Views.Library.Report;
using MorenoSystem.Views.Student.Report;

namespace MorenoSystem.ViewModels.Library
{
    public class LibraryReportViewModel : ViewModelBase
    {
        private MorenoContext _context;

        public LibraryReportViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            ReportTypes = Enum.GetNames(typeof(LibraryReportType)).ToList();
            Categories = _context.Categories.ToObservableCollection();
            _books = _context.Books.OrderBy(c => c.Title).ToList();
        }

        private List<Book> _books;

        public bool IsCategoryVisible
        {
            get { return GetProperty(() => IsCategoryVisible); }
            set { SetProperty(() => IsCategoryVisible, value); }
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
                if (value == LibraryReportType.MasterList.ToString())
                {
                    var report = new BookListReport() { DataSource = _books };
                    ReportDocument = report;
                }
                if (value == LibraryReportType.ByCategory.ToString())
                {
                    IsCategoryVisible = true;
                }
                else
                {
                    IsCategoryVisible = false;
                }
                if (value == LibraryReportType.Damaged.ToString())
                {
                    var report = new BookDamageReport() { DataSource = _books.Where(c => c.Damaged > 0) };
                    ReportDocument = report;
                }
                if (value == LibraryReportType.Outdated.ToString())
                {
                    var report = new BookDamageReport() { DataSource = _books.Where(c => c.Outdated > 0) };
                    ReportDocument = report;
                }

            }
        }

        public ObservableCollection<Category> Categories
        {
            get { return GetProperty(() => Categories); }
            set { SetProperty(() => Categories, value); }
        }

        public Category SelectedCategory
        {
            get { return GetProperty(() => SelectedCategory); }
            set
            {
                SetProperty(() => SelectedCategory, value);
                if (value != null)
                {
                    var report = new BookCategoryReportcs() { DataSource = value.Books.OrderBy(c => c.Title) };
                    ReportDocument = report;
                }
            }
        }
    }

    public enum LibraryReportType
    {
        MasterList,
        ByCategory,
        Damaged,
        Outdated

        
    }
}