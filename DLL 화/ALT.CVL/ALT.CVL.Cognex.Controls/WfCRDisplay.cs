using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALT.CVL.Core
{
    public partial class WfCRDisplay : UserControl
    {
        public WfCRDisplay()
        {
            InitializeComponent();
            Display = cogRecordDisplay1;
        }

        public CogRecordDisplay Display { get; private set; }
    }
}
