using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1.Common.Behaviors
{
    public class DataGridBehavior
    {
        public static readonly DependencyProperty RowNumbersProperty =
            DependencyProperty.RegisterAttached("RowNumbers", typeof(bool), typeof(DataGridBehavior),
            new FrameworkPropertyMetadata(false, OnRowNumbersChanged));

        private static void OnRowNumbersChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var grid = source as DataGrid;
            if (grid == null) return;
            if ((bool)args.NewValue)
            {
                grid.LoadingRow += onGridLoadingRow;
                grid.UnloadingRow -= onGridUnloadingRow;
            }
            else
            {
                grid.LoadingRow -= onGridLoadingRow;
                grid.UnloadingRow -= onGridUnloadingRow;
            }
        }

        private static void refreshDataGridRowNumber(object sender)
        {
            var grid = sender as DataGrid;
            if (grid == null) return;
            foreach (var row in grid.Items.Cast<object>().Select(item => (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(item)).Where(row => row != null))
            {
                row.Header = row.GetIndex() + 1;
            }
        }

        private static void onGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            refreshDataGridRowNumber(sender);
        }

        private static void onGridUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            refreshDataGridRowNumber(sender);
        }

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static void SetRowNumbers(DependencyObject element, bool value)
        {
            element.SetValue(RowNumbersProperty, value);
        }

        public static bool GetRowNumbers(DependencyObject element)
        {
            return (bool)element.GetValue(RowNumbersProperty);
        }
    }
}
