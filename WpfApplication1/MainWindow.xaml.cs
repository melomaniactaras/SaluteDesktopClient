using WpfApplication1.Common;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            var culture = CommonMethods.GetCulture();
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            InitializeComponent();
            W.DataContext = new ViewModels.MainWindowViewModel();         
        }
    }
}
