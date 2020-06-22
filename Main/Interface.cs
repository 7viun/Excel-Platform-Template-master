using Autodesk.Revit.UI;
using UI.Commands.ExportCSV;
using UI.Revit.Button;

namespace Main
{
    public class Interface
    {
        #region constructor

        public Interface()
        {

        }

        #endregion

        #region public methods

        /// <summary>
        /// Initializes all interface elements on custom created Revit tab.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Initialize(UIControlledApplication app)
        {
            // Create ribbon tab.
            string tabName = "Artelia VN";
            app.CreateRibbonTab(tabName);

            // Create the ribbon panels.
            var exportCommandsPanel = app.CreateRibbonPanel(tabName, "Export");

            #region manager

            var familyManagerShowButtonData = new RevitPushButtonDataModel
            {
                Label = "Export\nSchedules",
                Panel = exportCommandsPanel,
                Tooltip = "This command is used to export multiple Schedule\nat the same time",
                CommandNamespacePath = ExportCSVCommand.GetPath(),
                IconImageName = "MSExcel.ico",
                TooltipImageName = "MSExcel.ico"
            };
            var familyManagerShowButton = RevitPushButton.Create(familyManagerShowButtonData);

            #endregion
        }

        #endregion
    }
}
