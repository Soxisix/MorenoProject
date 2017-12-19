using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using MorenoSystem.Common;
using MorenoSystem.Common.Messages;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext;

namespace MorenoSystem.ViewModels.Teachers
{
    public class AdviserViewModel : ViewModelBase
    {
        private MorenoContext _context;

        public AdviserViewModel(ref MorenoContext context)
        {
            _context = context;
            LoadData();
            Messenger.Default.Register<UpdateTeacherMessage>(this, OnUpdateRecieve);
        }

        private void OnUpdateRecieve(UpdateTeacherMessage obj)
        {
            LoadData();
        }

        private void LoadData()
        {
            Teachers = _context.Teachers.ToObservableCollection();
        }

        public ObservableCollection<Teacher> Teachers
        {
            get { return GetProperty(() => Teachers); }
            set { SetProperty(() => Teachers, value); }
        }

        public Teacher SelectedTeacher
        {
            get { return GetProperty(() => SelectedTeacher); }
            set
            {
                SetProperty(() => SelectedTeacher, value);
                if (value != null)
                {
                    OnSelectedTeacherChanged(value);
                }
            }
        }

        private void OnSelectedTeacherChanged(Teacher teacher)
        {
            AdviserSections = teacher.Sections.ToObservableCollection();
        }

        public string Search
        {
            get { return GetProperty(() => Search); }
            set
            {
                SetProperty(() => Search, value);
                OnSearchChanged(value);
            }
        }

        private void OnSearchChanged(string searchTxt)
        {
            int numberId = 0;
            bool result = false;
            result = Int32.TryParse(searchTxt, out numberId);

            if (result)
            {
                Teachers = _context.Teachers.Where(c => c.Id == numberId).ToObservableCollection();
            }
            else if (!string.IsNullOrEmpty(searchTxt))
            {
                Teachers = _context.Teachers
                    .Where(c => c.FirstName.ToLower().Contains(searchTxt.ToLower()) || c.MiddleName.ToLower().Contains(searchTxt.ToLower()) ||
                                c.LastName.ToLower().Contains(searchTxt.ToLower())).ToObservableCollection();
            }
            else if (string.IsNullOrEmpty(searchTxt))
            {
                Teachers = _context.Teachers.ToObservableCollection();
            }
        }

        public ObservableCollection<Section> Sections
        {
            get { return _context.Sections.Where(c => c.Teacher == null).ToObservableCollection(); }
        }

        public Section SelectedSection
        {
            get { return GetProperty(() => SelectedSection); }
            set { SetProperty(() => SelectedSection, value); }
        }

        public ObservableCollection<Section> AdviserSections
        {
            get { return GetProperty(() => AdviserSections); }
            set { SetProperty(() => AdviserSections, value); }
        }

        public Section SelectedAdviserSection
        {
            get { return GetProperty(() => SelectedAdviserSection); }
            set { SetProperty(() => SelectedAdviserSection, value); }
        }

        public DelegateCommand AddCommand => new DelegateCommand(DoAdd, () => SelectedSection != null);

        private  void DoAdd()
        {
            SelectedTeacher.Sections.Add(SelectedSection);
            _context.SaveChanges();
            OnSelectedTeacherChanged(SelectedTeacher);
            RaisePropertyChanged(() => Sections);
            //SelectedSection = null;
        }

        public DelegateCommand DeleteCommand => new DelegateCommand(DoDelete);

        private void DoDelete()
        {
            SelectedTeacher.Sections.Remove(SelectedAdviserSection);
            _context.SaveChanges();
            OnSelectedTeacherChanged(SelectedTeacher);
            RaisePropertyChanged(() => Sections);
        }
    }
}