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
        static bool TrySetValue(XLWorkbook wb, string name, string value)
        {
            //if (wb.NamedRanges.TryGetValue(name, out var nr))
            if (wb.DefinedNames.TryGetValue(name, out var nr))
            {
                nr.Ranges.First().Value = value;
                return true;
            }
            return false;
        }

        public static void ReadResult(string serverName, int eventNo,string templatePath, string fileName)
        {
           
            // 保存先のフルパス（任意で変更可）
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string folderPath = "C:\\Users\\user";
            string fullPath = Path.Combine(folderPath, fileName);

            // フォルダがなければ作成
            Directory.CreateDirectory(folderPath);
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;

            string query = $@"
IF EXISTS (SELECT 1 FROM クラス WHERE 大会番号 = @eventNo)
BEGIN
-- クラスが存在する場合
with myresult as (
     SELECT 
         rank() over (partition by 記録.競技番号, 記録.新記録判定クラス ORDER BY 記録.ゴール ASC) AS 順位 ,
      プログラム.性別コード as 性別コード ,
        記録.新記録判定クラス , 
        プログラム.距離コード, 
        プログラム.種目コード, 
        記録.ゴール, 
        記録.新記録印刷マーク, 
        中間新記録マーク,
        選手.氏名 as 氏名, 
        選手.所属名称1 as 所属 

   from 記録 
    INNER JOIN 選手 ON 選手.選手番号 = 記録.選手番号 
	     and 選手.大会番号=記録.大会番号
    inner join プログラム on プログラム.競技番号=記録.競技番号 
         and プログラム.大会番号=記録.大会番号
 WHERE 記録.事由表示 = 0 and  記録.大会番号=   @eventNo 
    　　and プログラム.種目コード<6 
        and 記録.ゴール <> '' 
        and 記録.オープン=0
        and (プログラム.予決コード=6 or プログラム.予決コード=3) 
       Union ALL 
    SELECT 
        rank() over (partition by 記録.競技番号, 記録.新記録判定クラス
			 ORDER BY 記録.ゴール ASC) AS 順位 , 
   プログラム.性別コード as 性別コード,     
        記録.新記録判定クラス, 
        プログラム.距離コード, 
        プログラム.種目コード, 
        記録.ゴール, 
        記録.新記録印刷マーク,
        中間新記録マーク,
        concat (選手1.氏名, ' ・ ', 選手2.氏名,' ・ ',  選手3.氏名,' ・ ',  選手4.氏名 ) as 氏名,
   リレーチーム.チーム名 as 所属
   from 記録 
    inner join リレーチーム on リレーチーム.チーム番号 = 記録.選手番号 and リレーチーム.大会番号 =記録.大会番号
    INNER JOIN 選手 as 選手1 ON 選手1.選手番号 = 記録.第１泳者 and 選手1.大会番号=記録.大会番号
    inner join 選手 as 選手2 on 選手2.選手番号 = 記録.第２泳者 and 選手2.大会番号=記録.大会番号
    inner join 選手 as 選手3 on 選手3.選手番号 = 記録.第３泳者 and 選手3.大会番号=記録.大会番号
    inner join 選手 as 選手4 on 選手4.選手番号 = 記録.第４泳者 and 選手4.大会番号=記録.大会番号
    inner join プログラム on プログラム.競技番号=記録.競技番号 and プログラム.大会番号=  記録.大会番号
    WHERE 記録.事由表示 = 0 
        and 記録.大会番号=   @eventNo 
    　　and プログラム.種目コード>5 
        and 記録.ゴール <> '' 
        and 記録.オープン=0
        and (プログラム.予決コード=6 or プログラム.予決コード=3) 
    ) 
    select 
 myresult.新記録判定クラス as クラス番号,
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
     myresult.新記録印刷マーク,
	 myresult.中間新記録マーク
     from myresult 
	 inner join クラス on クラス.クラス番号=myresult.新記録判定クラス 
	 inner join 距離 on 距離.距離コード = myresult.距離コード
	 inner join 種目 on 種目.種目コード = myresult.種目コード
	 where myresult.順位=1 
	   and クラス.大会番号=@eventNo
	 order by クラス.クラス番号,性別, myresult.種目コード, myresult.距離コード
END	
ELSE BEGIN
-- クラスがない場合
with myresult as (
     SELECT 
         rank() over (partition by 記録.競技番号 ORDER BY 記録.ゴール ASC) AS 順位 ,
      プログラム.性別コード as 性別コード ,
        プログラム.距離コード, 
        プログラム.種目コード, 
        記録.ゴール, 
        記録.新記録印刷マーク,
        中間新記録マーク,
        選手.氏名 as 氏名, 
        選手.所属名称1 as 所属 

   from 記録 
    INNER JOIN 選手 ON 選手.選手番号 = 記録.選手番号 
	     and 選手.大会番号=記録.大会番号
    inner join プログラム on プログラム.競技番号=記録.競技番号 
         and プログラム.大会番号=記録.大会番号
 WHERE 記録.事由表示 = 0 and  記録.大会番号=   @eventNo 
    　　and プログラム.種目コード<6 --個人種目
        and 記録.ゴール <> '' 
        and 記録.オープン=0
        and (プログラム.予決コード=6 or プログラム.予決コード=3) 
       Union ALL 
    SELECT 
         rank() over (partition by 記録.競技番号, 記録.新記録判定クラス
			 ORDER BY 記録.ゴール ASC) AS 順位 , 
   プログラム.性別コード as 性別コード,
 
        プログラム.距離コード, 
        プログラム.種目コード, 
        記録.ゴール, 
        記録.新記録印刷マーク,
        中間新記録マーク,
        concat (選手1.氏名, ' ・ ', 選手2.氏名,' ・ ',  選手3.氏名,' ・ ',  選手4.氏名 ) as 氏名,
   リレーチーム.チーム名 as 所属
   from 記録 
 
    inner join リレーチーム on リレーチーム.チーム番号 = 記録.選手番号 and リレーチーム.大会番号 =記録.大会番号
    INNER JOIN 選手 as 選手1 ON 選手1.選手番号 = 記録.第１泳者 and 選手1.大会番号=記録.大会番号
    inner join 選手 as 選手2 on 選手2.選手番号 = 記録.第２泳者 and 選手2.大会番号=記録.大会番号
    inner join 選手 as 選手3 on 選手3.選手番号 = 記録.第３泳者 and 選手3.大会番号=記録.大会番号
    inner join 選手 as 選手4 on 選手4.選手番号 = 記録.第４泳者 and 選手4.大会番号=記録.大会番号
    inner join プログラム on プログラム.競技番号=記録.競技番号 and プログラム.大会番号=  記録.大会番号
    WHERE 記録.事由表示 = 0 
       and 記録.大会番号=   @eventNo 
    　　and プログラム.種目コード>5 
        and 記録.ゴール <> '' 
        and 記録.オープン=0
        and (プログラム.予決コード=6 or プログラム.予決コード=3) 
    ) 
    select 
     0 as クラス番号, 
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
     myresult.新記録印刷マーク,
	 myresult.中間新記録マーク
     from myresult 
	 inner join 距離 on 距離.距離コード = myresult.距離コード
	 inner join 種目 on 種目.種目コード = myresult.種目コード
	 where myresult.順位=1 
	 order by 性別, myresult.種目コード, myresult.距離コード
END;";

            using (var con = new SqlConnection(connectionString))
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
                using (var wb = new XLWorkbook(templatePath))
                {
                    int classNo = 0;
                    string gender="";
                    string style;
                    string distance;
                    string rangeName;

                    foreach (DataRow dr in dt.Rows)
                    {
                        classNo = Convert.ToInt32(dr["クラス番号"] );
                        gender = (string)dr["性別"];
                                            style = (string)dr["種目"];
                        distance = (string)dr["距離"];
                        
                        rangeName=gender +classNo+"_"+ distance.Trim() +"_"+ style;
                        TrySetValue(wb, rangeName+"Time",(string)dr["記録"]);
                        string nameString = ((string)dr["氏名"]).Replace(" ・ ", Environment.NewLine);

                        TrySetValue(wb, rangeName + "Name", nameString);
                        TrySetValue(wb, rangeName + "Team", (string)dr["所属"]);

                        string recordMark = (string)dr["新記録印刷マーク"];
                        if ((string)dr["中間新記録マーク"]!="") { 
                            recordMark += "中間 : " + (string)dr["中間新記録マーク"];
                        }
                        TrySetValue(wb, rangeName + "GR", (string)dr["新記録印刷マーク"]);
                    }
                    TrySetValue(wb, "大会名",GlobalV.EventName+"優勝者リスト");
                    TrySetValue(wb, "開催地", EventVenue);
                    TrySetValue(wb, "開催日", EventDate.Trim());

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
