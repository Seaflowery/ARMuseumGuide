namespace Util.Http
{
    /// <summary>
    /// HTTP 相关配置文件
    /// </summary>
    public static class HttpConfig
    {
        private static string sIp => "8.130.20.226";
        private static string sPort => "8000";


        public static string httpARMuseumText2Audio => "http://8.130.20.226:8000/ARMuseumAudio/";
        public static string httpARMuseumDB => "http://8.130.20.226:8000/ARMuseumDB/";
        public static string httpARMuseumFuzzy => "http://8.130.20.226:8000/ARMuseumFuzzy/";

    }
}