#define TRADITIONAL
using Microsoft.Data.SqlClient;
//using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Forms.VisualStyles;

namespace GodoCreater
{
    

    public static class GoDogen
    {
        public static string ConnectionString { get; set; } = string.Empty;
        public static bool Strategy1 { get; set; } = false;
        public static bool Strategy2 { get; set; } = false;
        static int MaxKumi = 0;
        static int MaxPrgNo = 0;
        static int MaxUID = 0;
        public static int EventNo { get; set; } = 0;
        static int[] PrgNobyUID = [];
        static int[]  ShumokubyPrgNo = [];
        static int[] NumSwimmers = [];
        static int[] NumSwimmersbyTID = [];
        static int[] TIDarray = [];
        static int[] DistancebyPrgNo = [];
        static int[] UIDFromPrgNo = [];
        static int[]  ClassbyPrgNo = [];
        static int[]  GenderbyPrgNo = [];
        static int[] Phase = [];
        static int MaxLaneNo4Yosen = 0;
        static int MaxLaneNo4TimeFinal = 0;
        static int MaxLaneNo4Final = 0;
        static int ZeroLaneUse = 0;



        private static string[] ShumokuTable = { "",
            "自由形","背泳ぎ","平泳ぎ", "バタフライ", "個人メドレー", "リレー", "メドレーリレー"};
        private static string[] DistanceTable = { "",
          "  25m" , "  50m" , " 100m" , " 200m" , " 400m" , " 800m" , "1500m"};
        private static string[] PHASENAME = { "", "予選", "準決勝", "タイム決勝", "Ｂ決勝", "Ａ決勝", "決勝", "スイムオフ" };
        private static int GoDoClass = 0;
        public static void PrepareGO()
        {
            ReadEventDb();
            ReadProgramDB();
            GetNumberOfSwimmers();
        }
        public static void Scenario1()
        {
            SetTID();
            AdjustStartLane();
           
        }
        public static void Scenario2()
        {
            DeleteGoDoTable();
            CreateGoDoTable();
        }
        public static void Scenario3(bool scenario4Flag)
        {
            if (scenario4Flag)
            {
                GoDoClass = GetOrCreateGodoClass();
            }

            MoveSwimmerAll(scenario4Flag);
        }
        public static int GetRaceNo(int prgNo, int kumi) {
            return (prgNo - 1) * MaxKumi + kumi - 1;
        }

