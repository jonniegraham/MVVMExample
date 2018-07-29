using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LinkTekTest.Utilities;

namespace LinkTekTest.Control
{
    internal class CustomButton : Button
    {
        public CustomButton()
        {
            Click += On_Click;
        }

        private void On_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
           Utils.FindVisualParent<Grid>(sender as Button).Focus();
        }
    }
}
