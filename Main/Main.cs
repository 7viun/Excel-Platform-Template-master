using Autodesk.Revit.UI;

namespace Main
{
    public class Main : IExternalApplication
    {
        #region external application public methods
        public Result OnStartup(UIControlledApplication application)
        {
            // Initialize whole plugin's user interface.
            var ui = new Interface();
            ui.Initialize(application);


            return Result.Succeeded;
        }


        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        #endregion


    }
}