        static bool GetNextRace(ref int prgNo, ref int kumi)
        {
            while(true) { 
                kumi++;
                if (kumi <=MaxKumi)
                    if (HowManySwimmers(prgNo, kumi) > 0) return true;

                kumi = 0;
                prgNo++;
                if (prgNo > MaxPrgNo) return false;
            }
        }
              /// <summary>
        /// if race which is the given prgNo and Kumi exists return true, otherwise false.
        /// </summary>
        /// <param name="prgNo"></param>
        /// <param name="kumi"></param>
        /// <returns></returns>
        static int HowManySwimmers(int prgNo, int kumi=1)
        {
            using (SqlConnection myCon = new SqlConnection(GoDogen.ConnectionString))
            {
                string myQuery = @"SELECT  count(選手番号) as numSwimmer
                FROM 記録 WHERE 組= @kumi  AND 競技番号=(
                    select 競技番号 from プログラム where 表示用競技番号=@prgNo
                      and 大会番号=@eventNo)
                 and 大会番号= @eventNo and 選手番号>0 ";
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@prgNo", prgNo);
                    myCommand.Parameters.AddWithValue("@kumi", kumi);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        int numSwimmer;
                        if (reader.Read())
                        {
                            numSwimmer = Convert.ToInt32(reader["numSwimmer"]);
                            return numSwimmer;
                        }
                    }

                }
            }
            return 0;
        }

        static int HowManySwimmers(int prgNo )
        {
            using (SqlConnection myCon = new SqlConnection(GoDogen.ConnectionString))
            {
                string myQuery = @"SELECT  count(1) as numSwimmer
                FROM 記録 WHERE  競技番号=(
                    select 競技番号 from プログラム where 表示用競技番号=@prgNo
                      and 大会番号=@eventNo)
                 and 大会番号= @eventNo and 選手番号>0 ";
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@prgNo", prgNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        int numSwimmer;
                        if (reader.Read())
                        {
                            numSwimmer = Convert.ToInt32(reader["numSwimmer"]);
                            return numSwimmer;
                        }
                    }

                }
            }
            return 0;
        }

        static int GetOrCreateGodoClass()
        {
            string className = "合同レース";
            string myQuery = "select クラス番号 FROM クラス WHERE クラス名称 = @className and 大会番号 = @eventNo ";
            int classNo=0; // for 合同レース
            using (SqlConnection myCon = new SqlConnection(GoDogen.ConnectionString))
            {

                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@className", className);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["クラス番号"]);
                        }
                    }


                }
                // 合同レース　クラス　not yet exists
                myQuery = "SELECT MAX(クラス番号) AS MAXCLASS FROM クラス where 大会番号 = @eventNo ";
                using (SqlCommand myCommand = new SqlCommand(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            classNo= Convert.ToInt32(reader["MAXCLASS"])+1;
                        }
                    }
                }
                if (classNo>0) {
                    myQuery = @"INSERT INTO クラス(クラス番号, クラス名称, 大会番号)
                                 VALUES(@classNo, @className, @eventNo)";
                    using (SqlCommand myCommand = new SqlCommand(myQuery, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@classNo", classNo);
                        myCommand.Parameters.AddWithValue("@className", className);
                        myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                        myCommand.ExecuteNonQuery();
                    }
                }
            }
            return classNo;
        }

        static void ReadEventDb() // read event table again to get maxlane and erolaneuse
        {
            using (SqlConnection myCon = new SqlConnection(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = "SELECT 使用水路予選, 使用水路タイム決勝, 使用水路決勝, ゼロコース使用 " +
                                 "FROM 大会設定 WHERE 大会番号 = " + EventNo;

                using (SqlCommand myCommand = new SqlCommand(myQuery, myCon))
                {
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            MaxLaneNo4Yosen = Convert.ToInt32(reader["使用水路予選"]);
                            MaxLaneNo4TimeFinal = Convert.ToInt32(reader["使用水路タイム決勝"]);
                            MaxLaneNo4Final = Convert.ToInt32(reader["使用水路決勝"]);
                            ZeroLaneUse = Convert.ToInt32(reader["ゼロコース使用"]);
                        }
                    }
                }

            }
        }
        
        public static void GetNumberOfSwimmers()
        {
            int prgNo = 1;
            int kumi = 0;
            int numSwimmer = 0;
            while (true)
            {
                kumi++;
                numSwimmer = HowManySwimmers(prgNo, kumi);
                if (numSwimmer>0) NumSwimmers[GetRaceNo(prgNo, kumi)] = numSwimmer;
                else
                {
                    kumi = 0;
                    prgNo++;
                    if (prgNo > MaxPrgNo) return;
                }

            }

        }
        public static int GetMaxPrgNo()
        {
            string myQuery = @"
            select MAX(表示用競技番号) as MaxPrgNo from プログラム
            where 大会番号=@eventNo ";
            int maxPrgNo = 0;

            using (SqlConnection conn = new SqlConnection( GoDogen.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(myQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EventNo", EventNo);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            maxPrgNo = Convert.ToInt32(reader["MaxPrgNo"]);
                        }
                    }
                }
            }
            return maxPrgNo;
        }
        static void InitProgramDBArray( SqlConnection myConn )
        {
            string myQuery = "select max(競技番号) as uid from プログラム where 大会番号= @eventNo" ;
            using (SqlCommand sqlCommand = new(myQuery, myConn))
            {
                sqlCommand.Parameters.AddWithValue("@eventNo", EventNo);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        MaxUID = Convert.ToInt32(reader["UID"]);
                    }
;
                }
            }
            myQuery = "select max(表示用競技番号) as MaxPrgNo from プログラム where 大会番号= @eventNo";
            using (SqlCommand sqlCommand = new(myQuery, myConn))
            {
                sqlCommand.Parameters.AddWithValue("@eventNo", EventNo);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        MaxPrgNo = Convert.ToInt32(reader["MaxPrgNo"]);
                    }
                }
            }
            myQuery = "select max(組) as MaxKumi from 記録 where 大会番号= @eventNo";
            using (SqlCommand sqlCommand = new(myQuery, myConn))
            {
                sqlCommand.Parameters.AddWithValue("@eventNo", EventNo);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        MaxKumi = Convert.ToInt32(reader["MaxKumi"]);
                    }
                }
            }
            ResizePrgArray();
        }

        static void SetTID()
        {
            int prgNo = 1;
            int kumi = 1;
            do
            {
                if (prgNo==34 )
                {
                    MessageBox.Show("debug");

                }
                if (TIDarray[GetRaceNo(prgNo, kumi)] == 0)
                    CheckIfTogetherable(prgNo, kumi);
            } while (GetNextRace(ref prgNo, ref kumi));
        }
        static bool NextKumiExist(int prgNo, int kumi)
        {
            int prgNoSave = prgNo;
            GetNextRace(ref prgNo, ref kumi);
            if (prgNo == prgNoSave) return true;
            return false;
        }
        static void CheckIfTogetherable(int prgNo, int kumi )
        {
            int subTotal=0;
            if (subTotal == MaxLaneNo4TimeFinal) return;
            if (NextKumiExist(prgNo,kumi) ) return;
            int prevPrgNo;
            int prevKumi;
            int kumiSave=kumi;
            int TID = prgNo;
            subTotal += NumSwimmers[GetRaceNo(prgNo, kumi)];
            while ( subTotal < MaxLaneNo4TimeFinal)
            {
                prevPrgNo = prgNo;
                prevKumi = kumi;
                if (GetNextRace(ref prgNo, ref kumi))
                {
                    if (!Strategy2 && (kumi > 1 || prevKumi > 1)) return;
                    if (!Strategy1 && (GenderbyPrgNo[prevPrgNo - 1] != GenderbyPrgNo[prgNo - 1])) return;
                    if (DistancebyPrgNo[prevPrgNo - 1] != DistancebyPrgNo[prgNo - 1]) return;
                    if (ShumokubyPrgNo[prevPrgNo - 1] != ShumokubyPrgNo[prgNo - 1]) return;
                    if (Phase[prevPrgNo - 1] != Phase[prgNo - 1]) return;
                    subTotal += NumSwimmers[GetRaceNo(prgNo, kumi)] ;
                    if (subTotal <= MaxLaneNo4TimeFinal)
                    {
                        TIDarray[GetRaceNo(prevPrgNo, prevKumi)] = TID;
                        TIDarray[GetRaceNo(prgNo, kumi)] = TID;
                        NumSwimmersbyTID[GetRaceNo(TID, kumiSave)] = subTotal;
                    }
                }
                else return;
            }
        }
        static void AdjustStartLane()
        {
            int tid = 0;
            int prevPrgNo=0;
            int prgNo=1;
            int kumi = 0;
            int prevKumi = 0;
            int startLane=0;
            int toPrgNo = 0;
            int toKumi = 0;
            int raceNo = 0;
            int thisTID = 0;
            while( GetNextRace(ref prgNo, ref kumi) )
            {
                raceNo = GetRaceNo(prgNo, kumi);
                thisTID = TIDarray[raceNo];
                if (thisTID>0)
                {
                    if (tid <thisTID)
                    {
                        toPrgNo = prgNo;
                        toKumi = kumi;

                        startLane = GetStartLane(NumSwimmersbyTID[raceNo]);
                        tid = thisTID;
                    }
                    else
                    {
                        startLane = startLane + NumSwimmers[GetRaceNo(prevPrgNo,prevKumi)];
                    }
                    MoveSwimmer(prgNo,kumi, startLane);
                }
                prevPrgNo = prgNo;
                prevKumi = kumi;
            }
                
        }
        static void ChangeGender(int prgNo)
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @"
                      update  プログラム set 性別コード=@gender where 表示用競技番号=@prgNo and 大会番号=@eventNo";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@prgNo", prgNo);
                    myCommand.Parameters.AddWithValue("@gender", 3); //混成
                    myCommand.ExecuteNonQuery();
                }
            }
         }
        static void ChangeClass(int prgNo,int classNo)
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @"
                      update  プログラム set クラス番号=@classNo where 表示用競技番号=@prgNo and 大会番号=@eventNo";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@prgNo", prgNo);
                    myCommand.Parameters.AddWithValue("@classNo", classNo);
                    myCommand.ExecuteNonQuery();
                }
            }
                    
        }
        static void DeleteProgram(int prgNo)
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @"
                      delete  from プログラム where 表示用競技番号=@prgNo and 大会番号=@eventNo";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@prgNo", prgNo);
                    myCommand.ExecuteNonQuery();
                }
            }
                    
        }
