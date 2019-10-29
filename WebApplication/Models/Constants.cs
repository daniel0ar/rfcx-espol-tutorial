namespace WebApplication.Models
{
    public class Constants
    {
        public const string APP_DIRECTORY="/home/estudiante/rfcx-espol-server/RfcxServer";
        public const string SERVER_DIRECTORY = "/var/rfcx-espol-server/";
        public const string BASE_LINK="station0";
        public const string BASE_URL="http://200.126.14.250/";
        public const string RUTA_ARCHIVOS = SERVER_DIRECTORY + "resources/";
        public const string RUTA_ARCHIVOS_BPV = RUTA_ARCHIVOS + "bpv/";
        public const string RUTA_ARCHIVOS_IMAGENES = RUTA_ARCHIVOS_BPV + "images/";
        public const string RUTA_ARCHIVOS_AUDIOS = RUTA_ARCHIVOS_BPV + "audios/";
        public const string RUTA_ARCHIVOS_IMAGENES_ESPECIES = RUTA_ARCHIVOS_IMAGENES + "species/";
        public const string SERVER_ICECAST_CONFIG_DIRECTORY="/var/rfcx-espol-server/icecast-config/";
        public const string TEMPLATE_ICECAST_CONFIG = "template/ices-playlist-0.xml";
        public const string APP_ICECAST_CONFIG_DIRECTORY= SERVER_DIRECTORY + "/icecast-config/";
        public const string TEMPLATE_ICECAST_CONFIG_FILENAME="ices-playlist-0.xml";
        public const string PLAYLIST_FILE_NAME="playlist.txt";
        public const string MONGO_CONNECTION = "mongodb://localhost:27017";
        public const string DEFAULT_DATABASE_NAME = "BosqueProtector";
        public const string RUTA_ARCHIVOS_ANALISIS_IMAGENES = RUTA_ARCHIVOS + "images/";
        public const int USER_ID=1000;
        public const int GROUP_ID=1000;
    }
}
