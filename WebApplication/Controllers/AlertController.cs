using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Linq;
using System;
using WebApplication.ViewModel;
using X.PagedList;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AlertController : Controller
    {

        private readonly IAlertRepository _AlertRepository;

        public AlertController(IAlertRepository AlertRepository)
        {
            _AlertRepository = AlertRepository;
        }

        [HttpGet("list")]
        public Task<string> Get()
        {
            return this.GetAlert();
        }

        private async Task<string> GetAlert()
        {
            var Alerts = await _AlertRepository.GetAllAlerts();
            return JsonConvert.SerializeObject(Alerts);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetAlertById(id);
        }

        private async Task<string> GetAlertById(string id)
        {
            var Alert = await _AlertRepository.GetAlert(id) ?? new Alert();
            return JsonConvert.SerializeObject(Alert);
        }


        [HttpPost]
        public IActionResult Post()
        {
            Alert alert = new Alert();
            List<Condition> condition_list = new List<Condition>();
            alert.Name = Request.Form["nombre_alerta"];
            alert.AlertType = Request.Form["tipo_alerta"];
            alert.Frecuency = Int32.Parse(Request.Form["frecuencia_alerta"]);
            string mails = Request.Form["correos_notificacion"];
            alert.Mailto = mails.Split(";").ToList();
            alert.Message = Request.Form["mensaje_alerta"];
            for (int i = 1; i <= Int32.Parse(Request.Form["conditions_number"]); i++)
            {
                Condition condition = new Condition();
                condition.StationId = Request.Form["estacion_alerta" + i.ToString()];
                condition.SensorId = Request.Form["sensor_alerta" + i.ToString()];
                condition.Comparison = Request.Form["condicion_alerta" + i.ToString()];
                condition.Threshold = Int32.Parse(Request.Form["threshold_alerta" + i.ToString()]);
                condition_list.Add(condition);
            }
            alert.Conditions = condition_list;
            bool result = _AlertRepository.Add(alert);
            setTempData(result, "createResult");
            return Redirect("index");
        }

        [HttpPost("{id}")]
        public IActionResult Post(string id)
        {
            Alert alert = new Alert();
            alert.AlertId = id;
            List<Condition> condition_list = new List<Condition>();
            alert.Name = Request.Form["nombre_alerta"];
            alert.AlertType = Request.Form["tipo_alerta"];
            string mails = Request.Form["correos_notificacion"];
            alert.Mailto = mails.Split(";").ToList();
            alert.Message = Request.Form["mensaje_alerta"];
            for (int i = 1; i <= Int32.Parse(Request.Form["conditions_number"]); i++)
            {
                Condition condition = new Condition();
                condition.StationId = Request.Form["estacion_alerta_" + i.ToString()];
                condition.SensorId = Request.Form["sensor_alerta_" + i.ToString()];
                condition.Comparison = Request.Form["condicion_alerta_" + i.ToString()];
                condition.Threshold = Int32.Parse(Request.Form["threshold_alerta_" + i.ToString()]);
                condition_list.Add(condition);
            }
            alert.Conditions = condition_list;
            _AlertRepository.UpdateAlert(id, alert);
            return Redirect("index");
            /* would work if method was async
            bool result = await _AlertRepository.UpdateAlert(id, alert);
            setTempData(result, "editResult");            
            Redirect("index");
             */

        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            bool result = await _AlertRepository.RemoveAlert(id);
            setTempData(result, "deleteResult");
            return result;
        }

        [HttpGet("{alertId}/condition/list")]
        public async Task<string> GetConditions(string alertId)
        {
            var Alert = await _AlertRepository.GetAlert(alertId) ?? new Alert();
            return JsonConvert.SerializeObject(Alert.Conditions);

        }
        [HttpGet("{alertId}/condition/{conditionId}")]
        public string GetCondition(string alertId, string conditionId)
        {
            Condition condition = _AlertRepository.getConditionObject(alertId, conditionId);

            return JsonConvert.SerializeObject(condition);

        }

        [HttpPatch("{alertId}/Status")]
        public async Task<bool> UpdateStatus(string alertId, [FromBody] Boolean Status)
        {
            bool result = await _AlertRepository.updateAlertStatus(alertId, Status);
            setTempData(result, "editResult");
            return result;
        }

        [HttpPatch("{alertId}/LastChecked")]
        public async Task<bool> UpdateLastChecked([FromRoute]string alertId, [FromBody] long LastChecked)
        {
            Console.WriteLine(LastChecked);
            bool result = await _AlertRepository.updateLastChecked(alertId, LastChecked);
            return result;
        }



        [HttpPatch("{alertId}/condition")]
        public async Task<bool> AddCondition(string alertId, [FromBody] Condition condition)
        {

            var Alert = await _AlertRepository.GetAlert(alertId) ?? new Alert();
            Alert.Conditions.Add(condition);
            return await _AlertRepository.UpdateAlert(alertId, Alert);
        }


        [HttpDelete("{alertId}/condition/{conditionId}")]
        public async Task<bool> DeleteCondition(string alertId, string conditionId)
        {
            if (string.IsNullOrEmpty(alertId))
            {
                return false;
            }
            var Alert = await _AlertRepository.GetAlert(alertId) ?? new Alert();
            int index = Alert.Conditions.FindIndex(x => x._id == ObjectId.Parse(conditionId));
            Alert.Conditions.RemoveAt(index);

            bool result = await _AlertRepository.UpdateAlert(alertId, Alert);
            setTempData(result, "deleteResult");
            return result;

        }

        [HttpPatch("{alertId}/condition/{conditionId}")]

        public async Task<bool> UpdateCondition(string alertId, string conditionId, [FromBody] Condition condition)
        {
            bool result = await _AlertRepository.editCondition(alertId, conditionId, condition);
            setTempData(result, "editResult");
            return result;

        }

        [HttpGet("index")]
        public IActionResult Index(AlertViewModel alertVM)
        {
            var pageNumber = (alertVM.Pnumber == 0) ? 1 : alertVM.Pnumber;
            var pageSize = 10;
            var alerts = _AlertRepository.GetAll().ToPagedList(pageNumber, pageSize);
            alertVM.Alerts = alerts;
            string[] variables = new string[]{"createResult","editResult","deleteResult"};
            initializeTempData(variables);
            return View(alertVM);
        }

        [HttpPost()]
        public IActionResult List(AlertViewModel alertVM)
        {
            var pageNumber = (alertVM.Pnumber == 0) ? 1 : alertVM.Pnumber;
            var pageSize = 10;
            var alerts = _AlertRepository.GetByName(alertVM.FilterName).ToPagedList(pageNumber, pageSize);
            alertVM.Alerts = alerts;
            string[] variables = new string[]{"createResult","editResult","deleteResult"};
            initializeTempData(variables);
            return View(alertVM);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("{id}/edit")]
        public IActionResult Edit(string id)
        {
            Alert alert = _AlertRepository.Get(id);
            return View(alert);
        }

        private void initializeTempData(string[] variables)
        {
            foreach (string variable in variables)
            {
                if (TempData[variable] == null)
                    TempData[variable] = 0;
            }
        }

        private void setTempData(bool result, string variable)
        {
            if (result)
                TempData[variable] = 1;
            else
                TempData[variable] = -1;
        }
    }
}
