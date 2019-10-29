using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace WebApplication.ViewModel
{
    public class IndexModel
    {
        public IDirectoryContents Stations { get; set; }
        public IDirectoryContents Files { get; set; }
        public String selected { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public DateTime start_d { get; set; }
        public DateTime end_d { get; set; }

        public List<string> stationFolders { get; set; }
        public IFileInfo[] filesSorted { get; set; }
    }
}