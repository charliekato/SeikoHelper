using Microsoft.Data.SqlClient;
using System.Text.Json;
namespace SeikoHelper
{
    public partial class SelectServer : Form
    {
        public SelectServer()
        {
            InitializeComponent();
        }
        private void SelectServer_Load(object sender,EventArgs e)
        {
            tbxServerName.Focus();
        }
        private void tbxServerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }
        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbxServerName.Text == "localhost")
            {
                GlobalV.ConnectionString = "Server=localhost\\SQLEXPRESS; Database = Sw; Integrated Security = True;TrustServerCertificate=True; ";

            }
            else
            {

                GlobalV.ConnectionString = GlobalV.MagicHead+tbxPassword.Text + GlobalV.MagicHead2+ tbxServerName.Text + GlobalV.MagicWord;
            }
            if (ServerAccessOK())
            {
                SaveConfig();
                SelectEvent selEventForm = new SelectEvent();
                selEventForm.Show();
            }
            else
            {
                MessageBox.Show("Could not access the server.", "!!ERROR!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static private void SaveConfig() {
            var config = new AppConfig {
                ConStr=GlobalV.ConnectionString            
            };

            string path = Path.Combine(
                Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData
                        ),
                    "SeikoHelper",
                    "config.json"
                    );
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            //JSONÇ…ïœä∑ÇµÇƒï€ë∂
            var options = new JsonSerializerOptions {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(path, json);
          
        }
        static public bool ServerAccessOK()
        {
            string sqlQuery = "select * from ëÂâÔê›íË";

            try
            {
                using (SqlConnection connection = new SqlConnection(GlobalV.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
		MessageBox.Show("error. : " + ex.Message);
                return false;
            }
        }

    }
}
