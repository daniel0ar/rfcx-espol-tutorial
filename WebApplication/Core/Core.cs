using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WebApplication.Models;
using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Mono.Unix.Native;

namespace WebApplication {
    public static class Core {

        public static Dictionary<string, int> StationDictionary;

        static Core() {
             StationDictionary = new Dictionary<string, int>();
        }

        public static string StationDictionaryPath { get {
            return Path.Combine(getServerDirectory() , FilesFolderName, "station_dictionary");
        }}

        public static void InitStationDictionaryFromFile() {
            if (File.Exists(StationDictionaryPath)) {
                var serializer = new JsonSerializer();
                using (var str = new StreamReader(StationDictionaryPath)) {
                    using (var jsonReader = new JsonTextReader(str)) {
                        var dic = serializer.Deserialize(jsonReader);
                        if (dic != null) {
                            StationDictionary = (Dictionary<string, int>)dic;
                        }
                    }
                }
            }
        }

        public static void SaveStationDictionaryToFile() {
            var serializer = new JsonSerializer();
                using (var str = new StreamWriter(StationDictionaryPath)) {
                    using (var jsonWriter = new JsonTextWriter(str)) {
                        serializer.Serialize(jsonWriter, StationDictionary);
                    }
                }
        }

        public static string FilesFolderName { get {return "files";} }

        public static string AudiosFolderPath { get {
            return Path.Combine(getServerDirectory() , FilesFolderName, "audios");
        }}

        public static string OggFolderPath { get {
            return Path.Combine(getServerDirectory() , FilesFolderName, "ogg");
        }}

        public static string FilesFolderPath { get { 
            return Path.Combine(getBPVAudioDirectory() , FilesFolderName);
        }}

        public static string SpeciesFolderPath { get {
            return Constants.RUTA_ARCHIVOS_IMAGENES_ESPECIES;
        }}

        public static void MakeFilesFolder() {
            if (!Directory.Exists(FilesFolderPath)) {
                Directory.CreateDirectory(FilesFolderPath);
                if (!Directory.Exists(AudiosFolderPath)) {
                    Directory.CreateDirectory(AudiosFolderPath);
                }
                if (!Directory.Exists(OggFolderPath)) {
                    Directory.CreateDirectory(OggFolderPath);
                }
            }
        }

        public static void MakeSpeciesFolder() {
            if (!Directory.Exists(SpeciesFolderPath)) {
                Directory.CreateDirectory(SpeciesFolderPath);
            }
        }

        public static string RecyclerFolderPath() {
            return Path.Combine(FilesFolderPath, "recycler");
        }

        public static string TemporaryFolderPath() {
            return Path.Combine(FilesFolderPath, "temp");
        }

        public static string StationFolderPath(string stationId) {
            return Path.Combine(getBPVAudioDirectory(), "station" + stationId);
        }

         public static string StationFolderPathImage(string stationId) {
            return Path.Combine(getBPVImagesDirectory(), "images");
        }

        public static string SpecieFolderPath(string specieId) {
            return Path.Combine(Constants.RUTA_ARCHIVOS_IMAGENES, "species/" + specieId + "/");
        }

        public static string StationFolderPathName(string name) {
            return Path.Combine(FilesFolderPath, name);
        }

        public static string StationFolder(string station) {
            return Path.Combine(FilesFolderPath, station);
        }

        public static string StationAudiosFolderPath(string stationId) {
            return Path.Combine(StationFolderPath(stationId), "audios");
        }

         public static string StationImagesFolderPath(string stationId) {
            return Path.Combine(StationFolderPathImage(stationId), stationId);
        }

        public static string StationOggFolderPath(string stationId) {
            return Path.Combine(StationFolderPath(stationId), "ogg");
        }

        public static string StationOggFolderPathImage(string stationId) {
            return Path.Combine(StationFolderPathImage(stationId), "ogg");
        }


        public static string StationAudiosFolderPathName(string name) {
            return Path.Combine(StationFolderPathName(name), "audios");
        }

        public static string StationOggFolderPathName(string name) {
            return Path.Combine(StationFolderPathName(name), "ogg");
        }
        
        public static void MakeRecyclerFolder() {
            var recyclerFolderPath = RecyclerFolderPath();
            if (!Directory.Exists(recyclerFolderPath)) {
                Directory.CreateDirectory(recyclerFolderPath);
            }
        }