/// <summary>
/// GetFirstLane
///     get the first lane on which is occupied by a swimmer.
/// </summary>
/// <param name="prgNo"></param>
/// <param name="kumi"></param>
/// <returns> returns 0 if the race does not exist or nobody occupied any lane.</returns>
        static int GetFirstLane(int prgNo, int kumi)
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @"select min(水路) as firstLane  from 記録
                 where 競技番号=( select 競技番号 from プログラム where 表示用競技番号=@prgNo 
                   and 大会番号=@eventNo ) and 組=@kumi and 大会番号=@eventNo
                   and 選手番号>0";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@prgNo", prgNo);
                    myCommand.Parameters.AddWithValue("@kumi", kumi);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["firstLane"]);
                        }
                    }
                }
            }
            return 0;

        }
/// <summary>
/// GetlastLane
///     get the last lane no which is occupied by a swimmer.
/// </summary>
/// <param name="prgNo"></param>
/// <param name="kumi"></param>
/// <returns> returns 0 if the race does not exist or nobody occupied any lane.</returns>
        static int GetLastLane(int prgNo, int kumi)
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @" select MAX(水路) as LastLane  from 記録
                 where 競技番号=( select 競技番号 from プログラム where 表示用競技番号=@prgNo 
                  and 大会番号=@eventNo ) and 大会番号=@eventNo
                  and 選手番号>0 and 組=@kumi";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@prgNo", prgNo);
                    myCommand.Parameters.AddWithValue("@kumi", kumi);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["LastLane"]);
                        }
                    }
                }
            }
            return 0;

        }
        static void MoveSwimmer(int prgNo, int kumi, int startLane)
        {
            int uid = UIDFromPrgNo[prgNo-1];
            int[] thisSwimmerID = new int[10];
            string[] entryTime = new string[10];
            Array.Fill(entryTime, string.Empty);
            string[] goal = new string[10];
            Array.Fill(goal, string.Empty);
            int[] classNo = new int[10];
            int[] certNo = new int[10];
            int[] firstOne = new int[10];
            int[] secondOne = new int[10];
            int[] thirdOne = new int[10];
            int[] lastOne = new int[10];
            int[] firstOneClassNo = new int[10];
            int[] firstOneCertNo = new int[10];
            int[] laneNo = new int[10];
            
            int lane;
            int scounter = 0;
            int maxScounter = 0;
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @"
                      select * from 記録 where 競技番号=@uid and 大会番号=@eventNo and 組=@kumi 
                        order by 水路";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@uid", uid);
                    myCommand.Parameters.AddWithValue("@kumi", kumi);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            thisSwimmerID[scounter] = Convert.ToInt32(reader["選手番号"]);
                            if (thisSwimmerID[scounter] > 0)
                            {
                                entryTime[scounter] = (string)reader["エントリータイム"];
                                classNo[scounter] = Convert.ToInt32(reader["新記録判定クラス"]);
                                certNo[scounter] = Convert.ToInt32(reader["標準記録判定クラス"]);
                                firstOne[scounter] = Convert.ToInt32(reader["第１泳者"]);
                                secondOne[scounter] = Convert.ToInt32(reader["第２泳者"]);
                                thirdOne[scounter] = Convert.ToInt32(reader["第３泳者"]);
                                lastOne[scounter] = Convert.ToInt32(reader["第４泳者"]);
                                firstOneClassNo[scounter] = Convert.ToInt32(reader["第一泳者新記録判定クラス"]);
                                firstOneCertNo[scounter] = Convert.ToInt32(reader["第一泳者標準記録判定クラス"]);
                                laneNo[scounter] = Convert.ToInt32(reader["水路"]);
                                goal[scounter] = (string)reader["ゴール"];
                                scounter++;

                            }
                        }
                        maxScounter = scounter;
                    }
                }
                scounter = 0;
                uid=UIDFromPrgNo[prgNo-1];
                for (lane = 1; lane<=MaxLaneNo4TimeFinal;lane++)
                {

                    myQuery = @" Update 記録
　　　　　　　　　　　　　　set 選手番号=@sid, エントリータイム=@entryTime, 
                         新記録判定クラス=@classNo, 標準記録判定クラス=@certNo, 
                         第１泳者=@firstOne , 第２泳者 = @secondOne ,
                         第３泳者=@thirdOne , 第４泳者 = @lastOne ,
                         第一泳者新記録判定クラス=@firstOneClassNo ,
                         第一泳者標準記録判定クラス=@firstOneCertNo,
                         ゴール=@goal
                      where 大会番号=@eventNo and 競技番号=@uid and 組=@tokumi and 水路=@laneNo";

                    using (SqlCommand myCommand = new(myQuery, myCon))
                    {
                        if (lane < startLane)
                        {
                            myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                            myCommand.Parameters.AddWithValue("@sid", 0);
                            myCommand.Parameters.AddWithValue("@entryTime", "");
                            myCommand.Parameters.AddWithValue("@classNo", 0);
                            myCommand.Parameters.AddWithValue("@certNo", 0);
                            myCommand.Parameters.AddWithValue("@firstOne", 0);
                            myCommand.Parameters.AddWithValue("@secondOne", 0);
                            myCommand.Parameters.AddWithValue("@thirdOne", 0);
                            myCommand.Parameters.AddWithValue("@lastOne", 0);
                            myCommand.Parameters.AddWithValue("@firstOneClassNo", 0);
                            myCommand.Parameters.AddWithValue("@firstOneCertNo", 0);
                            myCommand.Parameters.AddWithValue("@uid", uid);
                            myCommand.Parameters.AddWithValue("@tokumi", kumi);
                            myCommand.Parameters.AddWithValue("@laneNo", lane);
                            myCommand.Parameters.AddWithValue("@goal", "");

                        }
                        else
                        {

                            myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                            myCommand.Parameters.AddWithValue("@sid", thisSwimmerID[scounter]);
                            myCommand.Parameters.AddWithValue("@entryTime", entryTime[scounter]);
                            myCommand.Parameters.AddWithValue("@classNo", classNo[scounter]);
                            myCommand.Parameters.AddWithValue("@certNo", certNo[scounter]);
                            myCommand.Parameters.AddWithValue("@firstOne", firstOne[scounter]);
                            myCommand.Parameters.AddWithValue("@secondOne", secondOne[scounter]);
                            myCommand.Parameters.AddWithValue("@thirdOne", thirdOne[scounter]);
                            myCommand.Parameters.AddWithValue("@lastOne", lastOne[scounter]);
                            myCommand.Parameters.AddWithValue("@firstOneClassNo", firstOneClassNo[scounter]);
                            myCommand.Parameters.AddWithValue("@firstOneCertNo", firstOneCertNo[scounter]);
                            myCommand.Parameters.AddWithValue("@uid", uid);
                            myCommand.Parameters.AddWithValue("@tokumi", kumi);
                            myCommand.Parameters.AddWithValue("@laneNo", lane);
                            myCommand.Parameters.AddWithValue("@goal", goal[scounter]);
                            scounter++;
                        }
                        myCommand.ExecuteNonQuery();
                    }
                }   
            }
        }

        static void MoveSwimmerAll(bool scenario4Flag)
        {
            int prgNo = 1, kumi = 0; bool first = true;
            int nextPrgNo, nextKumi;
            while ( GetNextRace(ref prgNo,ref kumi))
            {
                nextPrgNo = prgNo;
                nextKumi = kumi;
                if (!GetNextRace(ref nextPrgNo, ref nextKumi)) return;
                while(CanGoTogether(prgNo,kumi,nextPrgNo,nextKumi))
                {
                    if (kumi==1)
                    {
                        if ( scenario4Flag ||( (nextKumi==1) &&  (GenderbyPrgNo[prgNo - 1] == GenderbyPrgNo[nextPrgNo - 1]) )) {
                            
                            MoveSwimmer(nextPrgNo, nextKumi, prgNo, kumi);
                            if (first)
                            {
                                first = false;
                                if (!scenario4Flag)
                                    ChangeClass(prgNo,0);
                                else
                                    ChangeClass(prgNo,GoDoClass   );
                                if (GenderbyPrgNo[prgNo - 1] != GenderbyPrgNo[nextPrgNo - 1])
                                    ChangeGender(prgNo);
                            }
                            if (HowManySwimmers(nextPrgNo) == 0)
                                DeleteProgram(nextPrgNo);
                            
                        }
                    } else
                    {
                        if (scenario4Flag)
                            if (HowManySwimmers(nextPrgNo, 2) == 0)
                            {
                                MoveSwimmerReverse(prgNo, kumi, nextPrgNo, nextKumi);
                                ChangeClass(nextPrgNo, GoDoClass);
                                DeleteProgram(prgNo);
                                if (GenderbyPrgNo[prgNo - 1] != GenderbyPrgNo[nextPrgNo - 1])
                                {
                                    ChangeGender(nextPrgNo);
                                }
                            }
                            else
                            {
                            }
                    }
                    if (!GetNextRace(ref nextPrgNo, ref nextKumi)) return;
                }
                first = true;

            }
        }

         static bool CanGoTogether(int prgNo1, int kumi1, int prgNo2, int kumi2)
        {
            if (prgNo1 == prgNo2) return false;
            int lastLane;
            int firstLane;
            if (NumSwimmers[GetRaceNo(prgNo1, kumi1)] +
                 NumSwimmers[GetRaceNo(prgNo2, kumi2)] > MaxLaneNo4TimeFinal) return false;
            lastLane = GetLastLane(prgNo1, kumi1);
            firstLane = GetFirstLane(prgNo2, kumi2);
            if (lastLane < firstLane) return true;

            return false;
        }

        /*------
                for (lane=1; lane<maxLane;lane++)
                {

                    myQuery = @" Update 記録
    　　　　　　　　　　　　　　set 選手番号=0, エントリータイム='', 
                             新記録判定クラス=0, 標準記録判定クラス=0, 
                             第１泳者=0 , 第２泳者 = 0 ,
                             第３泳者=0 , 第４泳者 = 0 ,
                             第一泳者新記録判定クラス=0 ,
                             第一泳者標準記録判定クラス=0 ,
                             ゴール='' 
                          where 大会番号=@eventNo and 競技番号=@uid and 組=@kumi and 水路=@laneNo";

                    using (SqlCommand myCommand = new(myQuery, myCon))
                    {

                                             myCommand.Parameters.AddWithValue("@laneNo", lane);
                        myCommand.ExecuteNonQuery();
                    }

                }
        */
        static void SetKumiToProgram(int prgNo, int kumi, SqlConnection myCon)
        {
            string setQuery = @"UPDATE プログラム SET 組数=@kumi WHERE 大会番号=@eventNo AND 競技番号=@uid ";
            int uid = UIDFromPrgNo[prgNo - 1];

            using (SqlCommand myCommand = new(setQuery, myCon))
            {
                myCommand.Parameters.Add("@eventNo", SqlDbType.Int).Value = EventNo;
                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@kumi", SqlDbType.Int).Value = kumi;
                myCommand.ExecuteNonQuery();
            }

        }
        static void DeleteRace(int prgNo, int kumi, SqlConnection myCon)
        {
            string delQuery = @"DELETE FROM 記録 WHERE 大会番号=@eventNo AND 競技番号=@uid AND 組=@kumi";
            int uid = UIDFromPrgNo[prgNo - 1];

            using (SqlCommand myCommand = new(delQuery, myCon))
            {
                myCommand.Parameters.Add("@eventNo", SqlDbType.Int).Value = EventNo;
                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@kumi", SqlDbType.Int).Value = kumi;
                myCommand.ExecuteNonQuery();
            }

            kumi++;
            while (RaceExist(prgNo, kumi, myCon))
            {
                DecrementKumiNo(prgNo, kumi, myCon);
                kumi++;
            }
            kumi = kumi - 2;
            SetKumiToProgram(prgNo, kumi, myCon);
        }

        static void DeleteLap(int prgNo, int kumi, SqlConnection myCon)
        {
            string delQuery = @"DELETE FROM ラップ WHERE 大会番号=@eventNo AND 競技番号=@uid AND 組=@kumi";
            int uid = UIDFromPrgNo[prgNo - 1];

            using (SqlCommand myCommand = new(delQuery, myCon))
            {
                myCommand.Parameters.Add("@eventNo", SqlDbType.Int).Value = EventNo;
                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@kumi", SqlDbType.Int).Value = kumi;
                myCommand.ExecuteNonQuery();
            }

            kumi++;
            while (LapExist(prgNo, kumi, myCon))
            {
                DecrementKumiNo4Lap(prgNo, kumi, myCon);
                kumi++;
            }
        }

        static void DecrementKumiNo4Lap(int prgNo, int kumi, SqlConnection myCon)
        {
            string Query = @"UPDATE ラップ SET 組=@newKumi WHERE 大会番号=@eventNo AND
                       競技番号=@uid AND 組=@kumi";

            int uid = UIDFromPrgNo[prgNo - 1];
            using (SqlCommand myCommand = new(Query, myCon))
            {
                myCommand.Parameters.Add("@eventNo", SqlDbType.Int).Value = EventNo;
                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@kumi", SqlDbType.Int).Value = kumi;
                myCommand.Parameters.Add("@newKumi", SqlDbType.Int).Value = kumi - 1;
                myCommand.ExecuteNonQuery();
            }
        }


        static void DecrementKumiNo(int prgNo, int kumi, SqlConnection myCon)
        {
            string Query = @"UPDATE 記録 SET 組=@newKumi WHERE 大会番号=@eventNo AND
                       競技番号=@uid AND 組=@kumi";

            int uid = UIDFromPrgNo[prgNo - 1];
            using (SqlCommand myCommand = new(Query, myCon))
            {
                myCommand.Parameters.Add("@eventNo", SqlDbType.Int).Value = EventNo;
                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@kumi", SqlDbType.Int).Value = kumi;
                myCommand.Parameters.Add("@newKumi", SqlDbType.Int).Value = kumi - 1;
                myCommand.ExecuteNonQuery();
            }
        }

        static bool LapExist(int prgNo, int kumi, SqlConnection myCon)
        {
            string Query = @"SELECT 1 FROM ラップ WHERE 大会番号=@eventNo AND 競技番号=@uid AND 組=@kumi";

            int uid = UIDFromPrgNo[prgNo - 1];
            using (SqlCommand myCommand = new(Query, myCon))
            {
                myCommand.Parameters.Add("@eventNo", SqlDbType.Int).Value = EventNo;
                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@kumi", SqlDbType.Int).Value = kumi;

                using (SqlDataReader reader = myCommand.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        static bool RaceExist(int prgNo, int kumi, SqlConnection myCon)
        {
            string Query = @"SELECT 1 FROM 記録 WHERE 大会番号=@eventNo AND 競技番号=@uid AND 組=@kumi";

            int uid = UIDFromPrgNo[prgNo - 1];
            using (SqlCommand myCommand = new(Query, myCon))
            {
                myCommand.Parameters.Add("@eventNo", SqlDbType.Int).Value = EventNo;
                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@kumi", SqlDbType.Int).Value = kumi;

                using (SqlDataReader reader = myCommand.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }


        /// <summary>
        /// Move swimmers registered in the program # = prgNo and kumi# = kumi to toPrgNo, toKumi 
        /// </summary>
        /// <param name="prgNo"></param> from program#
        /// <param name="kumi"></param> from kumi#
        /// <param name="toPrgNo"></param> to program#
        /// <param name="toKumi"></param> to kumi#
        static void MoveSwimmer(int prgNo, int kumi, int toPrgNo, int toKumi)
        {
            int uid = UIDFromPrgNo[prgNo-1];
            int[] thisSwimmerID = new int[11];
            string[] entryTime = new string[11];
            Array.Fill(entryTime, string.Empty);
            int[] classNo = new int[11];
            int[] certNo = new int[11];
            int[] firstOne = new int[11];
            int[] secondOne = new int[11];
            int[] thirdOne = new int[11];
            int[] lastOne = new int[11];
            int[] yosenTime = new int[11];  /* feb 2026 addition */
            int[] firstOneClassNo = new int[11];
            int[] firstOneCertNo = new int[11];
            string[] goal = new string[11];
            Array.Fill(goal, string.Empty);
            int lane;
            int startLane=0;
            int maxLane = 0;
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @"
                      select * from 記録 where 競技番号=@uid and 大会番号=@eventNo and 組=@kumi 
                        and 選手番号>0 
                        order by 水路 ";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@uid", uid);
                    myCommand.Parameters.AddWithValue("@kumi", kumi);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lane = Convert.ToInt32(reader["水路"]);
                            thisSwimmerID[lane] = Convert.ToInt32(reader["選手番号"]);
                            entryTime[lane] = (string)reader["エントリータイム"];
                            classNo[lane] = Convert.ToInt32(reader["新記録判定クラス"]);
                            certNo[lane] = Convert.ToInt32(reader["標準記録判定クラス"]);
                            firstOne[lane] = Convert.ToInt32(reader["第１泳者"]);
                            secondOne[lane] = Convert.ToInt32(reader["第２泳者"]);
                            thirdOne[lane] = Convert.ToInt32(reader["第３泳者"]);
                            lastOne[lane] = Convert.ToInt32(reader["第４泳者"]);
                            firstOneClassNo[lane] = Convert.ToInt32(reader["第一泳者新記録判定クラス"]);
                            firstOneCertNo[lane] = Convert.ToInt32(reader["第一泳者標準記録判定クラス"]);
                            goal[lane] = (string) (reader["ゴール"]);
                            //yosenTime[lane] = Convert.ToInt32(reader["予選タイム"]); /* feb 2026 addition */
                            if (startLane == 0)
                                startLane = lane;

                            maxLane = lane;
                        }
                    }
                }
                uid=UIDFromPrgNo[toPrgNo-1];
                for (lane = startLane; lane<=maxLane; lane++)   
                {

                    myQuery = @" Update 記録
　　　　　　　　　　　　　　set 選手番号=@sid, エントリータイム=@entryTime, 
                         新記録判定クラス=@classNo, 標準記録判定クラス=@certNo, 
                         第１泳者=@firstOne , 第２泳者 = @secondOne ,
                         第３泳者=@thirdOne , 第４泳者 = @lastOne ,
                         第一泳者新記録判定クラス=@firstOneClassNo ,
                         第一泳者標準記録判定クラス=@firstOneCertNo ,
                         --予選タイム=@yosenTime ,  /* feb 2026 addition */
                         予選タイム='',             
                         ゴール=@goal 
                      where 大会番号=@eventNo and 競技番号=@uid and 組=@tokumi and 水路=@laneNo";

                    using (SqlCommand myCommand = new(myQuery, myCon))
                    {

                        myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                        myCommand.Parameters.AddWithValue("@sid", thisSwimmerID[lane]);
                        myCommand.Parameters.AddWithValue("@entryTime", entryTime[lane]);
                        myCommand.Parameters.AddWithValue("@classNo", classNo[lane]);
                        myCommand.Parameters.AddWithValue("@certNo", certNo[lane]);
                        myCommand.Parameters.AddWithValue("@firstOne", firstOne[lane]);
                        myCommand.Parameters.AddWithValue("@secondOne", secondOne[lane]);
                        myCommand.Parameters.AddWithValue("@thirdOne", thirdOne[lane]);
                        myCommand.Parameters.AddWithValue("@lastOne", lastOne[lane]);
                        myCommand.Parameters.AddWithValue("@firstOneClassNo", firstOneClassNo[lane]);
                        myCommand.Parameters.AddWithValue("@firstOneCertNo", firstOneCertNo[lane]);
                        myCommand.Parameters.AddWithValue("@uid", uid);
                        myCommand.Parameters.AddWithValue("@tokumi", toKumi);
                        myCommand.Parameters.AddWithValue("@laneNo", lane);
                        myCommand.Parameters.AddWithValue("@goal", goal[lane]);
                        //myCommand.Parameters.AddWithValue("@yosenTime", yosenTime[lane]); /* feb 2026 addition */
                        myCommand.ExecuteNonQuery();
                    }
                }
                DeleteRace(prgNo, kumi, myCon);
                DeleteLap(prgNo, kumi, myCon);
            }   
        }

        static void MoveSwimmerReverse(int prgNo, int kumi, int toPrgNo, int toKumi)
        {
            int uid = UIDFromPrgNo[prgNo-1];
            int[] thisSwimmerID = new int[10];
            string[] entryTime = new string[10];
            Array.Fill(entryTime, string.Empty);
            int[] classNo = new int[10];
            int[] certNo = new int[10];
            int[] firstOne = new int[10];
            int[] secondOne = new int[10];
            int[] thirdOne = new int[10];
            int[] lastOne = new int[10];
            int[] firstOneClassNo = new int[10];
            int[] firstOneCertNo = new int[10];
            int[] laneNo = new int[10];
            string[] goal = new string[10];
            Array.Fill(goal, string.Empty);
            int scounter = 0;
            int startLane=0;
            int maxScounter = 0;
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = @"
                      select * from 記録 where 競技番号=@uid and 大会番号=@eventNo and 組=@kumi 
                        and 選手番号>0 
                        order by 水路 ";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.Parameters.AddWithValue("@uid", uid);
                    myCommand.Parameters.AddWithValue("@kumi", kumi);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            thisSwimmerID[scounter] = Convert.ToInt32(reader["選手番号"]);
                            entryTime[scounter] = (string)reader["エントリータイム"];
                            classNo[scounter] = Convert.ToInt32(reader["新記録判定クラス"]);
                            certNo[scounter] = Convert.ToInt32(reader["標準記録判定クラス"]);
                            firstOne[scounter] = Convert.ToInt32(reader["第１泳者"]);
                            secondOne[scounter] = Convert.ToInt32(reader["第２泳者"]);
                            thirdOne[scounter] = Convert.ToInt32(reader["第３泳者"]);
                            lastOne[scounter] = Convert.ToInt32(reader["第４泳者"]);
                            firstOneClassNo[scounter] = Convert.ToInt32(reader["第一泳者新記録判定クラス"]);
                            firstOneCertNo[scounter] = Convert.ToInt32(reader["第一泳者標準記録判定クラス"]);
                            laneNo[scounter] = Convert.ToInt32(reader["水路"]);
                            goal[scounter] = (string) (reader["ゴール"]);
                            scounter++;
                        }
                        maxScounter = scounter;
                    }
                }

                startLane++;
                scounter = 0;
                uid=UIDFromPrgNo[toPrgNo-1];
                for (scounter = 0; scounter<maxScounter; scounter++)
                {

                    myQuery = @" Update 記録
　　　　　　　　　　　　　　set 選手番号=@sid, エントリータイム=@entryTime, 
                         新記録判定クラス=@classNo, 標準記録判定クラス=@certNo, 
                         第１泳者=@firstOne , 第２泳者 = @secondOne ,
                         第３泳者=@thirdOne , 第４泳者 = @lastOne ,
                         第一泳者新記録判定クラス=@firstOneClassNo ,
                         第一泳者標準記録判定クラス=@firstOneCertNo ,
                         ゴール=@goal
                      where 大会番号=@eventNo and 競技番号=@uid and 組=@tokumi and 水路=@laneNo";

                    using (SqlCommand myCommand = new(myQuery, myCon))
                    {

                        myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                        myCommand.Parameters.AddWithValue("@sid", thisSwimmerID[scounter]);
                        myCommand.Parameters.AddWithValue("@entryTime", entryTime[scounter]);
                        myCommand.Parameters.AddWithValue("@classNo", classNo[scounter]);
                        myCommand.Parameters.AddWithValue("@certNo", certNo[scounter]);
                        myCommand.Parameters.AddWithValue("@firstOne", firstOne[scounter]);
                        myCommand.Parameters.AddWithValue("@secondOne", secondOne[scounter]);
                        myCommand.Parameters.AddWithValue("@thirdOne", thirdOne[scounter]);
                        myCommand.Parameters.AddWithValue("@lastOne", lastOne[scounter]);
                        myCommand.Parameters.AddWithValue("@firstOneClassNo", firstOneClassNo[scounter]);
                        myCommand.Parameters.AddWithValue("@firstOneCertNo", firstOneCertNo[scounter]);
                        myCommand.Parameters.AddWithValue("@uid", uid);
                        myCommand.Parameters.AddWithValue("@tokumi", toKumi);
                        myCommand.Parameters.AddWithValue("@laneNo", laneNo[scounter]);
                        myCommand.Parameters.AddWithValue("@goal", goal[scounter]);
                        myCommand.ExecuteNonQuery();
                    }
                }   
            }
        }

        static int GetStartLane(int totalNum)
        {
            if (totalNum >= MaxLaneNo4TimeFinal-1)
            {
                return 1 ;
            }
            if (totalNum >= MaxLaneNo4TimeFinal-3)
            {
                return 2 ;
            }
            if (totalNum >= MaxLaneNo4TimeFinal-5)
            {
                return 3 ;
            }
            if (totalNum >= MaxLaneNo4TimeFinal-7)
            {
                return 4 ;
            }
            return 5 ;

        }
        static void ReadProgramDB()
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                InitProgramDBArray(myCon);
                string myQuery = @"
                    select 表示用競技番号, 競技番号 as uid, 種目コード, 距離コード, 
                    性別コード, 予決コード, クラス番号 
                    FROM プログラム
                    where 大会番号= @eventNo ";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int myPrgNo = Convert.ToInt32(reader["表示用競技番号"]);
                            int myUID = Convert.ToInt32(reader["uid"]);
                            PrgNobyUID[myUID-1]= myPrgNo;
                            ShumokubyPrgNo[myPrgNo-1] = Convert.ToInt32(reader["種目コード"]);
                            DistancebyPrgNo[myPrgNo-1] = Convert.ToInt32(reader["距離コード"]);
                            UIDFromPrgNo[myPrgNo-1] = myUID;
                            ClassbyPrgNo[myPrgNo-1] = Convert.ToInt32(reader["クラス番号"]);
                            GenderbyPrgNo[myPrgNo-1] = Convert.ToInt32(reader["性別コード"]);
                            Phase[myPrgNo-1] = Convert.ToInt32(reader["予決コード"]);


                        }
                    }
                }
            }

        }

        static void DeleteGoDoTable()
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery = "delete from 合同レースプログラム where 大会番号=@EventNo;";
                using (SqlCommand myCommand = new(myQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                    myCommand.ExecuteNonQuery();
                }
            }
        }

        static void CreateGoDoTable()
        {
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery;
                int tifNumber = 0;
                int howMany = 1;
                int prgNo=1;
                int kumi = 1;
                int nextPrgNo;
                int nextKumi;
                bool first = true;
                do {
                    nextPrgNo = prgNo;
                    nextKumi = kumi;
                    if (!GetNextRace(ref nextPrgNo, ref nextKumi)) return;
                    int raceNo = GetRaceNo(prgNo,kumi);
                    int UID = UIDFromPrgNo[prgNo-1];
                    if (CanGoTogether(prgNo, kumi, nextPrgNo, nextKumi))
                    {
                        if (first)
                        {
                            first = false;
                            tifNumber++;
                            howMany = 1;
                            myQuery = @"insert into 合同レースプログラム 
                                 (大会番号, 合同レース番号, 競技番号1, 組1 )
                                  values ( @eventNo , @tifNumber , @uid, @kumi)";
                            using (SqlCommand myCommand = new(myQuery, myCon))
                            {
                                myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                                myCommand.Parameters.AddWithValue("@tifNumber", tifNumber);
                                myCommand.Parameters.AddWithValue("@uid", UID);
                                myCommand.Parameters.AddWithValue("@kumi", kumi);
                                myCommand.ExecuteNonQuery();
                            }
                            howMany++;
                            myQuery = @"update 合同レースプログラム 
                                    set 競技番号" + howMany + "= @uid, 組" + howMany + @"=@kumi 
                                    where 合同レース番号=@tifNumber and 大会番号=@eventNo";
                            UID = UIDFromPrgNo[nextPrgNo-1];

                            using (SqlCommand myCommand = new(myQuery, myCon))
                            {
                                myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                                myCommand.Parameters.AddWithValue("@tifNumber", tifNumber);
                                myCommand.Parameters.AddWithValue("@uid", UID);
                                myCommand.Parameters.AddWithValue("@kumi", nextKumi);
                                myCommand.ExecuteNonQuery();
                            }
}
                        else
                        {
                            howMany++;
                            myQuery = @"update 合同レースプログラム 
                                    set 競技番号" + howMany + "= @uid, 組" + howMany + @"=@kumi 
                                    where 合同レース番号=@tifNumber and 大会番号=@eventNo";
                            UID = UIDFromPrgNo[nextPrgNo-1];

                            using (SqlCommand myCommand = new(myQuery, myCon))
                            {
                                myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                                myCommand.Parameters.AddWithValue("@tifNumber", tifNumber);
                                myCommand.Parameters.AddWithValue("@uid", UID);
                                myCommand.Parameters.AddWithValue("@kumi", nextKumi);
                                myCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    else first = true;
                } while (GetNextRace(ref prgNo, ref kumi));
            }
 
        }
        static void ResizePrgArray() {
            int raceSize = MaxPrgNo * MaxKumi;
            Array.Resize(ref PrgNobyUID, MaxUID);
            Array.Resize(ref ShumokubyPrgNo, MaxPrgNo);
            Array.Resize(ref NumSwimmers, raceSize);
            Array.Resize(ref NumSwimmersbyTID, raceSize);
            Array.Resize(ref TIDarray, raceSize);
            Array.Resize(ref DistancebyPrgNo, MaxPrgNo);
            Array.Resize(ref UIDFromPrgNo, MaxPrgNo);
            Array.Resize(ref ClassbyPrgNo, MaxPrgNo);
            Array.Resize(ref GenderbyPrgNo, MaxPrgNo);
            Array.Resize(ref Phase, MaxPrgNo);
        }
        static void CreateGoDoTableObso()
        {
            DeleteGoDoTable();
            using (SqlConnection myCon = new(GoDogen.ConnectionString))
            {
                myCon.Open();
                string myQuery;
                int tifNumber = 0;
                int prevTif = 0;
                int howMany = 1;
                int prgNo=1;
                int kumi = 1;
                do {
                    int raceNo = GetRaceNo(prgNo,kumi);
                    int UID = UIDFromPrgNo[prgNo-1];
                    if ( TIDarray[raceNo]>0)
                    {
                        if ( TIDarray[raceNo]>prevTif) {
                            tifNumber++;
                            prevTif = TIDarray[raceNo];
                            howMany = 1;
                            myQuery = @"insert into 合同レースプログラム 
                                 (大会番号, 合同レース番号, 競技番号" + howMany + ", 組" +
                                 howMany + ") values ( @eventNo , @tifNumber , @uid, @kumi)";
                        }
                        else 
                        {
                            howMany++;
                            myQuery = @"update 合同レースプログラム 
                                    set 競技番号" + howMany +"= @uid, 組" + howMany+ @"=@kumi 
                                    where 合同レース番号=@tifNumber and 大会番号=@eventNo";

                        }

                        using (SqlCommand myCommand = new(myQuery, myCon))
                        {
                            myCommand.Parameters.AddWithValue("@eventNo", EventNo);
                            myCommand.Parameters.AddWithValue("@tifNumber", tifNumber);
                            myCommand.Parameters.AddWithValue("@uid", UID);
                            myCommand.Parameters.AddWithValue("@kumi", kumi);
                            myCommand.ExecuteNonQuery();
                        }
                    }
                } while (GetNextRace(ref prgNo, ref kumi));
            }
 

        }

    }
}
