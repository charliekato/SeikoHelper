using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeikoHelper
{
    public partial class MainMenu : Form
    {
        public MainMenu(string title)
        {
            InitializeComponent();
            this.Text = title;
        }


        private void checkBoxScenario3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxScenario3.Checked == true)
            {
                checkBoxScenario4.Checked = false;
            }

        }

        private void buttonExe_Click(object sender, EventArgs e)
        {
            string myMessage = "";
            GlobalV.Strategy1 = checkBoxStrategy1.Checked;
            GlobalV.Strategy2 = checkBoxStrategy2.Checked;
            GoDogen.PrepareGO();
            if (checkBoxScenario1.Checked)
            {
                GoDogen.Scenario1();
                myMessage = "下準備のレーン寄せ\n";
            }

            if (checkBoxScenario2.Checked)
            {
                GoDogen.Scenario2();
                myMessage += "合同レーステーブル作成\n";
            }

            if (checkBoxScenario3.Checked) {
                GoDogen.Scenario3(false);
                myMessage += "合同競技作成\n";
            }
            if (checkBoxScenario4.Checked)
            {
                GoDogen.Scenario3(true);
                myMessage += "プログラム用の合同競技作成\n";
            }
            if (checkBoxScenario5.Checked)
            {
                ProgramMerger.MergePrograms(GlobalV.EventNo);
                myMessage += "クラス無差別で競技再編成";
            }
            MessageBox.Show(myMessage + "終了");
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }

        private void checkBox4Pro_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxScenario4.Checked==true)
            checkBoxScenario3.Checked = false;
        }
    }
}
