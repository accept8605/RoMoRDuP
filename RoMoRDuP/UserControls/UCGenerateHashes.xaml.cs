using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoMoRDuP.UserControls
{
    /// <summary>
    /// Interaktionslogik für UCGenerateHashes.xaml
    /// </summary>
    public partial class UCGenerateHashes : UserControl
    {
       Tasks.Tasks tasks;

        public UserInterface.UserInterfaceBase userOptionsBase { get; set; }

        public UCGenerateHashes(Tasks.Tasks tasks, UserInterface.UserInterfaceBase userOptionsBase)
        {

            this.tasks = tasks;

            this.userOptionsBase = userOptionsBase;

            InitializeComponent();
        }
    }
}
