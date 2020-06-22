using Autodesk.Revit.UI;
using UI.ViewModels.Base;
using System.Windows.Input;
using UI.Commands.ExportCSV;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using Core;
using System.Windows;
using WinForms = System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using UI.Utils;
using Resources;
using X = Microsoft.Office.Interop.Excel;
using System;
using System.Reflection;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using Microsoft.Win32;

namespace UI.ViewModels
{
    public class ExportCSVViewModel : BaseViewModel
    {
        #region commands
        public ICommand FamilyBtnCommand { get; set; }
        public ICommand ExportBtnCommand { get; set; }
        public ICommand ExportCbndBtnCommand { get; set; }
        public ICommand FindPathCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }


        #endregion
        #region Variable
        private ViewScheduleExportOptions opt = new ViewScheduleExportOptions();
        private static ExportCSVCommand _exportCSVCommand;
        public static ExportCSVCommand exportCSVCommand
        {
            get { return _exportCSVCommand; }
        }
        private UIDocument _uidoc;
        private Autodesk.Revit.ApplicationServices.Application _app;
        private Document _doc;
        private long _elapsedmilisec;
        public long ElapsedMilisec
        {
            get
            {
                return _elapsedmilisec;
            }
            set
            {
                _elapsedmilisec = value;
                OnPropertyChanged();
            }
        }
        private double _currentprogress;
        public double CurrentProgress
        {
            get
            {
                return _currentprogress;
            }
            set
            {
                if (_currentprogress != value)
                {
                    _currentprogress = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _selectedradiobutton;
        public bool Selectedradiobutton
        {
            get
            {
                return _selectedradiobutton;
            }
            set
            {
                if (_selectedradiobutton!=value)
                {
                    _selectedradiobutton = value;
                    OnPropertyChanged();
                }
            }
        }

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
        private string _projectname;
        public string ProjectName
        {
            get
            {
                return _projectname;
            }
            set
            {
                _projectname =  value;
                OnPropertyChanged();

            }
        }
        #endregion
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

        public ExportCSVViewModel(ExportCSVCommand exportCSVCommand)
        {
            _exportCSVCommand = exportCSVCommand;
            _uidoc = _exportCSVCommand.uidoc;
            _app = _exportCSVCommand.app;
            _doc = _exportCSVCommand.doc;
            _projectname = "Amazon";
            LoadCommand = new RelayCommand(GetWebPage);
            SaveCommand = new RelayCommand(SaveToART);
            ExportCbndBtnCommand = new RelayCommand(ExportCbnButton, CanClickButton);
            FindPathCommand = new RelayCommand(OpenFile);
            _filepath = "Please click \"Browse\" to choose Export Directory";
            GetAllViewSchedule();
        }

        #endregion

        #region Methods API
        public void GetAllViewSchedule()
        {
            FilteredElementCollector collector = new FilteredElementCollector(_doc);
            ElementClassFilter viewScheduleInstanceFilter = new ElementClassFilter(typeof(ViewSchedule));
            var viewschedules = collector.WherePasses(viewScheduleInstanceFilter).ToElements();
            IEnumerable<ViewSchedule> views = viewschedules.Cast<ViewSchedule>().Where(x => !x.IsTemplate).Where(y => !y.IsTitleblockRevisionSchedule).OrderBy(v =>v.Name);
            foreach (ViewSchedule viewSchedule in views)
            {
                _listViewSchedule.Add(new ListViewSchedule() { listname = viewSchedule.Name});
                _element.Add(viewSchedule);
            }
        }


        #region private method for Export Combined Button
        private async void ExportCbnButton()
        {
            if (_filepath == "Please click \"Browse\" to choose Export Directory")
            {
                MessageBox.Show("Choose Directory to Export First!", "Artelia VN", 0, MessageBoxImage.Warning);
            }
            else
            {
                X.Application excel = new X.Application();
                excel.Visible = false;
                X.Workbook workbook = excel.Workbooks.Add();
                X.Worksheet worksheet;
                excel.DisplayAlerts = false;
                bool _firstworksheet = true;
                int countvar = 0;
                //BackgroundWorker worker = new BackgroundWorker();
                //worker.WorkerReportsProgress = true;
                //worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                //worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                //worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerCompleted);
                //worker.RunWorkerAsync();
                foreach (var v in listView)
                {
                    if (v.selecteditem == true)
                    {
                        bool _success = false;
                        foreach (ViewSchedule i in _element)
                        {
                            if (i.Name == v.listname)
                            {
                                var table = i.GetTableData().GetSectionData(SectionType.Body);
                                var nRows = table.NumberOfRows;
                                var nColumns = table.NumberOfColumns;

                                #region Get the Row to Start the Data Table, not the Column Header
                                int countrow = 0;
                                for(int k=0; k<nRows; k++)
                                {
                                    var x = table.CanRemoveRow(k);
                                    if (x == true)
                                    {
                                        countrow = k;
                                        break;
                                    }
                                }
                                #endregion

                                #region Get Column Headers Data
                                List<ExcelValuePair> excelValuePairs = new List<ExcelValuePair>();
                                for (int xrow = 0; xrow < countrow; xrow++)
                                {
                                    int xcol = 0;
                                    do
                                    {
                                        int right = table.GetMergedCell(xrow, xcol).Right;
                                        int bottom = table.GetMergedCell(xrow, xcol).Bottom;

                                        if (!String.IsNullOrEmpty(i.GetCellText(SectionType.Body, xrow, xcol)))
                                        {
                                            excelValuePairs.Add(new ExcelValuePair
                                            {
                                                ///get first cell that contain value in merged cell in header
                                                firstcell = AviunUtils.ColumnIndexToColumnLetter(xcol+1) + (xrow+2).ToString() ,
                                                ///get last cell that merged
                                                lastcell = AviunUtils.ColumnIndexToColumnLetter(right+1) + (bottom+2).ToString(),
                                                ///get the actual value inside first cell
                                                cellvalue = i.GetCellText(SectionType.Body, xrow, xcol)
                                            });
                                        }
                                        
                                        xcol = right + 1;
                                    }
                                    while (xcol < nColumns);
                                }
                                #endregion


                                #region Add all Needed Data in Table to List
                                List<List<string>> dataListRow = new List<List<string>>();
                                int getdata = countrow;
                                for (int r= getdata; r < nRows; r= getdata)
                                {
                                    List<string> dataListColumn = new List<string>();
                                    for (int c = 0; c < nColumns; c++)
                                    {
                                        dataListColumn.Add(i.GetCellText(SectionType.Body, r, c));
                                    }
                                    getdata++;
                                    dataListRow.Add(dataListColumn);
                                }
                                _success = true;
                                #endregion
                                #region Create WorkSheet, Ignore the First Automated Sheet

                                if (_firstworksheet)
                                {
                                    worksheet = workbook.Sheets.get_Item(1) as X.Worksheet;
                                    _firstworksheet = false;
                                }
                                else
                                {
                                    worksheet = excel.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value) as X.Worksheet;
                                }
                                string name = i.Name;
                                name = (31 < name.Length) ? name.Substring(0, 31) : name;
                                name = name.Replace(":", "_").Replace("/", "_");
                                int countdup = 1;
                                try { worksheet.Name = name; }
                                catch { worksheet.Name = "Duplicated Name " + countdup.ToString();countdup++; }
                                var range = worksheet.get_Range("A1", "A1");
                                range.Font.Bold = true;
                                range.Interior.Color = X.XlRgbColor.rgbLightBlue;
                                range.Font.Color = X.XlRgbColor.rgbMidnightBlue;
                                #endregion
                                #region Start to Write Data to Excel File
                                worksheet.Cells[1, 1] = i.GetCellText(SectionType.Header, 0, 0);
                                foreach (List<string> st in dataListRow)
                                {
                                    int col = 1;
                                    foreach (string value in st)
                                    {
                                        worksheet.Cells[countrow+2, col] = value;
                                        var cellrange = worksheet.Range[worksheet.Cells[countrow + 2, col], worksheet.Cells[countrow + 2, col]];
                                        cellrange.Font.Color = X.XlRgbColor.rgbMidnightBlue;
                                        col++;
                                    }
                                    countrow++;
                                }
                                #endregion


                                #region Write Column Headers to Excel File
                                foreach (ExcelValuePair excelValuePair in excelValuePairs)
                                {
                                    worksheet.Range[excelValuePair.firstcell].Value = excelValuePair.cellvalue;
                                    X.Range thisrange = worksheet.Range[excelValuePair.firstcell, excelValuePair.lastcell];
                                    thisrange.Style.HorizontalAlignment = X.XlHAlign.xlHAlignCenter;
                                    thisrange.Style.VerticalAlignment = X.XlVAlign.xlVAlignCenter;
                                    thisrange.Font.Bold = true;
                                    thisrange.Font.Color = X.XlRgbColor.rgbMidnightBlue;
                                    thisrange.Interior.Color = X.XlRgbColor.rgbLightBlue;
                                    thisrange.Merge();
                                }
                                #endregion
                                #region Edit Excel Presentation after writing all data
                                X.Range last = worksheet.Cells.SpecialCells(X.XlCellType.xlCellTypeLastCell, Type.Missing);
                                int lastusedcol = worksheet.Cells.Find("*", System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, X.XlSearchOrder.xlByColumns, X.XlSearchDirection.xlPrevious, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;
                                int lastusedrow = last.Row;
                                string columnLetter = AviunUtils.ColumnIndexToColumnLetter(lastusedcol);
                                string lastcellfirstrow = columnLetter + "1";
                                worksheet.Range["A1"].Style.HorizontalAlignment = X.XlHAlign.xlHAlignCenter;
                                string lastcell = columnLetter + lastusedrow.ToString();
                                var rangex = worksheet.get_Range("A1", lastcell);
                                X.Borders borders = rangex.Borders;
                                borders[X.XlBordersIndex.xlInsideHorizontal].LineStyle = X.XlLineStyle.xlContinuous;
                                borders[X.XlBordersIndex.xlInsideVertical].LineStyle = X.XlLineStyle.xlContinuous;
                                borders[X.XlBordersIndex.xlEdgeBottom].LineStyle = X.XlLineStyle.xlContinuous;
                                borders[X.XlBordersIndex.xlEdgeLeft].LineStyle = X.XlLineStyle.xlContinuous;
                                borders[X.XlBordersIndex.xlEdgeRight].LineStyle = X.XlLineStyle.xlContinuous;
                                borders[X.XlBordersIndex.xlEdgeTop].LineStyle = X.XlLineStyle.xlContinuous;
                                var rangey = worksheet.get_Range("A1", lastcellfirstrow);
                                rangey.EntireColumn.AutoFit();
                                rangey.Merge();
                                #endregion
                                #region Progress Bar
                                countvar += 1;
                                var progress = new Progress<double>(percent =>
                                {
                                    CurrentProgress = percent * countvar;
                                });
                                await Task.Run(() => worker_DoWork(progress));
                                #endregion
                            }
                            if (_success == true) { break; }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                workbook.SaveAs(_filepath + "\\Exported - Combined Schedules.xlsx");
                excel.Quit();
                MessageBox.Show("Schedules Exported Successfully!", "Artelia VN", 0, MessageBoxImage.Information);
                CurrentProgress = 0;
            }
        }
        #endregion

        private bool CanClickButton()
        {
            int countvar = 0;
            bool _success = false;
            foreach (var v in listView)
            {
                if (v.selecteditem == true)
                {
                    countvar+=1;
                    _success = true;
                }
                if (_success == true) { break; }
            }
            var a = countvar;
            if (a != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void worker_DoWork(IProgress<double> progress)
        {
            double countvar = 0;
            Thread.Sleep(100);
            foreach (var v in listView)
            {
                if (v.selecteditem == true)
                {
                    countvar += 1;
                }
            }
            progress.Report(100 / countvar);
        }
        private void OpenFile()
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.DefaultExt = ".ART";
            dialog.Filter = "Artelia Revision File (*.ART)|*.ART";
            if (dialog.ShowDialog()==true)
            {
                string path = dialog.FileName;

                FilePath = path;
            }
        }
        private void SaveFile()
        {
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.FileName = "Revision Data";
            dialog.DefaultExt = ".ART";
            dialog.Filter = "Artelia Revision File (*.ART)|*.ART";
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                FilePath = path;
            }
        }
        #region Private Save Command
        private void SaveToART()
        {
            SaveFile();
            MessageBox.Show(_projectname);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            using (XmlWriter writer = XmlWriter.Create(_filepath,settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Phases");
                    writer.WriteStartElement("Phase");
                    writer.WriteAttributeString("Name", "Basic Design");
                        writer.WriteStartElement("Revision");
                            writer.WriteElementString("ID", "A");
                            writer.WriteElementString("Issue", "27/02/2012");
                            writer.WriteElementString("ProjectName", _projectname);
                            writer.WriteElementString("Location", "District 9, HCM City");
                            writer.WriteElementString("Title", "Drawing List");
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }
        #endregion
        private void GetWebPage()
        {
            /// This Part is to Get Data from an online source (because of github pages, we have to set security protocol to TLS12)
            //string address = "https://7viun.github.io/Data.xml";
            OpenFile();
            string xmlStr = _filepath;
            //using(var wc =new WebClient())
            //{
            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //    xmlStr = wc.DownloadString(address);
            //}
            ///Start of Reading XML Document from downloaded path of address
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlStr);
            XmlNodeList xnList = xmlDoc.SelectNodes("Phases/Phase");
            foreach (XmlNode xn in xnList)
            {
                foreach(XmlNode xn1 in xn.SelectNodes("Revision"))
                {
                    var revisions = xn1["Issue"].InnerText;
                    MessageBox.Show(revisions);

                }
                var nameofphase = xn.Attributes["Name"].InnerText;
                MessageBox.Show(nameofphase);
            }
        }
        #endregion

    }
}
