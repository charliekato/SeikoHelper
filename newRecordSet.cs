using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClosedXML.Excel;
using System.Diagnostics;


namespace SeikoHelper
{
    /*
     * 本大会で樹立された新記録
     * */

    class NewRecordSet
    {
       static string myQuery = @"
if Exists (select 1 from クラス where 大会番号=@eventNo and クラス番号<100)
BEGIN
 SELECT  
		表示用競技番号 as No,
		クラス.クラス名称, 
        CASE プログラム.性別コード
            WHEN 1 THEN '男子'
            WHEN 2 THEN '女子'
            WHEN 3 THEN '混成'
            ELSE '混合'
        END AS 性別,
        CONCAT(距離.距離, ' ', 種目.種目) AS 種目, 
        記録.ゴール AS タイム, 
        記録.新記録印刷マーク as マーク,
        選手.氏名 AS 泳者, 
        選手.所属名称1 AS 所属・チーム名, 
         case 中間新記録マーク　
        　when '' then        ''
          else 中間記録 end as 中間記録,
		記録.中間新記録マーク as 中間マーク

    FROM 記録
    INNER JOIN プログラム ON プログラム.競技番号 = 記録.競技番号
	INNER JOIN クラス on クラス.クラス番号 = 記録.新記録判定クラス
    INNER JOIN 選手 ON 選手.選手番号 = 記録.選手番号
    INNER JOIN 距離 ON 距離.距離コード = プログラム.距離コード
    INNER JOIN 種目 ON 種目.種目コード = プログラム.種目コード
    WHERE プログラム.大会番号 = @eventNo
      AND 記録.大会番号 = @eventNo
      AND 選手.大会番号 = @eventNo
	  and クラス.大会番号 = @eventNo
      AND プログラム.種目コード < 6
      AND ( 新記録印刷マーク <> '' OR 中間新記録マーク <> '' )
      AND プログラム.競技番号 = 記録.競技番号
	  UNION all
  SELECT 
      表示用競技番号 as No,
      クラス.クラス名称,   
        CASE プログラム.性別コード
            WHEN 1 THEN '男子'
            WHEN 2 THEN '女子'
            WHEN 3 THEN '混成'
            ELSE '混合'
        END AS 性別,
        CONCAT(距離.距離, ' ', 種目.種目) AS 種目, 
        記録.ゴール AS タイム, 
	    記録.新記録印刷マーク as マーク,
        CONCAT(選手1.氏名, ' ・ ', 選手2.氏名, ' ・ ', 選手3.氏名, ' ・ ', 選手4.氏名) AS 泳者, 
         リレーチーム.チーム名 AS 所属・チーム名, 
         case 中間新記録マーク　
        　when '' then        ''
          else 中間記録 end as 中間記録,
		記録.中間新記録マーク as 中間マーク
    FROM 記録
    INNER JOIN プログラム ON プログラム.競技番号 = 記録.競技番号
	INNER JOIN クラス on クラス.クラス番号 = 記録.新記録判定クラス
	INNER JOIN リレーチーム ON リレーチーム.チーム番号 = 記録.選手番号
    INNER JOIN 選手 as 選手1 ON 選手1.選手番号 = 記録.第１泳者
    INNER JOIN 選手 as 選手2 ON 選手2.選手番号 = 記録.第２泳者
    INNER JOIN 選手 as 選手3 ON 選手3.選手番号 = 記録.第３泳者
    INNER JOIN 選手 as 選手4 ON 選手4.選手番号 = 記録.第４泳者
    INNER JOIN 距離 ON 距離.距離コード = プログラム.距離コード
    INNER JOIN 種目 ON 種目.種目コード = プログラム.種目コード
    WHERE プログラム.大会番号 = @eventNo
      AND 記録.大会番号 = @eventNo
	  AND リレーチーム.大会番号 = @eventNo
      AND 選手1.大会番号 = @eventNo
	  AND 選手2.大会番号 = @eventNo
	  AND 選手3.大会番号 = @eventNo
      AND 選手4.大会番号 = @eventNo
	  AND クラス.大会番号 = @eventNo
      AND プログラム.種目コード > 5
      AND (新記録印刷マーク <> '' OR 中間新記録マーク <> '' )
      AND プログラム.競技番号 = 記録.競技番号
	  ORDER BY No
END
ELSE BEGIN
    -- class does not exist
 SELECT  
		表示用競技番号 as No,
        CASE プログラム.性別コード
            WHEN 1 THEN '男子'
            WHEN 2 THEN '女子'
            WHEN 3 THEN '混成'
            ELSE '混合'
        END AS 性別,
        CONCAT(距離.距離, ' ', 種目.種目) AS 種目, 
        記録.ゴール AS タイム, 
        記録.新記録印刷マーク as マーク,
        選手.氏名 AS 泳者, 
        選手.所属名称1 AS 所属・チーム名, 

