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
                string myQuery = @" select MAX(�I��ԍ�) as MAX from �I�� where ���ԍ�=@eventNo";
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
                string myQuery = @"update �L�^ set �W���L�^����N���X=@sClass 
                                   where �I��ԍ�=@swimmerID and ���Z�ԍ�=@UID and �g=@kumi 
                                       and ���H=@laneNo and ���ԍ�=@eventNo";
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
                string myQuery = @"select �I��ԍ�, �W���L�^����N���X, ��ڃR�[�h, �����R�[�h from �G���g���[ where ���ԍ�=@eventNo";
                using (SqlCommand myCommand = new(myQuery, conn))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sClass[Convert.ToInt32(reader["�I��ԍ�"])] = Convert.ToInt32(reader["�W���L�^����N���X"]);
                        }
                    }
                }
                myQuery = @"SELECT ���Z�ԍ�, �g, ���H, �I��ԍ� from �L�^ where ���ԍ�=@eventNo";
                using (SqlCommand myCommand = new(myQuery, conn))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", GlobalV.EventNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int UID = Convert.ToInt32(reader["���Z�ԍ�"]);
                            int kumi = Convert.ToInt32(reader["�g"]);
                            int laneNo = Convert.ToInt32(reader["���H"]);
                            int swimmerID = Convert.ToInt32(reader["�I��ԍ�"]);
                            UpdateOneRecord(UID, kumi, laneNo, swimmerID, sClass[swimmerID]);
                        }

                    }
                }
            }

        }
    }
}

