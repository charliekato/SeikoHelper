using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClosedXML.Excel;
using System.Diagnostics;


namespace SeikoHelper
{

    class EntryList
    {
        static public bool tateFlag;
        public static void CreateEntryList(string fileName)
        {
            // 保存先のフルパス（任意で変更可）
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string folderPath = "C:\\Users\\user";
            string fullPath = Path.Combine(folderPath, fileName);

            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);

            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;

            string sql;
            string sqlYoko = @"
 WITH 種目一覧 AS ( 
    SELECT エントリー.選手番号, 
        プログラム.表示用競技番号 AS PrgNo, 
        クラス.クラス名称, 
        距離.距離 AS 距離文字列, 
        種目.種目, 
        記録.組, 
        記録.水路,
        ROW_NUMBER() OVER ( 
            PARTITION BY エントリー.選手番号 
            ORDER BY プログラム.表示用競技番号 
        ) AS 種目番号 
    FROM エントリー 
    INNER JOIN 種目 ON 種目.種目コード = エントリー.種目コード 
    INNER JOIN 距離 ON 距離.距離コード = エントリー.距離コード 
    INNER JOIN クラス ON クラス.クラス番号 = エントリー.新記録判定クラス 
    INNER JOIN プログラム ON プログラム.種目コード = エントリー.種目コード -- AND プログラム.クラス番号 = エントリー.新記録判定クラス 
        AND プログラム.距離コード = エントリー.距離コード 
    INNER JOIN 記録 ON 記録.競技番号 = プログラム.競技番号 
        AND 記録.選手番号 = エントリー.選手番号 
    WHERE エントリー.大会番号 = @eventNo 
        AND クラス.大会番号 = @eventNo 
        AND プログラム.大会番号 = @eventNo 
        AND 記録.大会番号 = @eventNo 
 ) 
 SELECT 選手.選手番号, 
     選手.氏名, 
     CASE 選手.性別コード 
         WHEN 1 THEN '男子' 
         WHEN 2 THEN '女子' 
         WHEN 3 THEN '混成' 
         WHEN 4 THEN '混合' 
     END AS 性別, 
    選手.所属名称1, 
     S1.クラス名称 + S1.距離文字列 + S1.種目 AS 種目1, 
    CONCAT(S1.PrgNo, '-', S1.組, '-', S1.水路) AS [競技番号-組-レーン1], 
    S2.クラス名称 + S2.距離文字列 + S2.種目 AS 種目2, 
    CONCAT(S2.PrgNo, '-', S2.組, '-', S2.水路) AS [競技番号-組-レーン2], 
    S3.クラス名称 + S3.距離文字列 + S3.種目 AS 種目3, 
    CONCAT(S3.PrgNo, '-', S3.組, '-', S3.水路) AS [競技番号-組-レーン3] 
    FROM 選手 
    LEFT JOIN (
        SELECT * FROM 種目一覧 WHERE 種目番号 = 1
        ) S1 ON 選手.選手番号 = S1.選手番号 
    LEFT JOIN (SELECT * FROM 種目一覧 
    WHERE 種目番号 = 2) S2 ON 選手.選手番号 = S2.選手番号 
    LEFT JOIN (
    SELECT * FROM 種目一覧 WHERE 種目番号 = 3
    ) S3 
    ON 選手.選手番号 = S3.選手番号 
    WHERE 選手.大会番号 = @eventNo 
    ORDER BY 選手.選手番号;";

