using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.FileProviders;
using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using X.PagedList.Mvc.Core;
using X.PagedList;
using System.Linq;
using WebApplication.ViewModel;
using Newtonsoft.Json.Bson;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.IO.Compression;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using WebApplication.Controllers;
using WebApplication.Repository;
using System.Collections;
using Microsoft.Extensions.Primitives;
using System.Drawing;
using System.Net.Http;



namespace WebApplication.Controllers
{
    [Route("api/imgcapture")]
    public class ImageController : Controller
    {
        private readonly IImageRepository _ImageRepository;
        private readonly IStationRepository _StationRepository;

        private readonly IFileProvider _fileProvider;
        

        public ImageController(IImageRepository ImageRepository, IStationRepository StationRepository, IFileProvider fileProvider)
        {
            _ImageRepository=ImageRepository;
            _StationRepository = StationRepository;
            _fileProvider = fileProvider;
            

        }

        
        

        [HttpGet("index")]
        public IActionResult Index()
        {
            //ViewBag.estaciones = _StationRepository.Get();
            // List<Station> estaciones = _StationRepository.Get();
            IEnumerable<Station> station = _StationRepository.Get();
            ImageViewModel ivm = new ViewModel.ImageViewModel
            {
                estaciones = station
            };
            return View(ivm);
        }

        
        public IActionResult Search(ImageViewModel imageVM)
        {
            var pageNumber = (imageVM.Pnumber == 0) ? 1 : imageVM.Pnumber;
            var pageSize = 10;
            imageVM.estaciones = _StationRepository.Get();
            var imagenes = _ImageRepository.GetByStationAndDate(imageVM.StationId, imageVM.Start, imageVM.End).ToPagedList(pageNumber, pageSize);
            imageVM.Images = imagenes;
            return View(imageVM);
        }

        [HttpPost]
        public async Task<ActionResult> PostPicture(ImageRequest req)
        {
            return await _ImageRepository.PostPicture(req);

        }

        
        [HttpGet("download")]
        public FileResult download([FromQuery]string nameFile, [FromQuery]string station)
        {
            return File(Url.Content("C:\\var\\rfcx-espol-server\\resources\\images\\"+station+"\\"+nameFile), "image/jpg", "archivo.jpg");
            
        }



        [HttpGet("list")]
        public async Task<ActionResult> List([FromQuery]long starttime, [FromQuery]long endtime, [FromQuery]int page=1, [FromQuery]int rows=25)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime start = epoch.AddSeconds(starttime);
            DateTime end = epoch.AddSeconds(endtime);
            var arr = await _ImageRepository.ListImages(start, end, page, rows);
            return new ContentResult(){ Content = JsonConvert.SerializeObject(arr)};
        }

       
        

        //[HttpGet("{_id}")]
        //public async Task<ActionResult> Show(string _id)
        //{
        //    var image = await _ImageRepository.Find(_id);
        //    var imgPath = Constants.RUTA_ARCHIVOS_ANALISIS_IMAGENES + image.StationId + "/" +  image.Path;
        //    return base.PhysicalFile(imgPath, "image/"+Path.GetExtension(image.Path).Substring(1));
        //}

        [HttpPut]
        public async Task<ActionResult> AddTag(int ImageId, string Tag)
        {
            await _ImageRepository.AddTag(ImageId, Tag);
            return Content("Actualizado");
        }

        [HttpGet]
        [Route("api/[controller]")]
        public Task<string> Get()
        {
            return this.GetImage();
        }

        private async Task<string> GetImage()
        {
            var Imagenes= await _ImageRepository.Get();
            return JsonConvert.SerializeObject(Imagenes);
        }

        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetImageByIdInt(id);
        }

        private async Task<string> GetImageByIdInt(int id)
        {
            var Imagen= await _ImageRepository.Get(id) ?? new Image();
            return JsonConvert.SerializeObject(Imagen);
        }

        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]")]
        public Task<string> GetImagesByStation([FromRoute]int StationId)
        {
            return this.GetImageByStation(StationId);
        }

        private async Task<string> GetImageByStation(int StationId)
        {
            var Images= await _ImageRepository.GetByStation(StationId);
            return JsonConvert.SerializeObject(Images);
        }

        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]/{ImageId:int}")]
        public Task<string> Get([FromRoute]int StationId, [FromRoute]int ImageId)
        {
            return this.GetImageById(StationId, ImageId);
        }
        
        private async Task<string> GetImageById(int StationId, int ImageId)
        {
            var Image= await _ImageRepository.Get(StationId, ImageId) ?? new Image();
            return JsonConvert.SerializeObject(Image);
        }

        [HttpPut]
        [Route("api/Station/{StationId:int}/[controller]/{ImageId:int}")]
        public async Task<bool> Put([FromRoute]int StationId, [FromRoute]int ImageId, [FromBody] Image Image)
        {
            if (ImageId==0) return false;
            return await _ImageRepository.Update(StationId, ImageId, Image);
        }

       

        
    }
}