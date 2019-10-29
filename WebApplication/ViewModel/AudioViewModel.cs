using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication.Models;
using X.PagedList.Mvc;
using X.PagedList;

namespace WebApplication.ViewModel
{
    public class AudioViewModel
    {
        public IPagedList<Audio> Audios { get; set; }
        public IEnumerable<Station> Stations { get; set; }
        public int StationId { get; set; }

        [DisplayFormat(DataFormatString = @"{0:DD\/MM\/YYYY}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        
        [DisplayFormat(DataFormatString = @"{0:DD\/MM\/YYYY}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
        public int Pnumber { get; set; }
        public string FilePath = Core.getBPVAudioDirectory();
    }
}