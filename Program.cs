namespace SeikoHelper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new SelectServer());
        }
        // application common configuration
    }
    // グローバル変数
    public static class GlobalV
    {
        public static AppConfig config = new AppConfig();
        //public static string ConnectionString = string.Empty;
        //public static int EventNo;
        //public static string EventName = string.Empty;

        public static string ServerName = string.Empty;
        public const string MagicWord = "\\SQLEXPRESS;Encrypt=True;TrustServerCertificate=True;";
        public const string MagicHead = "Persist Security Info=False;User ID=Sw;Password=";
        public const string MagicHead2 = ";Initial Catalog=Sw;Server=";
        //public static int MaxLaneNo4Yosen = 10;
        //public static int MaxLaneNo4Final = 10;
        //public static int MaxLaneNo4TimeFinal = 10;
        public static int ZeroLaneUse = 0;
        public static int MaxUID = 0;
        public static int MaxPrgNo = 0;
        public static int MaxKumi = 0;
        // プログラム　DB
        //public static int[] PrgNobyUID = [];
        //public static int[] ShumokubyPrgNo = [];
        //public static int[] DistancebyPrgNo = [];
        //public static int[] UIDFromPrgNo = [];
        //public static int[] ClassbyPrgNo = [];
        //public static int[] GenderbyPrgNo = [];
        //public static int[] NumSwimmers = [];
        //public static int[] NumSwimmersbyTID = [];
        //public static int[] TID = []; // togetherable ID

        public static int[] Phase = { };
        public static int GoDoClass; // 合同というクラスを新たに作る
        public static int raceNumber(int prgNo, int kumi)
        {
            return (prgNo - 1) * MaxKumi + kumi;
        }
    }
}
