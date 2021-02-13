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
    /// Interaktionslogik für UCTaskExecute.xaml
    /// </summary>
    public partial class UCTaskExecute : UserControl
    {

        public UserInterface.UserInterfaceBase userOptionsBase { get; set; }

        public UCTaskExecute(UserInterface.UserInterfaceBase userOptionsBase)
        {
            this.userOptionsBase = userOptionsBase;

            userOptionsBase.ScrollToEndTaskExecute = ScrollToEnd;

            InitializeComponent();
        }

        public void ScrollToEnd()
        {
            TextBox_TaskExecute.Focus();
            TextBox_TaskExecute.CaretIndex = TextBox_TaskExecute.Text.Length;
            TextBox_TaskExecute.ScrollToEnd();

        }
    }
}
