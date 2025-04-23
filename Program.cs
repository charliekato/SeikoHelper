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
    }
    public static class GlobalV
    {
        public const string MagicWord = "\\SQLEXPRESS;Encrypt=True;TrustServerCertificate=True;"; public const string MagicHead = "Persist Security Info=False;User ID=Sw;Password=;Initial Catalog=Sw;Server=";
        public static int EventNo;
        public static string ServerName = string.Empty;
    }
}