            string sqlTate = @"
WITH 種目一覧 AS ( 
    SELECT
        エントリー.選手番号,
        プログラム.表示用競技番号 AS PrgNo,
        クラス.クラス名称,
        距離.距離 AS 距離文字列,
        種目.種目,
        記録.組,
        記録.水路,
        ROW_NUMBER() OVER (
            PARTITION BY エントリー.選手番号
            ORDER BY プログラム.表示用競技番号
        ) AS 種目番号
    FROM エントリー 
    INNER JOIN 種目 ON 種目.種目コード = エントリー.種目コード
    INNER JOIN 距離 ON 距離.距離コード = エントリー.距離コード
    INNER JOIN クラス ON クラス.クラス番号 = エントリー.新記録判定クラス
    INNER JOIN プログラム ON プログラム.種目コード = エントリー.種目コード 
        AND プログラム.距離コード = エントリー.距離コード
    INNER JOIN 記録 ON 記録.競技番号 = プログラム.競技番号 
        AND 記録.選手番号 = エントリー.選手番号
    WHERE 
        エントリー.大会番号 = @eventNo 
        AND クラス.大会番号 = @eventNo 
        AND プログラム.大会番号 = @eventNo
        AND 記録.大会番号 = @eventNo
)

SELECT 
    選手.選手番号,
    選手.氏名,
    CASE 選手.性別コード
        WHEN 1 THEN '男子'
        WHEN 2 THEN '女子'
        WHEN 3 THEN '混成'
        WHEN 4 THEN '混合'
    END AS 性別, 
    選手.所属名称1,
    S.種目番号,
    S.クラス名称 + S.距離文字列 + S.種目 AS 種目,
    CONCAT(S.PrgNo, '-', S.組, '-', S.水路) AS [競技番号-組-レーン]
FROM 選手
LEFT JOIN 種目一覧 S ON 選手.選手番号 = S.選手番号
WHERE 選手.大会番号 = @eventNo
ORDER BY 選手.選手番号, S.種目番号;

    ";
            string sqlRelay = @"
select チーム名, 
   case プログラム.性別コード
        WHEN 1 THEN '男子'
        WHEN 2 THEN '女子'
        WHEN 3 THEN '混成'
        WHEN 4 THEN '混合'
    END AS 性別,
   CONCAT( case プログラム.距離コード when 3 then '4x25m ' 
                          when 4 then '4x50m '
                          when 5 then '4x100m '
                          when 6 then '4x200m ' end , 種目.種目) as 種目, 
    クラス.クラス名称 as クラス, 
　　CONCAT(プログラム.表示用競技番号, '-',記録.組, '-',記録.水路) as  [No-組-レーン] 
from リレーチーム 
inner join プログラム on プログラム.種目コード = リレーチーム.種目コード 
  and プログラム.距離コード = リレーチーム.距離コード
   and プログラム.クラス番号=リレーチーム.新記録判定クラス
inner join 種目 on 種目.種目コード = プログラム.種目コード
inner join 記録 on 記録.競技番号 = プログラム.競技番号 and 記録.選手番号 = リレーチーム.チーム番号
inner join クラス on クラス.クラス番号 = プログラム.クラス番号
where リレーチーム.大会番号 = @eventNo 
    and クラス.大会番号 = @eventNo
  and プログラム.大会番号 = @eventNo 
  and 記録.大会番号 = @eventNo ";

            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("個人種目");
                var worksheet2 = workbook.Worksheets.Add("リレー種目");
                using (var conn = new SqlConnection(connectionString))
                {
                    if (tateFlag) sql = sqlTate;
                    else sql = sqlYoko;
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            cmd.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);

                            var dt = new DataTable();
                            adapter.Fill(dt);

                            worksheet.Cell(1, 1).InsertTable(dt);
                        } // adapter
                    } //cmd
                        /*****/
                    // 列幅の調整
                    worksheet.Column("A").Width = 5.0;   // 選手番号（40px）
                    worksheet.Column("B").Width = 16.6;  // 氏名（130px）
                    worksheet.Column("C").Width = 5.0;   // 性別（40px）
                    worksheet.Column("D").Width = 16.6;  // 所属名称（130px）

                    // 種目1〜3（250px）、列は "G", "J", "M"
                    worksheet.Column("E").Width = 30.7;  // 種目1
                    worksheet.Column("G").Width = 30.7;  // 種目2
                    worksheet.Column("I").Width = 30.7;  // 種目3

                    // 競技番号-組-レーン1〜3（80px）、列は "F", "I", "L"
                    worksheet.Column("F").Width = 8.4;  // 番号1
                    worksheet.Column("H").Width = 8.4;  // 番号2
                    worksheet.Column("J").Width = 8.4;  // 番号3

                    using (SqlCommand cmd = new SqlCommand(sqlRelay, conn))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            cmd.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);

                            var dt = new DataTable();
                            adapter.Fill(dt);

                            worksheet2.Cell(1, 1).InsertTable(dt);
                        } // adapter
                    } //cmd

                    worksheet2.Column("A").Width = 20.4;  // チーム名
                    worksheet2.Column("B").Width = 8.4;  // 性別
                    worksheet2.Column("C").Width = 30.4;  // 種目
                    worksheet2.Column("D").Width = 30.4;  // クラス
                    worksheet2.Column("E").Width = 16.4;  // No-組-レーン

                  

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
