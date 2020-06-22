using Autodesk.Revit.UI;
using UserInterface.ViewModels.Base;
using Models;
using System.Windows.Input;

namespace UserInterface.ViewModels
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
