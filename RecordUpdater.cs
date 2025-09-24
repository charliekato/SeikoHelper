using Microsoft.Data.SqlClient;
namespace SeikoHelper
{
    public static class RecordUpdater
    {
        private static int GetMaxSwimmerNo()
        {
            using (SqlConnection conn = new SqlConnection(GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord))
            {
                conn.Open();
                string myQuery = @" select MAX(選手番号) as MAX from 選手 where 大会番号=@eventNo";
                using (SqlCommand myCommand = new(myQuery, conn))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                            return Convert.ToInt32(reader["MAX"]);
                    }
                }

            }
            return 0;
        }
        static void UpdateOneRecord(int UID, int kumi, int laneNo, int swimmerID, int sClass)
        {
            using (SqlConnection conn = new SqlConnection(GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord))
            {
                conn.Open();
                string myQuery = @"update 記録 set 標準記録判定クラス=@sClass 
                                   where 選手番号=@swimmerID and 競技番号=@UID and 組=@kumi 
                                       and 水路=@laneNo and 大会番号=@eventNo";
                using (SqlCommand myCommand = new(myQuery, conn))
                {
                    myCommand.Parameters.AddWithValue("@sClass", sClass);
                    myCommand.Parameters.AddWithValue("@swimmerID", swimmerID);
                    myCommand.Parameters.AddWithValue("@UID", UID);
                    myCommand.Parameters.AddWithValue("@kumi", kumi);
                    myCommand.Parameters.AddWithValue("@laneNo", laneNo);
                    myCommand.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                    myCommand.ExecuteNonQuery();
                }
            }
        }
        public static void GoUpdate()
        {
            int numSwimmer = GetMaxSwimmerNo()+1;
            int[] sClass = new int[numSwimmer];
            //int[] styleNo = new int[numSwimmer];
            //int[] distanceCode = new int[numSwimmer];

            using (SqlConnection conn = new SqlConnection(GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord))
            {
                conn.Open();
                string myQuery = @"select 選手番号, 標準記録判定クラス, 種目コード, 距離コード from エントリー where 大会番号=@eventNo";
                using (SqlCommand myCommand = new(myQuery, conn))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sClass[Convert.ToInt32(reader["選手番号"])] = Convert.ToInt32(reader["標準記録判定クラス"]);
                        }
                    }
                }
                myQuery = @"SELECT 競技番号, 組, 水路, 選手番号 from 記録 where 大会番号=@eventNo";
                using (SqlCommand myCommand = new(myQuery, conn))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int UID = Convert.ToInt32(reader["競技番号"]);
                            int kumi = Convert.ToInt32(reader["組"]);
                            int laneNo = Convert.ToInt32(reader["水路"]);
                            int swimmerID = Convert.ToInt32(reader["選手番号"]);
                            UpdateOneRecord(UID, kumi, laneNo, swimmerID, sClass[swimmerID]);
                        }

                    }
                }
            }

        }
    }
}

