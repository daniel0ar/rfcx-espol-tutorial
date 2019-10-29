using System;
using System.Collections.Generic;
using WebApplication.Models;
using System.ComponentModel.DataAnnotations;
using X.PagedList.Mvc;
using X.PagedList;

 
namespace WebApplication.ViewModel
{
    public class ImageViewModel
    {
        public IPagedList<Image> Images { get; set; }
        public Image Image { get; set; }

        public IEnumerable<Station> estaciones{ get; set; }    
        public int StationId { get; set; }

        [DisplayFormat(DataFormatString = @"{0:DD\/MM\/YYYY}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        
        [DisplayFormat(DataFormatString = @"{0:DD\/MM\/YYYY}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
        public int Pnumber { get; set; }
        public string FilePath = Core.getBPVImagesDirectory();
        
    }
}