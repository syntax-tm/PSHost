using System;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Interactivity;

namespace PSHost.Behaviors
{
    public class ScrollIntoViewBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            var itemsCollection = (INotifyCollectionChanged) AssociatedObject.Items;
            itemsCollection.CollectionChanged += OnListBox_CollectionChanged;
        }

        protected override void OnDetaching()
        {
            var itemsCollection = (INotifyCollectionChanged) AssociatedObject.Items;
            itemsCollection.CollectionChanged -= OnListBox_CollectionChanged;
        }

        private void OnListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var scrollPresenter = (ScrollContentPresenter) VisualTreeHelper.GetParent(AssociatedObject);
                var scrollViewer = scrollPresenter?.ScrollOwner;
                if (scrollViewer == null) return;
                scrollViewer.ScrollToEnd();
            }
        }
    }
}
