using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Text.Encodings.Web;
namespace WebApplication {

    public class StationViewController : Controller {
        
        private readonly ISensorRepository _SensorRepository;
        public StationViewController(ISensorRepository SensorRepository)
        {
            _SensorRepository = SensorRepository;
        }
        public IActionResult Index(string stationName, int stationId) {
            
            //retrieve sensors by station id
            //var sensors = _SensorRepository.GetByStationNotAsync(stationId);
            Console.WriteLine(stationName);
            ViewData["stationName"] =stationName;
            ViewData["stationId"]= stationId;           
            return View();
        }
               
    }

    
}