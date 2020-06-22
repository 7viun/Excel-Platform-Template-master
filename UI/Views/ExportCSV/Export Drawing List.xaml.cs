using System.Threading.Tasks;
using System.Windows;
using UI.Commands.ExportCSV;
using UI.ViewModels;

namespace UI.Views.ExportCSV
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ExportDrawingListMainView : Window
    {
        private ExportCSVCommand _exportCSVCommand;
        private ExportCSVViewModel _exportCSVViewModel;
        public ExportDrawingListMainView(ExportCSVCommand exportCSVCommand)
        {
            _exportCSVCommand = exportCSVCommand;
            InitializeComponent();
            DataContext = _exportCSVViewModel = new ExportCSVViewModel(_exportCSVCommand);
        }
    }
}
