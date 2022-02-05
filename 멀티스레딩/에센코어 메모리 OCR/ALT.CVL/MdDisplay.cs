using Cognex.VisionPro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ALT.CVL
{
    public class MdDisplay
    {
        #region Constructor
        public MdDisplay()
        {
        }
        #endregion

        #region Variables



        #endregion

        #region Properties
        public ObservableCollection<WindowsFormsHost> OCWfhDisplay { get; }
        #endregion

        #region Functions

        public ObservableCollection<WindowsFormsHost> GetOCWfhDisplay(int cameraCount)
        {
            ObservableCollection<WindowsFormsHost> displays = new ObservableCollection<WindowsFormsHost>();
            for (int i = 0; i < cameraCount; i++)
            {
                displays.Add(new WindowsFormsHost { Child = new UCCrDisplay() });
            }
            return displays;
        }

        
        #endregion

    }
}
