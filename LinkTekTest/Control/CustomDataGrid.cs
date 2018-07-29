using System.Windows.Controls;
using System.Windows.Input;
using LinkTekTest.Utilities;

namespace LinkTekTest.Control
{
    /// <summary>
    /// DataGrid with custom behavior.
    /// </summary>
    internal class CustomDataGrid : DataGrid
    {
        public CustomDataGrid()
        {
            MouseDown += EventSetter_OnHandler;
        }

        /// <summary>
        /// Row is selected irrespective of where on the datagrid row the mouse is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Border originalSource))
                return;

            var dataGridRow = Utils.FindVisualParent<DataGridRow>(originalSource);

            if (!(dataGridRow is DataGridRow row))
                return;

            row.IsSelected = true;
        }
    }
}
