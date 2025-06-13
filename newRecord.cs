using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Data.SqlClient;

namespace SeikoHelper
{

    public static class NewRecordExporter /*: Form*/
    {
        private static int TimeStr2Int(string myTime)
        {
            string temp = myTime.Replace(".", "");
            return  Convert.ToInt32(temp.Replace(":",""));
        }
        private static void InsertGameRecord(int genderCode, int classNo, int distanceCode, int strokeCode, string GR)
        {
            string myQuery;
            if ( classNo>0)
            {

                myQuery= @" 
                     insert into 新記録 ( 大会番号, 性別コード, 記録区分番号,
                     記録名称番号, 種目コード, 距離コード, 記録) values
                     ( @eventNo, @gender, @classNo, 1, @strokeCode, @distanceCode, @GR ) ";
            } else
            {
                myQuery = @"  insert into 新記録 ( 大会番号, 性別コード, 
                     記録名称番号, 種目コード, 距離コード, 記録) values
                     ( @eventNo, @gender, 1, @strokeCode, @distanceCode, @GR )";
            }
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@eventNo", SqlDbType.Int).Value = GlobalV.EventNo;
                    command.Parameters.Add("@gender", SqlDbType.Int).Value = genderCode;
                    command.Parameters.Add("@strokeCode", SqlDbType.Int).Value = strokeCode;
                    command.Parameters.Add("@distanceCode", SqlDbType.Int).Value = distanceCode;
                    command.Parameters.Add("@classNo", SqlDbType.Int).Value = classNo;
                    command.Parameters.Add("@GR", SqlDbType.NVarChar).Value = GR;
                    command.ExecuteNonQuery();
                }
            }
        }
        private static void UpdateGameRecord(int genderCode, int classNo, int distanceCode, int strokeCode, string GR)
        {
            string myQuery = @" 
                      update 新記録 set 記録=@GR where 大会番号=@eventNo
                       and 性別コード=@gender
                       and 記録区分番号=@classNo
                       and 記録名称番号=1 
                       and 種目コード=@strokeCode
                       and 距離コード=@distanceCode";
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@eventNo", SqlDbType.Int).Value = GlobalV.EventNo;
                    command.Parameters.Add("@gender", SqlDbType.Int).Value = genderCode;
                    command.Parameters.Add("@strokeCode", SqlDbType.Int).Value = strokeCode;
                    command.Parameters.Add("@distanceCode", SqlDbType.Int).Value = distanceCode;
                    command.Parameters.Add("@classNo", SqlDbType.Int).Value = classNo;
                    command.Parameters.Add("@GR", SqlDbType.NVarChar).Value = GR;
                    command.ExecuteNonQuery();
                }
            }
        }
        private static  string GetGameRecord(int genderCode, int classNo, int distanceCode, int strokeCode)
        {
            string myQuery = @"
              select 記録 from 新記録 where 大会番号=@eventNo 
                   and 性別コード=@gender
                   and 記録区分番号=@classNo
                   and 記録名称番号=1 
                   and 種目コード=@strokeCode
                   and 距離コード=@distanceCode";
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;
            string GR = "";
            using ( SqlConnection connection=new SqlConnection(connectionString))
            {

               connection.Open();
                using (SqlCommand command = new SqlCommand(myQuery, connection))
                {
                    command.Parameters.Add("@eventNo", SqlDbType.Int).Value = GlobalV.EventNo;
                    command.Parameters.Add("@gender", SqlDbType.Int).Value = genderCode;
                    command.Parameters.Add("@strokeCode", SqlDbType.Int).Value = strokeCode;
                    command.Parameters.Add("@distanceCode", SqlDbType.Int).Value = distanceCode;
                    command.Parameters.Add("@classNo", SqlDbType.Int).Value = classNo;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            GR = (string)reader["記録"];
                        else GR = "";

                    }
                } 
            }
            
            return GR;

        }

        public static string ConvToString(object value)
        {
            if (value == null || value == DBNull.Value)
                return "";
            return (string)value;
        }

        private static bool ClassExist()
        {
            string myQuery = "select 1 from クラス where 大会番号 = @eventNo";
            using (SqlConnection conn = new SqlConnection(GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(myQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EventNo", GlobalV.EventNo);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return false;
                        return true;

                    }
                }
            }
        }
        public static int ConvToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            return Convert.ToInt32(value);
        }
        public static void UpdateGameRecord()
        {
            string mySql;
            bool classExist = ClassExist();
            if (classExist) {
                mySql= @"
                WITH 最速記録 AS (
                        SELECT 
                            記録.競技番号,
                            記録.ゴール, 
                            記録.新記録判定クラス,
                             rank() over (partition by 記録.競技番号, 記録.新記録判定クラス ORDER BY 記録.ゴール ASC) AS 順位
                       FROM 記録
                        WHERE 記録.事由表示 = 0 and 選手番号>0 and 記録.大会番号=@eventNo
                    )
                    SELECT 
                        プログラム.表示用競技番号,
                        最速記録.新記録判定クラス,
                        プログラム.性別コード ,
                        プログラム.距離コード, 
                        プログラム.種目コード,
                        最速記録.ゴール as 記録
                    FROM プログラム
                    INNER JOIN 最速記録 ON 最速記録.競技番号 = プログラム.競技番号 AND 最速記録.順位 = 1
                    WHERE プログラム.大会番号 = @eventNo
                    ORDER BY プログラム.表示用競技番号;";

            } else
            {
                 mySql= @"
                    WITH 最速記録 AS (
                        SELECT 
                            記録.競技番号,
                            記録.ゴール, 
                             rank() over (partition by 記録.競技番号 ORDER BY 記録.ゴール ASC) AS 順位
                       FROM 記録
                        WHERE 記録.事由表示 = 0 and 選手番号>0 and 記録.大会番号=@eventNo
                    )
                    SELECT 
                        プログラム.表示用競技番号,
                        プログラム.性別コード ,
                        プログラム.距離コード, 
                        プログラム.種目コード,
                        最速記録.ゴール as 記録
                    FROM プログラム
                    INNER JOIN 最速記録 ON 最速記録.競技番号 = プログラム.競技番号 AND 最速記録.順位 = 1
                    WHERE プログラム.大会番号 = @eventNo
                    ORDER BY プログラム.表示用競技番号;";
            }
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(mySql, connection))
                {
                    command.Parameters.Add("@eventNo", SqlDbType.Int).Value = GlobalV.EventNo;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int genderCode;
                            int strokeCode;
                            int distanceCode;
                            int classNo;
                            string myStrTime;
                            //string myStrTime = (string) reader["ゴール"];
                            int mytime;
                            int thisGR;
                            string gameRecord;
                            genderCode = ConvToInt(reader["性別コード"]);
                            strokeCode = ConvToInt(reader["種目コード"]);
                            distanceCode = ConvToInt(reader["距離コード"]);
                            if (classExist)
                                classNo = ConvToInt(reader["新記録判定クラス"]);
                            else classNo = 0;
                            myStrTime = ConvToString( reader["記録"]);
                            mytime = TimeStr2Int(myStrTime);
                            gameRecord = GetGameRecord(genderCode, classNo, distanceCode, strokeCode);
                            if (gameRecord=="" )
                                InsertGameRecord(genderCode,classNo,distanceCode,strokeCode,myStrTime);
                            else
                            {

                                thisGR =  TimeStr2Int( gameRecord);
                                if ( mytime<thisGR)
                                {
                                    UpdateGameRecord(genderCode, classNo, distanceCode, strokeCode, myStrTime );
                                }

                            }



                        }

                    }
                }
            }
        }
        public static void ExeExport(/*object sender, EventArgs e*/)
        {
            // 1. SQL文を指定
            string sql = "SELECT * FROM 新記録 where 大会番号=@eventNo"; 

            // 2. 接続文字列
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;

            // 3. SQLを実行して DataTable に取得
            DataTable table = new DataTable();
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(sql, con))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
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
                        using (var writer = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
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
