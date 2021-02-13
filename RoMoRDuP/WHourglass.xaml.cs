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
using System.Windows.Shapes;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace RoMoRDuP
{
    /// <summary>
    /// Interaktionslogik für WHourglass.xaml
    /// </summary>
    /// 
    public partial class WHourglass : Window
    {

        // Remove Close Button
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        public string Info { get; set; }

        public WHourglass(string Info)
        {
            this.Info = Info;

            InitializeComponent();
        }

        private void On_Loaded(object sender, RoutedEventArgs e) // Remove Close Button
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }


        // TBD prevent Alt+F4 - Set Cancel true OnClosing
    }
}
