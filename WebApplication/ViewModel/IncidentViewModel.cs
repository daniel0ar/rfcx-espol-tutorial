using System;
using WebApplication.Models;
using X.PagedList;

namespace WebApplication.ViewModel
{
    public class IncidentViewModel
    {
        public IPagedList<Incident> Incidents { get; set; }
        public int Pnumber { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}