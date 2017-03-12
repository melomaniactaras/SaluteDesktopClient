using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Globalization;
using WpfApplication1.ViewModels;

//using OxyPlot.Wpf;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Charts.xaml
    /// </summary>
    public partial class Charts
    {
        public Charts()
        {
            InitializeComponent();
            DataContext = new ChartsViewModel();
        }
    }
}
