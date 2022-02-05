using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ALT.DS.UC
{
    public class SlideToggleButton : ToggleButton
    {
        private ResourceDictionary resourceDictionary;

        public SlideToggleButton() : base()
        {
            //resourceDictionary.Source = new Uri("pack://application:,,,/"+System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ";DSToggleButtonStyle.xaml", UriKind.Absolute);

            //Template = (resourceDictionary["toggleButtonTemplate"] as ControlTemplate);

            

            
        }
    }
}
