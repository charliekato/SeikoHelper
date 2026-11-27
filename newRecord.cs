using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Data.SqlClient;
using ClosedXML.Excel;
using System.Diagnostics;

namespace SeikoHelper
{

    public static class NewRecordExporter /*: Form*/
    {
        static string[] genderStr = { "男子", "女子", "混成", "混合" };
        private static int TimeStr2Int(string myTime)
        {
            string temp = myTime.Replace(".", "");
            if (temp == "") return 0;

            return  Convert.ToInt32(temp.Replace(":",""));
        }


        public static void BackupDB()
        {
            string Query = @"
                     Select * INTO 新記録bk 
                     From 新記録
                     Where 大会番号=@eventNo";
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(Query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    command.ExecuteNonQuery();
                }
            }
        }
        // 記録名称番号を取得する。なければ0を返す。
        // 記録名称番号は、記録名称テーブルの大会番号と記録名称が「大会」であるレコードの記録名称番号を指す。
        // 新記録名称番号は、大会記録の新記録名称番号は通常 1 。(他の優先記録 例えば、県記録、日本記録などは2以上が使われる。)
        // 優先記録の記録区分番号は　0。大会記録の記録区分番号はクラス番号と同じ。クラスがない場合は0。
        public static int GetRecordNameNumber()
        {
            string Query = @"Select TOP 1 記録名称番号 from 新記録名称
                              where 大会番号=@eventNo";
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(Query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    object reader = command.ExecuteScalar();
                    if (reader != null)
                    {
                        return Convert.ToInt32(reader.ToString());
                    }
                    else return 0;

                }
            }
        }
        private static void InsertGameRecord(int genderCode, int classNo, int distanceCode,
            int strokeCode, string GR,string swimmeRname,  string shozoku, string eventName=""  )
        {
            int tournamentRecordNo = GetRecordNameNumber();

            string myQuery = @" 
                     insert into 新記録 ( 大会番号, 性別コード, 記録区分番号,
                     記録名称番号, 種目コード, 距離コード, 記録, 保持者番号, 記録保持者, 
                    所属,競技会名, 会場) values
                     ( @eventNo, @gender, @classNo, @recordNo, @strokeCode, 
                       @distanceCode,@timeValue,@holderNo, @name,@shozoku,@eventName,@eventVenue 
                      )"; 
            
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    command.Parameters.AddWithValue("@gender", SqlDbType.Int).Value = genderCode;
                    command.Parameters.AddWithValue("@classNo", SqlDbType.Int).Value = classNo;
                    command.Parameters.AddWithValue("@recordNo", SqlDbType.Int).Value = tournamentRecordNo;
                    command.Parameters.AddWithValue("@strokeCode", SqlDbType.Int).Value = strokeCode;
                    command.Parameters.AddWithValue("@distanceCode", SqlDbType.Int).Value = distanceCode;
                    command.Parameters.Add("@timeValue", SqlDbType.NVarChar,8).Value = GR;
                    command.Parameters.AddWithValue("@holderNo", SqlDbType.Int).Value = 0;
                    command.Parameters.AddWithValue("@name", swimmeRname);
                    command.Parameters.AddWithValue("@shozoku", shozoku);
                    command.Parameters.Add("@eventName", SqlDbType.NVarChar, 50).Value = eventName;
                    command.Parameters.Add("@eventVenue", SqlDbType.NVarChar,30).Value=WinnerList.EventVenue;

                    command.ExecuteNonQuery();
                }
            }
        }
        private static void InsertGameRecord(int genderCode, int classNo, int distanceCode,
            int strokeCode, string GR,string name1, string name2, string name3, string name4, string shozoku  )
        {
            int tournamentRecordNo = GetRecordNameNumber();
            string name = name1;
            if (strokeCode>5) { 
                name += "・"+name2 + "・"+name3+"・"+name4;
            }
            string myQuery = @" 
                     insert into 新記録bk ( 大会番号, 性別コード, 記録区分番号,
                     記録名称番号, 種目コード, 距離コード, 記録, 日付,保持者番号, 記録保持者, 
                    所属,競技会名, 会場) values
                     ( @eventNo, @gender, @classNo, @recordNo, @strokeCode, 
                       @distanceCode,@timeValue,@hizuke,@holderNo, @name,@shozoku,@eventName,@eventVenue)"; 
            
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    command.Parameters.AddWithValue("@gender", SqlDbType.Int).Value = genderCode;
                    command.Parameters.AddWithValue("@classNo", SqlDbType.Int).Value = classNo;
                    command.Parameters.AddWithValue("@recordNo", SqlDbType.Int).Value = tournamentRecordNo;
                    command.Parameters.AddWithValue("@strokeCode", SqlDbType.Int).Value = strokeCode;
                    command.Parameters.AddWithValue("@distanceCode", SqlDbType.Int).Value = distanceCode;
                    command.Parameters.Add("@timeValue", SqlDbType.NVarChar,8).Value = GR;
                    command.Parameters.Add("@hizuke", SqlDbType.NVarChar,10).Value = "";
                    command.Parameters.AddWithValue("@holderNo", SqlDbType.Int).Value = 0;
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@shozoku", shozoku);
                    command.Parameters.Add("@eventName", SqlDbType.NVarChar,50).Value =NewRecordManager.thisYear;
                    command.Parameters.Add("@eventVenue", SqlDbType.NVarChar,30).Value=WinnerList.EventVenue;

                    command.ExecuteNonQuery();
                }
            }
        }
        private static void UpdateGameRecord(int genderCode, int classNo, int distanceCode,
            int strokeCode, string GR, string swimmer1,string swimmer2, string swimmer3, 
            string swimmer4, string shozoku)
        {
            string myQuery = @" 
                update 新記録bk set 記録=@timeValue , 記録保持者=@Name, 所属=@Shozoku,
                       競技会名=@eventName, 会場=@eventVenue
                       where 大会番号=@eventNo
                       and 性別コード=@gender
                       and 記録区分番号=@classNo
                       and 記録名称番号=1 
                       and 種目コード=@strokeCode
                       and 距離コード=@distanceCode";
            string name=swimmer1;
            if (strokeCode>5)
            {
                name += "・" + swimmer2 + "・" + swimmer3 + "・" + swimmer4;
            }
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.CommandType = CommandType.Text; // これはあってもなくてもいいですが、明示的に指定することができます。
                    command.Parameters.AddWithValue("@eventNo",  SqlDbType.Int).Value=GlobalV.config.EventNo;
                    command.Parameters.AddWithValue("@gender", SqlDbType.Int).Value =genderCode;
                    command.Parameters.AddWithValue("@strokeCode", SqlDbType.Int).Value =strokeCode;
                    command.Parameters.AddWithValue("@distanceCode", SqlDbType.Int).Value =distanceCode;
                    command.Parameters.AddWithValue("@classNo", SqlDbType.Int).Value =classNo;
                    command.Parameters.Add("@timeValue", SqlDbType.VarChar,8).Value= GR;
                    command.Parameters.Add("@Name", SqlDbType.VarChar,30).Value=(object)name ?? DBNull.Value; 
                    command.Parameters.Add("@shozoku",SqlDbType.VarChar,16).Value=(object)shozoku ?? DBNull.Value; 
                    command.Parameters.Add("@eventName", SqlDbType.NVarChar, 50).Value = NewRecordManager.thisYear;
                    command.Parameters.Add("@eventVenue", SqlDbType.NVarChar, 30).Value = WinnerList.EventVenue;
                    command.ExecuteNonQuery();
                }
            }
        }
        private static  string GetGameRecord(int genderCode, int classNo, int distanceCode, int strokeCode)
        {
            string myQuery = @"
              select 記録 from 新記録bk where 大会番号=@eventNo 
                   and 性別コード=@gender
                   and 記録区分番号=@classNo
                   and 記録名称番号=( select TOP 1 記録名称番号 from 新記録名称 where 記録名称='大会' and 大会番号=@eventNo)
                   and 種目コード=@strokeCode
                   and 距離コード=@distanceCode";
            string GR = "";
            using ( SqlConnection connection=new SqlConnection(GlobalV.config.ConnectionString))
            {

                connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    command.Parameters.AddWithValue("@gender", SqlDbType.Int).Value = genderCode;
                    command.Parameters.AddWithValue("@strokeCode", SqlDbType.Int).Value = strokeCode;
                    command.Parameters.AddWithValue("@distanceCode", SqlDbType.Int).Value = distanceCode;
                    command.Parameters.AddWithValue("@classNo", SqlDbType.Int).Value = classNo;
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            GR = dr["記録"] == DBNull.Value ? "" : dr["記録"].ToString() ?? "";
                        }
                    }
                } 
            }
            return GR ;
        }
        public static string if_not_null_string(object dr)
        {
            if (dr == DBNull.Value) return "";
            return (string)dr;
        }

        public static string ConvToString(object value)
        {
            if (value == null || value == DBNull.Value)
                return "";
            return (string)value;
        }

        public static int ConvToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            return Convert.ToInt32(value);
        }
           public static void CompareToExcel(string serverName, int eventNo,string excelFile)
        {
            int numDifference=0;
            string query = " SELECT * from v新記録 where 大会番号=@eventNo and 記録名称='大会'";

            using (var con = new SqlConnection(GlobalV.config.ConnectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@eventNo", eventNo);
                con.Open();
                var dt = new DataTable();
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                try
                {
                    ExcelPhoneticCleaner.RemovePhoneticRuns(excelFile);
                    using (var wb = new XLWorkbook(excelFile))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            int classNo = Convert.ToInt32(dr["記録区分番号"]);
                            int gender = Convert.ToInt32(dr["性別コード"]);
                            string genderName = (string)dr["性別"];
                            string stroke = Convert.ToString(dr["種目コード"]) ?? "";
                            string distance = Convert.ToString(dr["距離コード"]) ?? "";
                            string strokeStr = (string)dr["種目"];
                            string distanceStr = (string)dr["距離"];

                            string rangeName = genderName + classNo + "_" + distance + "_" + stroke;
                            string ExcelTime = WinnerList.TryGetValue(wb, rangeName + "Time");
                            if ((TimeStr2Int(ExcelTime) != TimeStr2Int((string)dr["記録"])) )
                            {
                                numDifference++;
                                MessageBox.Show(genderName + "classNo:"+classNo + " " 
                                    + distanceStr + " " + strokeStr + "  DB : " + (string)dr["記録"] +
                                    "\n" + "Excel : " + ExcelTime);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excelファイルの処理中にエラーが発生しました: " + ex.Message);
                }
                finally
                {
                    if (numDifference == 0) MessageBox.Show("すべて一致しました。");
                    else MessageBox.Show("MissCompare が" + numDifference + "ありました。");
                }
            }
        }
        public static void UpdateExcel()
        {
            Cursor? previousCursor = Cursor.Current;
            MessageBox.Show("次の画面でテンプレートExcel(前回大会後の大会記録の入ったもの)を選択してください。", 
                "テンプレート選択", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // テンプレート選択
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                string templatePath;

                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = "テンプレート Excel を選択";
                    ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";

                    if (ofd.ShowDialog() != DialogResult.OK)
                        return;
                    templatePath = ofd.FileName;
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "保存先のファイルを選択してください";
                sfd.Filter = "エクセルファイル (*.xlsx)|*.xlsx|すべてのファイル (*.*)|*.*";
                sfd.DefaultExt = "xlsx";

                // 処理中カーソル（WaitCursor）を設定
                Cursor.Current = Cursors.WaitCursor;
                //EntryList.CreateEntryList(textBoxEntryList.Text);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string outFile = sfd.FileName;
                    NewRecordExporter.UpdateExcel(GlobalV.ServerName, GlobalV.config.EventNo, outFile, templatePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // カーソルを元に戻す
                Cursor.Current = previousCursor;
            }


        }
        public static void UpdateExcel(string serverName, int eventNo,string filePath ,string templatePath)
        {

            string query = " SELECT * from 新記録bk where 大会番号=@eventNo";

            using (var con = new SqlConnection(GlobalV.config.ConnectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@eventNo", eventNo);
                con.Open();
                var dt = new DataTable();
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }

                ExcelPhoneticCleaner.RemovePhoneticRuns(templatePath);
                // ClosedXMLでExcelファイル作成
                using (var wb = new XLWorkbook(templatePath))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string classNo = Convert.ToString(dr["記録区分番号"]) ?? "" ;  
                        int gender = Convert.ToInt32(dr["性別コード"]);
                        string stroke = Convert.ToString(dr["種目コード"]) ?? "";
                        string distance = Convert.ToString(dr["距離コード"]) ?? "";
                        string rangeName = genderStr[gender - 1] + classNo + "_" + distance + "_" + stroke;
                        string mytime = Convert.ToString(dr["記録"]) ?? "";
                        string oldGR = WinnerList.TryGetValue(wb, rangeName + "Time");
                        if ((oldGR=="")|| (TimeStr2Int(oldGR) > TimeStr2Int(mytime)) )
                        {
                            string nameStr= Convert.ToString(dr["記録保持者"]) ?? "";
                            WinnerList.TrySetValue(wb, rangeName + "Time", mytime);
                            WinnerList.SetMark(wb, rangeName + "Time", XLColor.Yellow);
                            WinnerList.TrySetValue(wb, rangeName + "Name", nameStr.Replace("・","\n"));
                            WinnerList.SetMark(wb, rangeName + "Name", XLColor.Yellow);
                            WinnerList.TrySetValue(wb, rangeName + "Team", (string)dr["所属"]);
                            WinnerList.SetMark(wb, rangeName + "Team", XLColor.Yellow);
                            WinnerList.TrySetValue(wb, rangeName + "GR", NewRecordManager.thisYear);
                            WinnerList.SetMark(wb, rangeName + "GR", XLColor.Yellow);
                        }
                    }
                    WinnerList.TrySetValue(wb, "eventDate", WinnerList.EventDate+"現在");
                    // ファイル保存
                    wb.SaveAs(filePath);
                    var result = MessageBox.Show(
                        $"Excelファイルを保存しました：\n{filePath}\n\n開きますか？",
                        "完了",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        Process.Start("explorer.exe", $"\"{filePath}\"");
                    }
                }
            }
        }
        public static void DeleteBackup()
        {
            string query = "drop table 新記録bk";
            try
            {

                using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("バックアップテーブルの削除中にエラーが発生しました: " + ex.Message);
            }
        }
        public static void InsertGameRecord()
        {
            string query = "select * from v最速者リスト where 大会番号=@eventNo";
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int genderCode = ConvToInt(reader["性別コード"]);
                            int strokeCode = ConvToInt(reader["種目コード"]);
                            int distanceCode = ConvToInt(reader["距離コード"]);
                            string swimmer1 = Convert.ToString(reader["第1泳者"]) ?? "";
                            string swimmer2 = Convert.ToString(reader["第2泳者"]) ?? "";
                            string swimmer3 = Convert.ToString(reader["第3泳者"]) ?? "";
                            string swimmer4 = Convert.ToString(reader["第4泳者"]) ?? "";
                            string shozoku = Convert.ToString(reader["所属・チーム名"]) ?? "";
                            string hizuke = Convert.ToString(reader["日付"]) ?? "";
                            //venue = (string)reader["開催地"];
                            ////eventName = (string) reader["競技会名"];
                            int classNo = ConvToInt(reader["新記録判定クラス"]);
                            string myStrTime = Convert.ToString(reader["ゴール"]) ?? "";

                            int mytime = TimeStr2Int(myStrTime);

                            string gameRecord = GetGameRecord(genderCode, classNo, distanceCode, strokeCode);
                            if (gameRecord == "")
                                InsertGameRecord(genderCode, classNo, distanceCode, strokeCode,
                                    myStrTime, swimmer1, swimmer2, swimmer3, swimmer4, shozoku);
                        }
                    }
                }
            }
        }
        public static void UpdateGameRecord4RelayFirstSwimmer()
        {
            string query = "select * from v発生新記録 where 大会番号=@eventNo and 中間新記録マーク<>''";
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    using (SqlDataReader reader = command.ExecuteReader())

                    {
                        while (reader.Read())
                        {
                            int strokeCode = ConvToInt(reader["種目コード"]);
                            if (strokeCode < 6) continue;
                            int genderCode = ConvToInt(reader["性別コード"]);
                            int distanceCode = ConvToInt(reader["距離コード"]);
                            string swimmer1 = (string)reader["第1泳者"];
                            string shozoku = (string)reader["所属・チーム名"];
                            string hizuke = (string)reader["日付"];
                            string myStrTime = Convert.ToString(reader["中間記録"]) ?? "";
                            int classNo = Convert.ToInt32(reader["第一泳者新記録判定クラス"]);
                            int strokeCode2 = strokeCode - 5;
                            int distanceCode2 = distanceCode - 2;

                            int mytime = TimeStr2Int(myStrTime);
                            string gameRecord = GetGameRecord(genderCode, classNo, distanceCode2, strokeCode2);
                            int thisGR = TimeStr2Int(gameRecord);
                            if (mytime < thisGR)
                            {
                                UpdateGameRecord(genderCode, classNo, distanceCode2, strokeCode2,
                                    myStrTime, swimmer1, "", "", "", shozoku);
                            }
                        }
                    }
                }
            }
        }
        private static int GetMaxClassNo()
        {
            string query = "select MAX(クラス番号) from クラス where 大会番号=@eventNo";

            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    object reader = command.ExecuteScalar();
                    if (reader != null)
                    {
                        return Convert.ToInt32(reader.ToString());
                    }
                    else return 0;
                }
            }

        }
        public static void Excel2DB()
        {
            MessageBox.Show("次の画面で大会記録の入ったExcel File を選択してください。", 
                "Excel File 選択", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Cursor.Current = Cursors.WaitCursor;
            string excelFile;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = " 大会記録の入ったExcel を選択";
                ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                excelFile = ofd.FileName;
            }
            try
            {
                ExcelPhoneticCleaner.RemovePhoneticRuns(excelFile);
                using (var wb = new XLWorkbook(excelFile))
                {
                    string[] gender = new string[] { "", "男子", "女子", "混成", "混合" };
                    int classNo, distanceCode, styleCode, genderCode;
                    for (classNo = 1; classNo <= GetMaxClassNo(); classNo++)
                    {
                        for (distanceCode = 1; distanceCode <= 7; distanceCode++)
                        {
                            for (styleCode = 1; styleCode <= 7; styleCode++)
                            {
                                for (genderCode = 1; genderCode <= 4; genderCode++)
                                {
                                    string prefix = gender[genderCode] + classNo + "_" + distanceCode + "_" + styleCode;
                                    string timeRange = prefix + "Time";
                                    string timeValue = Convert.ToString(WinnerList.TryGetValue(wb, timeRange));
                                    if (timeValue != "")
                                    {
                                        string nameRange = prefix + "Name";
                                        string teamRange = prefix + "Team";
                                        string dateRange = prefix + "GR"; 
                                        string swimmerName = WinnerList.TryGetValue(wb, nameRange);
                                        string swimmerName2 = swimmerName.Replace("\n", "・");
                                        string teamName = WinnerList.TryGetValue(wb, teamRange);
                                        string dateName = WinnerList.TryGetValue(wb, dateRange);
                                        if (ExistRecord(genderCode,classNo,distanceCode,styleCode)) 
                                            UpdateGameRecord(genderCode, classNo, distanceCode, styleCode, timeValue, swimmerName2, teamName,dateName);
                                        else InsertGameRecord(genderCode,classNo, distanceCode, styleCode, timeValue,swimmerName2,teamName,dateName);

                                    }
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("DB Update 終了");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel2DBでエラー。" + ex.Message);
            }

        }
        public static void UpdateGameRecord()
        {
            string query = "select * from v発生新記録 where 大会番号=@eventNo";
            
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int genderCode = ConvToInt(reader["性別コード"]);
                            int strokeCode = ConvToInt(reader["種目コード"]);
                            int distanceCode = ConvToInt(reader["距離コード"]);
                            string swimmer1 = Convert.ToString(reader["第1泳者"]) ?? "";
                            string swimmer2 = Convert.ToString(reader["第2泳者"]) ?? "";
                            string swimmer3 = Convert.ToString(reader["第3泳者"]) ?? "";
                            string swimmer4 = Convert.ToString(reader["第4泳者"]) ?? "";
                            string shozoku = Convert.ToString(reader["所属・チーム名"]) ?? "";
                            string hizuke = Convert.ToString(reader["日付"]) ?? "";
                            //venue = (string)reader["開催地"];
                            ////eventName = (string) reader["競技会名"];
                            int classNo = ConvToInt(reader["新記録判定クラス"]);
                            string myStrTime = Convert.ToString( reader["ゴール"]) ?? "";
                            
                            int mytime = TimeStr2Int(myStrTime);
                            string gameRecord = GetGameRecord(genderCode, classNo, distanceCode, strokeCode);
                            int thisGR =  TimeStr2Int( gameRecord);
                            if ( mytime<thisGR)
                            {
                                UpdateGameRecord(genderCode, classNo, distanceCode, strokeCode,
                                        myStrTime, swimmer1, swimmer2, swimmer3, swimmer4, shozoku);
                            }
                        }
                    }
                }
            }
        }
        private static bool ExistRecord(int gender, int classNo, int distanceCode, int strokeCode) {
            string query = @"select 1 from 新記録 
                                where 記録区分番号=@classNo 
                                  and 距離コード=@distanceCode
                                  and 種目コード=@strokeCode
                                  and 性別コード=@genderCode
                                  and 大会番号=@eventNo";
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@eventNo", SqlDbType.Int).Value = GlobalV.config.EventNo;
                    command.Parameters.AddWithValue("@genderCode", SqlDbType.Int).Value = gender;
                    command.Parameters.AddWithValue("@distanceCode", SqlDbType.Int).Value = distanceCode;
                    command.Parameters.AddWithValue("@strokeCode", SqlDbType.Int).Value = strokeCode;
                    command.Parameters.AddWithValue("@classNo", SqlDbType.Int).Value = classNo;
                    object reader = command.ExecuteScalar();
                    if (reader != null)
                    {
                        return true;
                    }
                    else return false;
                }
            }
        }
        private static void UpdateGameRecord(int genderCode, int classNo, int distanceCode,
            int strokeCode, string timeValue, string swimmer, string shozoku ,string eventName="") // eventNameに年度か回数をいれる。
        {
            string myQuery = @" 
                update 新記録 set 記録=@timeValue , 記録保持者=@Name, 所属=@Shozoku, 
                       競技会名=@eventName, 会場=@eventVenue
                       where 大会番号=@eventNo
                       and 性別コード=@gender
                       and 記録区分番号=@classNo
                       and 記録名称番号=1 
                       and 種目コード=@strokeCode
                       and 距離コード=@distanceCode";
            using (SqlConnection connection = new SqlConnection(GlobalV.config.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.CommandType = CommandType.Text; // これはあってもなくてもいいですが、明示的に指定することができます。
                    command.Parameters.AddWithValue("@eventNo",  SqlDbType.Int).Value=GlobalV.config.EventNo;
                    command.Parameters.AddWithValue("@gender", SqlDbType.Int).Value =genderCode;
                    command.Parameters.AddWithValue("@strokeCode", SqlDbType.Int).Value =strokeCode;
                    command.Parameters.AddWithValue("@distanceCode", SqlDbType.Int).Value =distanceCode;
                    command.Parameters.AddWithValue("@classNo", SqlDbType.Int).Value =classNo;
                    command.Parameters.Add("@timeValue", SqlDbType.VarChar,8).Value= timeValue;
                    command.Parameters.Add("@Name", SqlDbType.VarChar,30).Value=(object)swimmer ?? DBNull.Value; 
                    command.Parameters.Add("@shozoku",SqlDbType.VarChar,16).Value=(object)shozoku ?? DBNull.Value;
                    command.Parameters.Add("@eventName", SqlDbType.NVarChar, 50).Value = eventName;
                    command.Parameters.Add("@eventVenue", SqlDbType.NVarChar, 30).Value = WinnerList.EventVenue;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ExeExport(/*object sender, EventArgs e*/)
        {
            string sql = "SELECT * FROM 新記録bk where 大会番号=@eventNo"; 


            //  SQLを実行して DataTable に取得
            DataTable table = new DataTable();
            try
            {
                using (var con = new SqlConnection(GlobalV.config.ConnectionString))
                using (var cmd = new SqlCommand(sql, con))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@eventNo", GlobalV.config.EventNo);
                    con.Open();
                    adapter.Fill(table);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL実行中にエラー：" + ex.Message);
                return;
            }

            // 4. 保存先ファイルを選択
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "TXTファイル|*.txt";
                sfd.Title = "updateした大会記録を保存します。保存先を指定してください";
                sfd.FileName = "新記録.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName, false, Encoding.Unicode))
                        {
                            // ヘッダー
                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                writer.Write(table.Columns[i].ColumnName);
                                if (i < table.Columns.Count - 1) writer.Write(",");
                            }
                            writer.WriteLine();

                            // データ
                            foreach (DataRow row in table.Rows)
                            {
                                for (int i = 0; i < table.Columns.Count; i++)
                                {
                                    string field = row[i]?.ToString()?.Replace("\"", "\"\"") ?? "";
                                    writer.Write("\"" + field + "\"");
                                    if (i < table.Columns.Count - 1) writer.Write(",");
                                }
                                writer.WriteLine();
                            }
                        }

                        MessageBox.Show("保存完了");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("保存中にエラー：" + ex.Message);
                    }
                }
            }
        }

    }

}
