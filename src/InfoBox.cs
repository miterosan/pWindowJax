using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pWindowJax
{
    public partial class InfoBox : Form
    {
        public string Version { set => versionLabel.Text = value; }

        public InfoBox()
        {
            InitializeComponent();
        }
    }
}
