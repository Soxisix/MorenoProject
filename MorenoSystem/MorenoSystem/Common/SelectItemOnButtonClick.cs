using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace MorenoSystem.Common
{
    public class SelectItemOnButtonClick : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Handler), true);
        }
        protected override void OnDetaching()
        {
            this.AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Handler));
            base.OnDetaching();
        }
        private void Handler(object s, RoutedEventArgs e)
        {
            object clicked = (e.OriginalSource as FrameworkElement)?.DataContext;
            var lbi = AssociatedObject.ItemContainerGenerator.ContainerFromItem(clicked) as ListBoxItem;
            if (lbi != null) lbi.IsSelected = true;
        }
    }
}