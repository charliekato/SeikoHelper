using System.Diagnostics;
using System.IO;
namespace SeikoHelper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        string homeDir = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile
        );

       
        private void GodoCreaterClick(object sender, EventArgs e)
        {
            string exePath = Path.Combine(
                homeDir, "bin",
                "GodoCreater",
                "GodoCreater.exe"
            );

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"{GlobalV.config.EventNo.ToString()} \"{GlobalV.config.EventName}\"",
                UseShellExecute = true
            });
        }
        private void lblExtractNewRecord_Click(object sender, EventArgs e)
        {
            NewRecordManager newRecordManager = new NewRecordManager();
            newRecordManager.Show();
        }


        private void labelEntryList_Click(object sender, EventArgs e)
        {
            string tateFlag="false";
            Cursor? previousCursor = Cursor.Current;
            if (rdbTateMochi.Checked) { tateFlag = "true"; }

            try
            {
                // 処理中カーソル（WaitCursor）を設定
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "保存先のファイルを選択してください";
                sfd.Filter = "エクセルファイル (*.xlsx)|*.xlsx|すべてのファイル (*.*)|*.*";
                sfd.DefaultExt = "xlsx";

                Cursor.Current = Cursors.WaitCursor;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;
                    var psi = new ProcessStartInfo 
                    {
                        FileName = Path.Combine( homeDir, "bin",
                                "CreateEntryList", "CreateEntryList.exe" ),
                        Arguments = $"{filePath} {(GlobalV.config.EventNo).ToString()} {tateFlag}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var process = Process.Start(psi);
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show("Errors in CreateEntryList");
                    }

                }
            }
            finally
            {
                // カーソルを元に戻す
                Cursor.Current = previousCursor;
            }

        }

        private void lblSCLASS_Click(object sender, EventArgs e)
        {
            RecordUpdater.GoUpdate();
        }

        private void labelWinnerList_Click(object sender, EventArgs e)
        {
            Cursor? previousCursor = Cursor.Current;
            // テンプレート選択
            MessageBox.Show("次の画面でテンプレート Excel を選択してください。", "テンプレート選択", MessageBoxButtons.OK, MessageBoxIcon.Information);
            string templatePath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "テンプレート Excel を選択";
                ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                templatePath = ofd.FileName;
            }
            try
            {
                // 処理中カーソル（WaitCursor）を設定
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "保存先のファイルを選択してください";
                sfd.Filter = "エクセルファイル (*.xlsx)|*.xlsx|すべてのファイル (*.*)|*.*";
                sfd.DefaultExt = "xlsx";

                Cursor.Current = Cursors.WaitCursor;
                //EntryList.CreateEntryList(textBoxEntryList.Text);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;
                    WinnerList.CreateWinnerList(GlobalV.ServerName, GlobalV.config.EventNo, templatePath, filePath);
                }
            }
            finally
            {
                // カーソルを元に戻す
                Cursor.Current = previousCursor;
            }

        }

        private void lblRunPaperSokuho_Click(object sender, EventArgs e)
        {

            PaperSokuho paperSokuho = new PaperSokuho();
            paperSokuho.Show();
        }
    }
}
