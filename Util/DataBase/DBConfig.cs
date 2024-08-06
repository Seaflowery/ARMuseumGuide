namespace Util.DataBase
{
    public class DBConfig
    {
        public static DBConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBConfig();
                    _instance.ReadConfig();
                }

                return _instance;
            }
        }

        private static DBConfig _instance;


        private string _sServer;
        private string _sPort;
        private string _sUser;
        private string _sPassword;
        private string _sDatabase;

        public string sConnection =>
            $"server={_sServer};port={_sPort};user={_sUser};password={_sPassword}; database={_sDatabase};";

        
        
        /// <summary>
        /// TODO 阅读配置文件表 -> 需要修改成读取文件配置
        /// </summary>
        private void ReadConfig()
        {
            _sServer = "cdb-omb6qd8s.cd.tencentcdb.com";
            _sPort = "10090";
            _sUser = "root";
            _sPassword = "20010922nyh";
            _sDatabase = "ARMuseum";
        }
    }
}