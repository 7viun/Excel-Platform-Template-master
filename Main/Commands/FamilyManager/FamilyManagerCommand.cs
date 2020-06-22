using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using UserInterface.Views.FamilyManager;

namespace UserInterface.Commands.FamilyManager
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class FamilyManagerCommand:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var mainView = new FamilyManagerMainView();
            mainView.ShowDialog();
            return Result.Succeeded;
        }


        public static string GetPath()
        {
            return typeof(FamilyManagerCommand).Namespace + "." + nameof(FamilyManagerCommand);
        }
    }
}