        public static void MakeStationFolder(string stationId) {
            var stationFolderPath = StationFolderPath(stationId);
            if (!Directory.Exists(stationFolderPath)) {
                Directory.CreateDirectory(stationFolderPath);
                GrantAccess(stationFolderPath);
                Directory.CreateDirectory(StationAudiosFolderPath(stationId));
                GrantAccess(StationAudiosFolderPath(stationId));
                Directory.CreateDirectory(StationOggFolderPath(stationId));
                GrantAccess(StationOggFolderPath(stationId));
                File.Create(Path.Combine(stationFolderPath, Constants.PLAYLIST_FILE_NAME));
                GrantAccess(Path.Combine(stationFolderPath, Constants.PLAYLIST_FILE_NAME));
                string templateFile=Path.Combine(Constants.SERVER_ICECAST_CONFIG_DIRECTORY,Constants.TEMPLATE_ICECAST_CONFIG);

                string filename=Constants.TEMPLATE_ICECAST_CONFIG_FILENAME.Replace("0",stationId);

                string icecastConfigApp=Path.Combine(Constants.APP_ICECAST_CONFIG_DIRECTORY,filename);

                string icecastConfigServer=Path.Combine(Constants.SERVER_ICECAST_CONFIG_DIRECTORY, filename);

                File.Copy(templateFile, icecastConfigApp, true);
                File.Copy(templateFile,icecastConfigServer, true);

                string text = File.ReadAllText(icecastConfigApp);
                text = text.Replace(Constants.BASE_LINK, Constants.BASE_LINK.Replace("0",stationId));
                File.WriteAllText(icecastConfigApp, text);
                GrantAccess(icecastConfigApp);
                string text1 = File.ReadAllText(icecastConfigServer);
                text1 = text.Replace(Constants.BASE_LINK, Constants.BASE_LINK.Replace("0",stationId));
                File.WriteAllText(icecastConfigServer, text1);
                GrantAccess(icecastConfigServer);
            }
        }

        public static void MakeSpecieFolder(string specieId) {
            var specieFolderPath = SpecieFolderPath(specieId);
            if (!Directory.Exists(specieFolderPath)) {
                Directory.CreateDirectory(specieFolderPath);
                GrantAccess(specieFolderPath);
                string templateFile = Path.Combine(Constants.SERVER_ICECAST_CONFIG_DIRECTORY,Constants.TEMPLATE_ICECAST_CONFIG);
                string filename = Constants.TEMPLATE_ICECAST_CONFIG_FILENAME.Replace("0", specieId);
                string icecastConfigApp = Path.Combine(Constants.APP_ICECAST_CONFIG_DIRECTORY, filename);
                string icecastConfigServer = Path.Combine(Constants.SERVER_ICECAST_CONFIG_DIRECTORY, filename);
                File.Copy(templateFile, icecastConfigApp, true);
                File.Copy(templateFile, icecastConfigServer, true);
                string text = File.ReadAllText(icecastConfigApp);
                text = text.Replace(Constants.BASE_LINK, Constants.BASE_LINK.Replace("0", specieId));
                File.WriteAllText(icecastConfigApp, text);
                GrantAccess(icecastConfigApp);
                string text1 = File.ReadAllText(icecastConfigServer);
                text1 = text.Replace(Constants.BASE_LINK, Constants.BASE_LINK.Replace("0", specieId));
                File.WriteAllText(icecastConfigServer, text1);
                GrantAccess(icecastConfigServer);
            }
        }

        public static void MakeStationFolderName(string name){
            string name1=Path.Combine(FilesFolderPath,name);
            if(!Directory.Exists(name1)){
                Directory.CreateDirectory(name1);
                Directory.CreateDirectory(StationAudiosFolderPathName(name1));
                Directory.CreateDirectory(StationOggFolderPathName(name1));
            }
        }

        // /var/rfcx-espol-server/
        public static string getServerDirectory(){
            return Constants.SERVER_DIRECTORY;
        }

        // /var/rfcx-espol-server/resources/bpv/audios/
        public static string getBPVAudioDirectory() {
            return Constants.RUTA_ARCHIVOS_AUDIOS;
        }

        // /var/rfcx-espol-server/resources/
        public static string getBPVImagesDirectory() {
            return Constants.RUTA_ARCHIVOS;
            
        }
               
        private static void GrantAccess(string fullPath) {
            Syscall.chown(fullPath, Constants.USER_ID, Constants.GROUP_ID);//1000 es el UID y GID de estudiante (file, UID, GID)
            Syscall.chmod(fullPath, FilePermissions.ALLPERMS);
        }
        
    }

}