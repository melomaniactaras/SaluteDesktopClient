using WpfApplication1.ViewModels;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class MySettings
    {
        public MySettings()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}
