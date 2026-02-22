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
    public partial class NewRecordManager : Form
    {
        public NewRecordManager()
        {
            InitializeComponent();
        }


        private void btnCompXls2DB_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog efd = new())
            {
                efd.Title = "Excelファイルを選択してください。";
                efd.Filter = "Excel ファイル(*.xlsx)|*.xlsx";
                efd.FilterIndex = 1;
                efd.RestoreDirectory = true;
                if (efd.ShowDialog() == DialogResult.OK) {
                    string excelFile = efd.FileName;
                    NewRecordExporter.CompareToExcel(GlobalV.ServerName, GlobalV.EventNo, excelFile);
                }
            }
        }

        private void btnGR2XLS_Click(object sender, EventArgs e)
        {
            Cursor previousCursor = Cursor.Current;

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
                    NewRecordSet.CreateNewRecordEstablished(filePath);
                }
            }
            finally
            {
                // カーソルを元に戻す
                Cursor.Current = previousCursor;
            }
        }

        private void btnTxtGen_Click(object sender, EventArgs e)
        {
            NewRecordExporter.DeleteBackup(); /* text file 出力*/
            NewRecordExporter.BackupDB();         // 新記録 table を 新記録bk にコピー
            NewRecordExporter.UpdateGameRecord(); //* 新記録bk tableを更新*/
            NewRecordExporter.UpdateGameRecord4RelayFirstSwimmer();  /* リレーの第一泳者　*/
            NewRecordExporter.InsertGameRecord(); //* 新記録bk tableに今まで記録がなかった種目に今回初めて泳者がいた。*/
            NewRecordExporter.ExeExport(); /* text file 出力*/
            NewRecordExporter.UpdateExcel(); /* text file 出力*/
            //NewRecordExporter.DeleteBackup(); /* text file 出力*/
        }

       
    }
}


