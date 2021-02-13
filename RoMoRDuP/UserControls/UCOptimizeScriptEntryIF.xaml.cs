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
    /// Interaktionslogik für UCOptimizeScriptEntryFind.xaml
    /// </summary>
    public partial class UCOptimizeScriptEntryIF : UCOptimizeScriptEntryBase
    {
        public UCOptimizeScriptEntryIF()
        {
            InitializeComponent();
        }

        override public void ContractExpander()
        {
            Expander_Condition.IsExpanded = false;

        }

    }
}
