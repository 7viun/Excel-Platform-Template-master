using Autodesk.Revit.UI;
using UI.ViewModels.Base;
using System.Windows.Input;
using UI.Commands.FamilyManager;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using Core;
using System.Windows;
using WinForms = System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace UI.ViewModels
{
    public class FamilyManagerMainPageViewModel : BaseViewModel
    {
        #region commands
        public ICommand FamilyBtnCommand { get; set; }
        public ICommand PreferencesBtnCommand { get; set; }
        public ICommand FindPathCommand { get; set; }


        #endregion
        private ViewScheduleExportOptions opt = new ViewScheduleExportOptions();

        private static FamilyManagerCommand _familyManagerCommand;
        public static FamilyManagerCommand familyManagerCommand
        {
            get { return _familyManagerCommand; }
        }
        private UIDocument _uidoc;
        private Autodesk.Revit.ApplicationServices.Application _app;
        private Document _doc;

        private string _filepath;
        public string FilePath
        {
            get
            {
                return _filepath;
            }
            set
            {
                _filepath =  value;
                OnPropertyChanged();

            }
        }

        #region ObservableCollection Collections

        private ObservableCollection<ViewSchedule> _element = new ObservableCollection<ViewSchedule>();
        private ObservableCollection<ListViewSchedule> _listViewSchedule = new ObservableCollection<ListViewSchedule>();
        public ObservableCollection<ListViewSchedule> listView
        {
            get
            {
                return _listViewSchedule;
            }
            set
            {
                _listViewSchedule = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region constructor

        public FamilyManagerMainPageViewModel(FamilyManagerCommand familyManagerCommand)
        {
            _familyManagerCommand = familyManagerCommand;
            _uidoc = _familyManagerCommand.uidoc;
            _app = _familyManagerCommand.app;
            _doc = _familyManagerCommand.doc;
            FamilyBtnCommand = new RelayCommand(FamilyBtnExec);
            PreferencesBtnCommand = new RelayCommand(PreferenceBtnExec);
            FindPathCommand = new RelayCommand(OpenFile);
            GetAllViewSchedule();
        }

        #endregion

        #region Methods API
        public void GetAllViewSchedule()
        {
            FilteredElementCollector collector = new FilteredElementCollector(_doc);
            ElementClassFilter viewScheduleInstanceFilter = new ElementClassFilter(typeof(ViewSchedule));
            var viewschedules = collector.WherePasses(viewScheduleInstanceFilter).ToElements();
            IEnumerable<ViewSchedule> views = viewschedules.Cast<ViewSchedule>().Where(x => !x.IsTemplate).Where(y => !y.IsTitleblockRevisionSchedule).OrderBy(v =>v.ViewName);
            foreach (ViewSchedule viewSchedule in views)
            {
                _listViewSchedule.Add(new ListViewSchedule() { listname = viewSchedule.ViewName});
                _element.Add(viewSchedule);
            }
        }
        public void ExportCSV(ListViewSchedule _listViewSchedule)
        {
      
            foreach (ViewSchedule i in _element)
            {
                    if (i.ViewName == _listViewSchedule.listname)
                    {
                        i.Export(_filepath, "Export " + i.ViewName + ".csv", opt);
                    }
            }
        }
        #endregion
        #region private method for Button
        private void FamilyBtnExec()
        {
        }

        private void PreferenceBtnExec()
        {
            try
            {
                foreach (var b in listView)
                {
                    if (b.selecteditem == true)
                    {
                        ExportCSV(b);
                    }
                    else
                    {
                        continue;
                    }
                }
                MessageBox.Show("All have been done");
            }
            catch
            {
                MessageBox.Show("Please choose the path to Export");
            }


        }

        private void OpenFile()
        {
            var dialog = new WinForms.FolderBrowserDialog();
            if(dialog.ShowDialog()==WinForms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                FilePath = path;
            }
        }
        #endregion

    }
}
