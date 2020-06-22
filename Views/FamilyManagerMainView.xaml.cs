using Core.ViewModels;
using System.Windows;

namespace Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FamilyManagerMainView : Window
    {
        public FamilyManagerMainView()
        {
            
            InitializeComponent();
            DataContext = new FamilyManagerMainPageViewModel();
        }
    }
}
