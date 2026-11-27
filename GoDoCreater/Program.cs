using System.Text.Json;

namespace GodoCreater {

    internal static class Program
    {
        [STAThread]
        static int Main()
        {
            ApplicationConfiguration.Initialize();
            
            string path=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "SeikoHelper","common.config");
            string json = File.ReadAllText(path);
            AppConfig config =
            JsonSerializer.Deserialize<AppConfig>(json)
            ?? throw new Exception("common.config の読み込みに失敗");
            GoDogen.ConnectionString = config.ConnectionString;
            GoDogen.EventNo = config.EventNo;
            string EventName= config.EventName;

            Application.Run(new MainMenu( EventName));
            return 0;
        }
        
           
    }
    public class AppConfig 
    {
        public string ConnectionString { get; set; }=string.Empty;
        public int EventNo { get; set; } = 0;
        public string EventName { get; set; } = string.Empty;

    }
}
