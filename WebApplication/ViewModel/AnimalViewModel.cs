using WebApplication.Models;
using X.PagedList;

namespace WebApplication.ViewModel
{
    public class AnimalViewModel
    {
        public IPagedList<Animal> Animals { get; set; }
        public int Pnumber { get; set; }

    }
}