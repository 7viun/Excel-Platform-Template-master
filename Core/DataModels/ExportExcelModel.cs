using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ListViewSchedule
    {
        public string listname { get; set; }
        public bool selecteditem { get; set; }
    }

    public struct ExcelValuePair
    {
        public string firstcell;
        public string lastcell;
        public string cellvalue;
    }
}

    

