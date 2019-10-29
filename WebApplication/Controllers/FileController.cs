using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using System;
using WebApplication.Models;
using WebApplication.Controllers;
using WebApplication.Repository;
using WebApplication.IRepository;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace WebApplication {

    [Route("[controller]")]
    public class FileController : Controller {
        private readonly IAudioRepository _AudioRepository;
        private readonly IStationRepository _StationRepository;

        public class StationFile {
            public KeyValueAccumulator formAccumulator;
            public MemoryStream memoryStream;
            public StationFile() {
                formAccumulator = new KeyValueAccumulator();
                memoryStream = new MemoryStream();
            }
        }

        private readonly IFileProvider _fileProvider;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        public FileController(IFileProvider fileProvider, IAudioRepository AudioRepository, IStationRepository StationRepository) {
            _fileProvider = fileProvider;
             _AudioRepository=AudioRepository;
             _StationRepository=StationRepository;
        }

        public IActionResult Index() {
            return View();
        }
        
        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

        public async Task<StationFile> HandleMultipartRequest() {
            var stationFile = new StationFile();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null) {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        await section.Body.CopyToAsync(stationFile.memoryStream);
                        stationFile.memoryStream.Seek(0, SeekOrigin.Begin);
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            stationFile.formAccumulator.Append(key.Value, value);

                            if (stationFile.formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }
            return stationFile;
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile() {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var stationFile = await HandleMultipartRequest();
            var formData = stationFile.formAccumulator.GetResults();
            StringValues filename;
            bool ok = false;
            //Cuando se envian los audios se envia el APIKey con ese obtener el id y guardar en la base
            //APIKey para autenticar
            StringValues APIKey;
            StringValues Id;

            ok = formData.TryGetValue("APIKey", out APIKey);
            if (!ok) {
                return BadRequest("Expected APIKey key");
            }

            ok = formData.TryGetValue("Id", out Id);
            if (!ok) {
                return BadRequest("Expected ID key");
            }

            {
                string strStationId = "";
                int id;
                var StationResult=_StationRepository.Get(APIKey.ToString());
                var stationCount=_StationRepository.GetStationCount(APIKey);
                
                if(stationCount!=0){
                    id=StationResult.Result.Id;
                    strStationId=id.ToString(); //id 1 2 3
                    //string name=StationResult.Result.Name; //name folder with station name
                    
                    //Authentication
                    if(strStationId!=Id){
                        return BadRequest("Authentication Failed");
                    }

                    //other parameters
                    ok = formData.TryGetValue("filename", out filename);
                    if (!ok) {
                        return BadRequest("Expected filename key");
                    }
                    
                    StringValues recordingDate;
                    ok = formData.TryGetValue("RecordingDate", out recordingDate);
                    if (!ok) {
                        return BadRequest("Expected RecordingDate key");
                    }

                    StringValues duration;
                    ok = formData.TryGetValue("Duration", out duration);
                    if (!ok) {
                        return BadRequest("Expected Duration key");
                    }

                    StringValues format;
                    ok = formData.TryGetValue("Format", out format);
                    if (!ok) {
                        return BadRequest("Expected Format key");
                    }

                    var audio =new Audio();
                    try {
                        audio.RecordingDate = DateTime.ParseExact(recordingDate, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
                    } 
                    catch (Exception ex)
                    {
                        return BadRequest("Expected date to be in format 'dd/mm/yyyy hh:mm'");
                    }
                    audio.ArriveDate = DateTime.Now;
                    audio.StationId = id;
                    audio.Duration = duration;
                    audio.Format = format;
                    audio.LabelList = new List<String>();
                    await _AudioRepository.Add(audio);
                    var new_audio = await _AudioRepository.GetLastAudio();

                    string strfilename = "" + new_audio.Id + "." + new_audio.Format;

                    var filePath="";
                    if(strStationId!=null){
                        Core.MakeStationFolder(strStationId);
                        filePath = Path.Combine(Core.StationAudiosFolderPath(strStationId),
                                                    strfilename);
                        Console.Write(filePath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        await stationFile.memoryStream.CopyToAsync(stream);
                        stationFile.memoryStream.Close();
                    }
                    
                    { // Convert Decompressed File to ogg and add to playlist
                        var process = new Process();
                        process.StartInfo.FileName = "ffmpeg";
                        var fileInfo = new FileInfo(filePath);
                        var filenameNoExtension = fileInfo.Name.Remove((int)(fileInfo.Name.Length - fileInfo.Extension.Length));
                        // var milliseconds = long.Parse(filenameNoExtension);
                        // var date = DateTimeExtensions.DateTimeFromMilliseconds(milliseconds);
                        // var localDate = date.ToLocalTime();
                        var oggFilename = filenameNoExtension + ".ogg";
                        var oggFilePath = Path.Combine(Core.StationOggFolderPath(strStationId), oggFilename);
                        process.StartInfo.Arguments = "-i " + filePath + " " + oggFilePath;
                        process.Start();
                    }

                }
                else{
                    return Content("Invalid APIKEY");
                }                
            }
            return Content("File received");
        }
        
    }
}