using DevExpress.Mvvm;
using MorenoSystem.MyEFContext;

namespace MorenoSystem.ViewModels.Students
{
    public class StudentAddViewModel : ViewModelBase
    {
        private readonly MorenoContext _context;
        public StudentAddViewModel(MorenoContext context)
        {
            _context = context;
        }
    }
}