         case 中間新記録マーク　
        　when '' then        ''
          else 中間記録 end as 中間記録,
		記録.中間新記録マーク as 中間マーク
    FROM 記録
    INNER JOIN プログラム ON プログラム.競技番号 = 記録.競技番号
	    AND プログラム.大会番号=記録.大会番号
    INNER JOIN 選手 ON 選手.選手番号 = 記録.選手番号 
	       AND 選手.大会番号=記録.大会番号
    INNER JOIN 距離 ON 距離.距離コード = プログラム.距離コード
    INNER JOIN 種目 ON 種目.種目コード = プログラム.種目コード
    WHERE  記録.大会番号 = @eventNo
      AND プログラム.種目コード < 6
      AND (新記録印刷マーク <> '' OR 中間新記録マーク <> '')
      AND プログラム.競技番号 = 記録.競技番号
	  UNION all
  SELECT 
	    表示用競技番号 as No,
        CASE プログラム.性別コード
            WHEN 1 THEN '男子'
            WHEN 2 THEN '女子'
            WHEN 3 THEN '混成'
            ELSE '混合'
        END AS 性別,
        CONCAT(距離.距離, ' ', 種目.種目) AS 種目, 
        記録.ゴール AS タイム, 
         記録.新記録印刷マーク as マーク,
        CONCAT(選手1.氏名, ' ・ ', 選手2.氏名, ' ・ ', 選手3.氏名, ' ・ ', 選手4.氏名) AS 泳者, 
        リレーチーム.チーム名 AS 所属・チーム名, 
         case 中間新記録マーク　
        　when '' then        ''
          else 中間記録 end as 中間記録,
		記録.中間新記録マーク as 中間マーク
    FROM 記録
    INNER JOIN プログラム ON プログラム.競技番号 = 記録.競技番号
    INNER JOIN リレーチーム ON リレーチーム.チーム番号 = 記録.選手番号
    INNER JOIN 選手 as 選手1 ON 選手1.選手番号 = 記録.第１泳者
    INNER JOIN 選手 as 選手2 ON 選手2.選手番号 = 記録.第２泳者
    INNER JOIN 選手 as 選手3 ON 選手3.選手番号 = 記録.第３泳者
    INNER JOIN 選手 as 選手4 ON 選手4.選手番号 = 記録.第４泳者
    INNER JOIN 距離 ON 距離.距離コード = プログラム.距離コード
    INNER JOIN 種目 ON 種目.種目コード = プログラム.種目コード
    WHERE プログラム.大会番号 = @eventNo
      AND 記録.大会番号 = @eventNo
	  AND リレーチーム.大会番号 = @eventNo
      AND 選手1.大会番号 = @eventNo
	  AND 選手2.大会番号 = @eventNo
	  AND 選手3.大会番号 = @eventNo
      AND 選手4.大会番号 = @eventNo
      AND プログラム.種目コード > 5
      AND (新記録印刷マーク <> '' OR 中間新記録マーク <> '')
      AND プログラム.競技番号 = 記録.競技番号
	  ORDER by No
END
 ";
        public static void CreateNewRecordEstablished(string fileName)
        {
            // 保存先のフルパス（任意で変更可）
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string folderPath = "C:\\Users\\user";
            string fullPath = Path.Combine(folderPath, fileName);

            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);

            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("発生大会記録");
                using (var conn = new SqlConnection(connectionString))
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
                                worksheet.Cell(excelRow, 1).Value = Convert.ToInt16(dr["No"]);
                                worksheet.Cell(excelRow, 2).Value = (string)dr["クラス名称"];
                                worksheet.Cell(excelRow, 3).Value = (string)dr["性別"];
                                worksheet.Cell(excelRow, 4).Value = (string)dr["種目"];
                                worksheet.Cell(excelRow, 5).Value = (string)dr["タイム"];
                                worksheet.Cell(excelRow, 6).Value = (string)dr["マーク"];
                                worksheet.Cell(excelRow, 7).Value = (string)dr["泳者"];
                                worksheet.Cell(excelRow, 8).Value = (string)dr["所属・チーム名"];
                                worksheet.Cell(excelRow, 9).Value = (string)dr["中間記録"];
                                worksheet.Cell(excelRow, 10).Value = (string)dr["中間マーク"];
                                excelRow++;
                            }
                        } // dr
                    } //cmd
                    /*****/
                    // 列幅の調整
                    worksheet.Cell(1, 1).Value = "No";
                    worksheet.Cell(1, 2).Value = "クラス";
                    worksheet.Cell(1, 3).Value = "性別";
                    worksheet.Cell(1, 4).Value = "種目";
                    worksheet.Cell(1, 5).Value = "記録";
                    worksheet.Cell(1, 6).Value = "マーク";
                    worksheet.Cell(1, 7).Value = "泳者";
                    worksheet.Cell(1, 8).Value = "所属・チーム名";
                    worksheet.Cell(1, 9).Value = "中間記録";
                    worksheet.Cell(1, 10).Value = "マーク";

                    worksheet.Column("A").Width = 3.0;   // No
                    worksheet.Column("B").Width = 14.6;  // クラス
                    worksheet.Column("C").Width = 5.0;   // 性別（40px）
                    worksheet.Column("D").Width = 21.6;  // 種目

                    worksheet.Column("E").Width = 9.0;  // 記録
                    worksheet.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    worksheet.Column("F").Width = 6.3;  // マーク
                    worksheet.Column("G").Width = 12.8;  // 泳者

                    worksheet.Column(7).Style.Alignment.WrapText = true;
                    worksheet.Column("H").Width = 17.4;  // 所属
                    worksheet.Column("I").Width = 9.0;  // 中間記録
                    worksheet.Column("I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column("J").Width = 6.7;  // 中間記録
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
