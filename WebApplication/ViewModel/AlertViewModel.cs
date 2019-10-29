using System;
using WebApplication.Models;
using X.PagedList;

namespace WebApplication.ViewModel
{
    public class AlertViewModel
    {
        public IPagedList<Alert> Alerts { get; set; }
        public int Pnumber { get; set; }
        public String FilterName { get;set; }
    }
}