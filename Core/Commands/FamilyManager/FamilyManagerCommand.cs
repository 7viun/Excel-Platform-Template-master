using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;

namespace UserInterface.Commands.FamilyManager
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class FamilyManagerCommand:IExternalCommand
    {
        private Document _doc;
        public Document doc { get { return _doc; } }
        private Application _app;
        public Application app { get { return _app; } }
        private UIDocument _uidoc;
        public UIDocument uidoc { get { return _uidoc; } }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            _uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            _doc = uidoc.Document;
            var mainView = new FamilyManagerMainView();
            mainView.ShowDialog();
            return Result.Succeeded;
        }


        public static string GetPath()
        {
            return typeof(FamilyManagerCommand).Namespace + "." + nameof(FamilyManagerCommand);
        }
        public static string GetAssemblyCore()
        {
            return Assembly.GetExecutingAssembly().Location;
        }
    }
}
