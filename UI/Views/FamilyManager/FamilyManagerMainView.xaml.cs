using System.Windows;
using UI.Commands.FamilyManager;
using UI.ViewModels;

namespace UI.Views.FamilyManager
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FamilyManagerMainView : Window
    {
        private FamilyManagerCommand _familyManagerCommand;
        public FamilyManagerMainView(FamilyManagerCommand familyManagerCommand)
        {
            _familyManagerCommand = familyManagerCommand;
            InitializeComponent();
            DataContext = new FamilyManagerMainPageViewModel(_familyManagerCommand);

        }
    }
}
