using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.FileProviders;
using WebApplication.ViewModel;
using System.IO;
using X.PagedList.Mvc.Core;
using X.PagedList;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    public class AudiosController : Controller
    {
        
        private readonly IAudioRepository _audioRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IFileProvider _fileProvider;

        public AudiosController(IAudioRepository audioRepository, IStationRepository stationRepository, IFileProvider fileProvider)
        {
            _audioRepository = audioRepository;
            _stationRepository = stationRepository;
            _fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            var audioVM = new AudioViewModel()
                {
                    Stations = _stationRepository.Get()
                };
            return View(audioVM);
        }

        [HttpPost]
        public IActionResult List(AudioViewModel audioVM)
        {
            var pageNumber = (audioVM.Pnumber == 0) ? 1 : audioVM.Pnumber;
            var pageSize = 10;
            audioVM.Stations = _stationRepository.Get();
            var audios = _audioRepository.GetByStationAndDate(audioVM.StationId, audioVM.Start, audioVM.End).ToPagedList(pageNumber, pageSize);
            audioVM.Audios = audios;
            return View(audioVM);
        }

        public FileResult DownloadFile(string namefile, string station)
        {
            string[] files = namefile.Split(',');
            if (files.Length == 1){
                DirectoryInfo DI = new DirectoryInfo(Core.StationAudiosFolderPath(station));
                string fileAddress = DI.FullName + '/' + namefile;
                var net = new System.Net.WebClient();
                var data = net.DownloadData(fileAddress);
                var content = new System.IO.MemoryStream(data);

                return File(content, "audio/mp4", namefile);
            } else {
                var directory = Core.StationAudiosFolderPath(station);
                string archive = Path.Combine(Core.getBPVAudioDirectory() + "files", "audios.zip");
                var temp = Core.TemporaryFolderPath();

                if (System.IO.File.Exists(archive))
                {
                    System.IO.File.Delete(archive);
                }

                Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

                foreach (var f in files)
                {
                    System.IO.File.Copy(Path.Combine(directory, f), Path.Combine(temp, f));
                }

                System.IO.Compression.ZipFile.CreateFromDirectory(temp, archive);

                return PhysicalFile(archive, "application/zip", "audios.zip");
            }
        }

        [HttpPut]
        public async Task<ActionResult> AddTag(int AudioId, string Tag)
        {
            await _audioRepository.AddTag(AudioId, Tag);
            return Content("Actualizado");
        }

        [HttpGet]
        [Route("api/[controller]")]
        public Task<string> Get()
        {
            return this.GetAudio();
        }

        private async Task<string> GetAudio()
        {
            var Audios= await _audioRepository.Get();
            return JsonConvert.SerializeObject(Audios);
        }

        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetAudioByIdInt(id);
        }

        private async Task<string> GetAudioByIdInt(int id)
        {
            var Audio= await _audioRepository.Get(id) ?? new Audio();
            return JsonConvert.SerializeObject(Audio);
        }

        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]")]
        public Task<string> GetAudiosByStation([FromRoute]int StationId)
        {
            return this.GetAudioByStation(StationId);
        }

        private async Task<string> GetAudioByStation(int StationId)
        {
            var Audios= await _audioRepository.GetByStation(StationId);
            return JsonConvert.SerializeObject(Audios);
        }

        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]/{AudioId:int}")]
        public Task<string> Get([FromRoute]int StationId, [FromRoute]int AudioId)
        {
            return this.GetAudioById(StationId, AudioId);
        }

        private async Task<string> GetAudioById(int StationId, int AudioId)
        {
            var Audio= await _audioRepository.Get(StationId, AudioId) ?? new Audio();
            return JsonConvert.SerializeObject(Audio);
        }

        [HttpPut]
        [Route("api/Station/{StationId:int}/[controller]/{AudioId:int}")]
        public async Task<bool> Put([FromRoute]int StationId, [FromRoute]int AudioId, [FromBody] Audio Audio)
        {
            if (AudioId==0) return false;
            return await _audioRepository.Update(StationId, AudioId, Audio);
        }

        //[HttpDelete("{id}")]
        [HttpDelete]
        [Route("[controller]/api/Station/{StationId:int}/Audio/{AudioId:int}")]
        public async Task<bool> Delete([FromRoute]int StationId, [FromRoute]int AudioId)
        {
            if (AudioId==0) return false;
            return await _audioRepository.Remove(StationId, AudioId);
        }
    }
}