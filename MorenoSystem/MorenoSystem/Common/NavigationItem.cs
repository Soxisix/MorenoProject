using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;

namespace MorenoSystem.Common
{
    public class NavigationItem : ViewModelBase
    {
        
        private Thickness _marginRequirement = new Thickness(16);

        public NavigationItem(string name, object content, PackIconKind icon)
        {
            Name = name;
            Content = content;
            Icon = icon;
        }

        public string Name
        {
            get { return GetProperty(() => Name); }
            set { this.SetProperty(() => Name, value); }
        }

        public object Content
        {
            get { return GetProperty(() => Content); }
            set { this.SetProperty(() => Content, value); }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get { return GetProperty(() => HorizontalScrollBarVisibilityRequirement); }
            set { this.SetProperty(() => HorizontalScrollBarVisibilityRequirement, value); }
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get { return GetProperty(() => VerticalScrollBarVisibilityRequirement); }
            set { this.SetProperty(() => VerticalScrollBarVisibilityRequirement, value); }
        }

        public Thickness MarginRequirement
        {
            get { return GetProperty(() => MarginRequirement); }
            set { this.SetProperty(() => MarginRequirement, value); }
        }

        public PackIconKind Icon
        {
            get { return GetProperty(() => Icon); }
            set { SetProperty(() => Icon, value); }
        }
    }
}