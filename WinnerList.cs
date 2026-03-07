using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;

namespace SeikoHelper
{
    public static class ExcelPhoneticCleaner
    {
        public static void RemovePhoneticRuns(string filePath)
        {
            using (var archive = ZipFile.Open(filePath, ZipArchiveMode.Update))
            {
                var entry = archive.GetEntry("xl/sharedStrings.xml");
                if (entry == null)
                    return;

                XDocument doc;
                using (var stream = entry.Open())
                {
                    doc = XDocument.Load(stream);
                }

                XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                // <rPh> 削除
                var rPhNodes = doc.Descendants(ns + "rPh").ToList();
                foreach (var node in rPhNodes)
                    node.Remove();

                // <phoneticPr> 削除
                var phoneticPrNodes = doc.Descendants(ns + "phoneticPr").ToList();
                foreach (var node in phoneticPrNodes)
                    node.Remove();

                // 上書き保存
                entry.Delete();
                var newEntry = archive.CreateEntry("xl/sharedStrings.xml");

                using (var stream = newEntry.Open())
                {
                    doc.Save(stream);
                }
            }
        }
        }
    class WinnerList
    {
        public static string EventDate = string.Empty;
        public static string EventVenue = string.Empty;
        public static bool TrySetValue(XLWorkbook wb, string name, string value)
        {
            //if (wb.NamedRanges.TryGetValue(name, out var nr))
            if (wb.DefinedNames.TryGetValue(name, out var nr))
            {
                nr.Ranges.First().Value = value;
                return true;
            }
            return false;
        }
        public static void SetMark(XLWorkbook wb, string name, XLColor color)
        {
            if (wb.DefinedNames.TryGetValue(name, out var nr))
            {
                var firstCell = nr.Ranges.First();
                firstCell.Style.Fill.BackgroundColor=color;
            }
        }
        public static string TryGetValue(XLWorkbook wb, string name)
        {
            if (wb.DefinedNames.TryGetValue(name, out var nr))
            {
                var firstRange = nr.Ranges.First();
                var firstCell = firstRange.FirstCell();
                return firstCell.GetValue<string>();
            }
            return "";
        }


        
        public static void CreateWinnerList(string serverName, int eventNo,string templatePath, string fileName)
        {
           
            // 保存先のフルパス（任意で変更可）
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string folderPath = "C:\\Users\\user";
            string fullPath = Path.Combine(folderPath, fileName);

            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);

            string query = "select * from vランキング　where 大会番号=@eventNo and 順位=1";


            using (var con = new SqlConnection(GlobalV.ConnectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                con.Open();
                var dt = new DataTable();
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                // ClosedXMLでExcelファイル作成

                ExcelPhoneticCleaner.RemovePhoneticRuns(templatePath);

                using (var wb = new XLWorkbook(templatePath))
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        int classNo = Convert.ToInt32(dr["新記録判定クラス"] );
                        
                        string gender = Convert.ToString(dr["性別"]) ?? "";
                        string style = Convert.ToString(dr["種目コード"]) ?? "";
                        string distance = Convert.ToString(dr["距離コード"]) ?? "";
                        
                        string rangeName=gender +classNo+"_"+ distance +"_"+ style;
                        TrySetValue(wb, rangeName+"Time",(string)dr["ゴール"]);
                        string name = (string)dr["第1泳者"];
                        string name2 = (string)dr["第2泳者"];

                        if (name2 != "")
                        {
                            name += "\n" + name2;
                            name += "\n" + (string)dr["第3泳者"];
                            name += "\n" + (string)dr["第4泳者"];
                        }
                        TrySetValue(wb, rangeName + "Name", name);
                        TrySetValue(wb, rangeName + "Team", (string)dr["所属"]);

                        string recordMark = (string)dr["新記録印刷マーク"];
                        if ((string)dr["中間新記録マーク"]!="") { 
                            recordMark += "中間 : " + (string)dr["中間新記録マーク"];
                        }
                        TrySetValue(wb, rangeName + "GR", ((string)dr["新記録印刷マーク"]).Trim());
                    }
                    TrySetValue(wb, "eventName",GlobalV.EventName+"優勝者リスト");
                    TrySetValue(wb, "eventVenue", EventVenue);
                    TrySetValue(wb, "eventDate", EventDate.Trim());

                    // ファイル保存
                    wb.SaveAs(fullPath);
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
                }
            }
        }
    }


}
