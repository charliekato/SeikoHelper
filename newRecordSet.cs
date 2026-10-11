using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClosedXML.Excel;
using System.Diagnostics;


namespace SeikoHelper
{
    // 発生新記録のExcel出力
    // v発生新記録というview を作っておくこと。
    class NewRecordSet
    {
       static string myQuery = "select * from v発生新記録 where 大会番号=@eventNo";
        public static void CreateNewRecordEstablished(string fileName)
        {
            // 保存先のフルパス（任意で変更可）
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string folderPath = "C:\\Users\\user";
            string fullPath = Path.Combine(folderPath, fileName);

            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);

            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("発生大会記録");
                using (var conn = new SqlConnection(GlobalV.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(myQuery, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                        int excelRow = 2;
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                worksheet.Cell(excelRow, "A").Value = Convert.ToInt16(dr["No"]);
                                worksheet.Cell(excelRow, "B").Value = (string)dr["クラス名称"];
                                worksheet.Cell(excelRow, "C").Value = (string)dr["性別"];
                                worksheet.Cell(excelRow, "D").Value = (string)dr["距離"];
                                worksheet.Cell(excelRow, "E").Value = (string)dr["種目"];
                                worksheet.Cell(excelRow, "F").Value = (string)dr["ゴール"];
                                string swimmerName = (string)dr["第1泳者"];
                                string swimmerName2 = (string)dr["第2泳者"];
                                if (swimmerName2 != "")
                                {
                                    swimmerName += "\n" + swimmerName2;
                                    swimmerName += "\n" + (string)dr["第3泳者"];
                                    swimmerName += "\n" + (string)dr["第4泳者"];
                                }
                                worksheet.Cell(excelRow, "G").Value = swimmerName;
                                worksheet.Cell(excelRow, "H").Value = (string)dr["所属・チーム名"];
                                worksheet.Cell(excelRow, "I").Value = (string)dr["中間記録"];
                                worksheet.Cell(excelRow, "J").Value = (string)dr["新記録印刷マーク"];
                                worksheet.Cell(excelRow, "K").Value = (string)dr["中間新記録マーク"];
                                excelRow++;
                            }
                        } // dr
                    } //cmd
                    /*****/
                    // 列幅の調整
                    worksheet.Cell(1, "A").Value = "No";
                    worksheet.Cell(1, 2).Value = "クラス";
                    worksheet.Cell(1, 3).Value = "性別";
                    worksheet.Cell(1, 4).Value = "距離";
                    worksheet.Cell(1, 5).Value = "種目";
                    worksheet.Cell(1, 6).Value = "記録";
                    worksheet.Cell(1, 7).Value = "泳者";
                    worksheet.Cell(1, 8).Value = "所属・チーム名";
                    worksheet.Cell(1, 9).Value = "中間記録";
                    worksheet.Cell(1, 10).Value = "マーク";
                    worksheet.Cell(1, 11).Value = "マーク";

                    worksheet.Column("A").Width = 3.0;   // No
                    worksheet.Column("B").Width = 14.6;  // クラス
                    worksheet.Column("C").Width = 5.0;   // 性別（40px）
                    worksheet.Column("D").Width = 7.5;   // 距離
                    worksheet.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column("E").Width = 16;  // 種目

                    worksheet.Column("F").Width = 8.5;  // 記録
                    worksheet.Column("F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    worksheet.Column("G").Width = 12.8;  // 泳者
                    worksheet.Column("H").Width = 14;    // 所属

                    worksheet.Column(7).Style.Alignment.WrapText = true;
                    worksheet.Column("I").Width = 8.5;  // 中間記録
                    worksheet.Column("J").Width = 9.0;  // マーク
                    worksheet.Column("J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column("K").Width = 6.3;  // 中間記録マーク
                                                         // 

                    workbook.SaveAs(fullPath);
                    var result = MessageBox.Show(
                        $"Excelファイルを保存しました：\n{fullPath}\n\n開きますか？",
                        "完了",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        Process.Start("explorer.exe", $"\"{fullPath}\"");
                    }
                } // conn 
            }
        }
    }
}
