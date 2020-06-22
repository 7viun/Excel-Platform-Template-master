using Autodesk.Revit.UI;
using ViewModels.ViewModels.Base;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Models;
using UserInterface.Commands.FamilyManager;

namespace ViewModels.ViewModels
{
    public class FamilyManagerMainPageViewModel : BaseViewModel
    {



        #region public properties
        public ApplicationPageType Currentpage { get; set; } = ApplicationPageType.Family;
        #endregion


        #region commands
        public ICommand FamilyBtnCommand { get; set; }

        public ICommand PreferencesBtnCommand { get; set; }

        #endregion

        #region private properties
        private static FamilyManagerCommand _familyManagerCommand;
        public static FamilyManagerCommand familyManagerCommand
        {
            get { return _familyManagerCommand; }
        }
        private UIDocument _uidoc;
        private Autodesk.Revit.ApplicationServices.Application _app;
        private Document _doc;
        #endregion

        #region constructor

        public FamilyManagerMainPageViewModel()
        {

            FamilyBtnCommand = new RelayCommand(FamilyBtnExec);
            PreferencesBtnCommand = new RelayCommand(PreferenceBtnExec);

        }

        #endregion

        #region private method
        private void FamilyBtnExec()
        {
            // test code to see if button command works.
            TaskDialog.Show("Test", Currentpage.ToString());
        }

        private void PreferenceBtnExec()
        {
            // test code to see if button command works.
            TaskDialog.Show("Test", "showed working preference button");
        }
        #endregion

    }
}
