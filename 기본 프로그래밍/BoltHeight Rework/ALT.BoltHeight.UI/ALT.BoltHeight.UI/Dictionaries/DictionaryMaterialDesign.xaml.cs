using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace ALT.BoltHeight.UI.Dictionaries
{
    public partial class DictionaryMaterialDesign
    {
        public void CheckFloat(object sender, TextCompositionEventArgs e)
        {

            Regex regex;

            if ((sender as TextBox).Text.Contains('.'))
                regex = new Regex(@"\d");
            else
                regex = new Regex(@"[\d\.]");

            if (!regex.IsMatch(e.Text))
                e.Handled = true;
        }

        public void CheckInteger(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d");

            if (!regex.IsMatch(e.Text))
                e.Handled = true;
        }
    }
}
