using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SeikoHelper.SelectServer;

namespace SeikoHelper
{
    public partial class SelectEvent : Form
    {
        public SelectEvent()
        {
            InitializeComponent();
            this.ResumeLayout(true);
            ShowEventList();
        }
        public void ShowEventList()
        {
            string sqlQuery = "select * from 大会設定";
            listEvent.Items.Clear();

            listEvent.DrawMode = DrawMode.OwnerDrawFixed;
            listEvent.DrawItem += (sender, e) =>
            {
                e.DrawBackground();
                if (e.Index >= 0)
                {
                    string item = Convert.ToString(listEvent.Items[e.Index]) ?? ""  ;
                    string[] parts = item.Split('|'); // "|" 区切りでアイテムを分割

                    // カラムごとに描画
                    int x = e.Bounds.Left;
                    e.Graphics.DrawString(parts[0], e.Font, Brushes.Black, x, e.Bounds.Top);
                    x += 60; // カラムの位置調整
                    e.Graphics.DrawString(parts[1], e.Font, Brushes.Black, x, e.Bounds.Top);
                    x += 640;
                    e.Graphics.DrawString(parts[2], e.Font, Brushes.Black, x, e.Bounds.Top);
                    x += 530;
                    e.Graphics.DrawString(parts[3], e.Font, Brushes.Black, x, e.Bounds.Top);
                }
                e.DrawFocusRectangle();
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(GlobalV.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string eventNo =  Convert.ToString(reader["大会番号"]);
                                string eventName = (string) reader["大会名1"];
                                string eventVenue = (string) reader["開催地"];
                                string eventStart = (string) reader["始期間"];
                                string eventEnd = (string) reader["終期間"];
                                string eventDate = "";
                                if (eventStart == eventEnd)
                                {
                                    eventDate = Space(6) + eventStart + Space(6);
                                }
                                else
                                {
                                    eventDate = eventStart + "～" + eventEnd;
                                }
                                string showStr = myRight(eventNo, 3) + "|" +
                                    eventName + "|" +
                                    eventVenue + "|" +
                                    eventDate;
                                listEvent.Items.Add(showStr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("エラー [ ShowEvnetList ] \n" + ex.Message);
            }


        }
        private static string myRight(string input, int length)
        {
            string head = new string(' ', length);

            string untrim = head + input;
            return untrim.Substring(untrim.Length - length, length);

        }
        public static string Space(int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }
            return new string(' ', length);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var selectedItem = listEvent.SelectedItem;
            if (selectedItem != null)
            {
                string title = selectedItem.ToString();
                string[] parts = selectedItem.ToString().Split('|');
                GlobalV.EventNo = Int32.Parse(parts[0]);
                MainForm mainForm = new MainForm();
                GlobalV.EventName = parts[1];
                WinnerList.EventVenue= parts[2];
                WinnerList.EventDate = parts[3];
                mainForm.Text = title;
                mainForm.Show();
                //this.Hide();

            }
            else
            {
                MessageBox.Show("大会が選択されていません");
            }


        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
