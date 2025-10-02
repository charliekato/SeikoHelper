using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClosedXML.Excel;
using System.Diagnostics;

namespace SeikoHelper
{
    class WinnerList
    {
        public static string EventDate = string.Empty;
        public static string EventVenue = string.Empty;
        public static void ReadResult(string serverName, int eventNo,string fileName)
        {
            // 保存先のフルパス（任意で変更可）
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string folderPath = "C:\\Users\\user";
            string fullPath = Path.Combine(folderPath, fileName);

            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;

            string query = $@"
    with myresult as (
        SELECT 
            rank() over (partition by 記録.競技番号";
            
            if ( NewRecordExporter.ClassExist())
            {
                query +=  ", 記録.新記録判定クラス";
            }
            query = query + $@"  ORDER BY 記録.ゴール ASC) AS 順位,
            プログラム.性別コード as 性別コード, ";
            if (NewRecordExporter.ClassExist())
            {
                query +=  " 記録.新記録判定クラス, ";
            }

            query = query + $@"
                       プログラム.距離コード,
            プログラム.種目コード,
            記録.ゴール,
            記録.新記録印刷マーク,
            選手.氏名 as 氏名,
            選手.所属名称1 as 所属
        from 記録
            inner join 選手 on 選手.選手番号 = 記録.選手番号
                and 選手.大会番号 = 記録.大会番号
            inner join プログラム on プログラム.競技番号 = 記録.競技番号
                and プログラム.大会番号 = 記録.大会番号
        where 記録.事由表示 = 0 
          and 選手.大会番号 = {eventNo}
          and プログラム.種目コード < 6
          and 記録.ゴール <> ''
          and (プログラム.予決コード = 6 or プログラム.予決コード = 3)

        union all

        SELECT
            rank() over (partition by 記録.競技番号, 記録.新記録判定クラス ORDER BY 記録.ゴール ASC) AS 順位,
            プログラム.性別コード as 性別コード,";
            if (NewRecordExporter.ClassExist())
            {
                query +=  "記録.新記録判定クラス,";
            }
            query +=  $@"
            プログラム.距離コード,
            プログラム.種目コード,
            記録.ゴール,
            記録.新記録印刷マーク,
            concat(
                選手1.氏名, CHAR(10),
                選手2.氏名, CHAR(10),
                選手3.氏名, CHAR(10),
                選手4.氏名
            ) as 氏名,
            リレーチーム.チーム名 as 所属
        from 記録
            inner join リレーチーム on リレーチーム.チーム番号 = 記録.選手番号
                and リレーチーム.大会番号 = 記録.大会番号
            inner join 選手 as 選手1 on 選手1.選手番号 = 記録.第１泳者
                and 選手1.大会番号 = 記録.大会番号
            inner join 選手 as 選手2 on 選手2.選手番号 = 記録.第２泳者
                and 選手2.大会番号 = 記録.大会番号
            inner join 選手 as 選手3 on 選手3.選手番号 = 記録.第３泳者
                and 選手3.大会番号 = 記録.大会番号
            inner join 選手 as 選手4 on 選手4.選手番号 = 記録.第４泳者
                and 選手4.大会番号 = 記録.大会番号
            inner join プログラム on プログラム.競技番号 = 記録.競技番号
                and プログラム.大会番号 = 記録.大会番号
        where 記録.事由表示 = 0
          and 記録.大会番号 = {eventNo}
          and プログラム.種目コード > 5
          and 記録.ゴール <> ''
          and (プログラム.予決コード = 6 or プログラム.予決コード = 3)
    )
    select ";
            if (NewRecordExporter.ClassExist())
            {
                query +=  " クラス.クラス名称,";
            }
            query += $@"
        case myresult.性別コード
            when 1 then '男子'
            when 2 then '女子'
            when 3 then '混成'
            when 4 then '混合'
        end as 性別,
        距離.距離,
        種目.種目,
        myresult.ゴール as 記録,
        myresult.氏名,
        myresult.所属,
        myresult.新記録印刷マーク
    from myresult";
            if (NewRecordExporter.ClassExist())
            {
                query += "   inner join クラス on クラス.クラス番号 = myresult.新記録判定クラス";
            }
            query += $@"
        inner join 距離 on 距離.距離コード = myresult.距離コード
        inner join 種目 on 種目.種目コード = myresult.種目コード
    where myresult.順位 = 1 ";
            if (NewRecordExporter.ClassExist())
            {
                query += $"  and クラス.大会番号 = {eventNo}";

            }
            query += " order by ";
            if (NewRecordExporter.ClassExist()) {
                query += "クラス.クラス番号, ";
            }
            query+= " 性別, myresult.種目コード, myresult.距離コード; ";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                con.Open();
                var dt = new DataTable();
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }

                // ClosedXMLでExcelファイル作成
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("結果");
                    string className="";
                    string gender="";

                    int row = 3;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (NewRecordExporter.ClassExist())
                        {
                            if (className != (string)dr["クラス名称"])
                            {
                                className = (string)dr["クラス名称"];
                                ws.Cell(row, 1).Value = className;
                                gender = "";
                                row++;
                            }
                        }
                        if (gender != (string)dr["性別"])
                        {
                            gender = (string)dr["性別"];
                            ws.Cell(row, 2).Value = gender;
                            row++;
                        }
                        ws.Cell(row, 3).Value = (string)dr["種目"];
                        ws.Cell(row, 4).Value = (string)dr["距離"];

                        ws.Cell(row, 5).Value = (string)dr["記録"];
                        
                        // 氏名（改行あり）
                        var nameCell = ws.Cell(row, 6);
                        nameCell.Value = (string)dr["氏名"];
                        nameCell.Style.Alignment.WrapText = true; // 改行を有効にする

                        ws.Cell(row, 7).Value = (string)dr["所属"];
                        ws.Cell(row, 8).Value = (string)dr["新記録印刷マーク"];
                        row++;
                    }
                    ws.Cell(1, 1).Value = GlobalV.EventName+"優勝者リスト";
                    ws.Cell(2, 1).Value = EventVenue;
                    ws.Cell(2, 7).Value = EventDate;
                    ws.Column(1).Width = 2;  // クラス
                    ws.Column(2).Width = 2;  // 性別
                    ws.Column(3).Width = 15; // 種目
                    ws.Column(4).Width = 6;  // 距離
                    ws.Column(5).Width = 9;  // タイム
                    ws.Column(6).Width = 15; // 氏名
                    ws.Column(7).Width = 15; // 所属
                    ws.Column(8).Width = 9;  // 新記録
                    ws.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column(5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


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
