using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeikoHelper
{
    public partial class PaperSokuho : Form
    {
        public static string fileName = string.Empty;
        public PaperSokuho()
        {
            InitializeComponent();
        }

        string appDir = AppContext.BaseDirectory;

        string dataDir = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
            ),
            "SeikoHelper"
        );
        private void btnTemplatePath_Click(object sender, EventArgs e)
        {

            using var dialog = new OpenFileDialog();

            dialog.Title = "ファイルを選択してください";
            dialog.Filter = "Excelファイル (*.xlsx)|*.xlsx|すべてのファイル (*.*)|*.*";
            dialog.InitialDirectory = Path.Combine(dataDir, "templates");

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
                txtBoxTemplateFilePath.Text = fileName;
            }
        }

        private async void btnGo_Click(object sender, EventArgs e)
        {
            // 入力チェック
           
            if (!int.TryParse(txtBoxPrgNo.Text, out int prgNo))
            {
                MessageBox.Show("競技番号が正しくありません。");
                return;
            }

            string templateFile = txtBoxTemplateFilePath.Text;

            if (!File.Exists(templateFile))
            {
                MessageBox.Show("テンプレートファイルが指定されていません。");
                return;
            }

            // Python プログラム
            string pythonScript = Path.Combine(
                Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile
                        ),
                "bin",
                "PaperSokuho",
                "python",
                "create_ranking4inclusive.py"
            );

            if (!File.Exists(pythonScript))
            {
                MessageBox.Show($"Pythonプログラムが見つかりません。\n{pythonScript}");
                return;
            }

            // Python 実行
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

            psi.ArgumentList.Add(pythonScript);
            psi.ArgumentList.Add(GlobalV.config.EventNo.ToString());
            psi.ArgumentList.Add(prgNo.ToString());
            psi.ArgumentList.Add(templateFile);

            try
            {
                using var process = new Process();
                process.StartInfo = psi;

                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(output)
                            ? "ランキングを作成しました。"
                            : output,
                        "完了"
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"ランキング作成に失敗しました。\n\n{error}",
                        "エラー"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Python実行エラー");
            }

        }
    }
}
