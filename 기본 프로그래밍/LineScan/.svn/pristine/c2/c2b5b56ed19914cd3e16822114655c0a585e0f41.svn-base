using System.Text.RegularExpressions;
using System.Windows.Input;

namespace LineScanViewer.Dictionaries
{
    partial class DictionaryTextBox
    {
        public void CheckInteger(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d");

            if (!regex.IsMatch(e.Text))
                e.Handled = true;
        }
    }
}
