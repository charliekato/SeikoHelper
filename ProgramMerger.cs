using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace SeikoHelper
{
    public class ProgramMerger
    {


        // プログラムレコードの構造体
        private class ProgramRecord
        {
            public int 大会番号;
            public int 競技番号;
            public int 表示用競技番号;
            public int 組数;
            public int クラス番号;
            public int 種目コード;
            public int 距離コード;
            public int 性別コード;
            public int 予決コード;
            public string 日付;
            public string 時間;
            public int ポイント設定番号;
            public int 予選競技番号;
            public bool ポイント計算;
            public int 競技面;
            public int 午前午後;
            public int 進行フラグ;
            public string 備考;
        }
        private class RecordRecord
        {
            public int 大会番号;
            public int 競技番号;
            public int 組;
            public int 水路;
            public int 事由入力ステータス;
            public string ゴール;
            public string 中間記録;
            public int 選手番号;
            public int 第１泳者;
            public int 第２泳者;
            public int 第３泳者;
            public int 第４泳者;
            public string 新記録印刷マーク;
            public string 新記録電光マーク;
            public string 棄権印刷マーク;
            public string 棄権電光マーク;
            public string 中間新記録マーク;
            public string エントリータイム;
            public string 予選タイム;
            public int 失格泳者;
            public string 引継タイム１;
            public string 引継タイム２;
            public string 引継タイム３;
            public int 新記録判定クラス;
            public int 標準記録判定クラス;
            public string 予備;
            public string リアクション;
            public string 引継ぎ１;
            public string 引継ぎ２;
            public string 引継ぎ３;
            public int ラップカウント;
            public string 資格級;
            public int 事由表示;
            public string 事由予備;
            public string 標準１;
            public string 標準２;
            public string 標準３;
            public string 標準４;
            public string 標準５;
            public string 中間標準１;
            public string 中間標準２;
            public string 中間標準３;
            public string 中間標準４;
            public string 中間標準５;
            public int オープン;
            public string FINAポイント;
            public string 中間新記録電光マーク;
            public int 第一泳者新記録判定クラス;
            public int 第一泳者標準記録判定クラス;
            public string 泳力級;
        }

        private class LapRecord
        {
            public short 大会番号;
            public short ラップ区分;
            public short 競技番号;
            public short 組;
            public short 水路;
            public short 記録区分番号;
            public short 記録名称番号;

            public string m25;
            public string m50;
            public string m75;
            public string m100;
            public string m125;
            public string m150;
            public string m175;
            public string m200;
            public string m225;
            public string m250;
            public string m275;
            public string m300;
            public string m325;
            public string m350;
            public string m375;
            public string m400;
            public string m425;
            public string m450;
            public string m475;
            public string m500;
            public string m525;
            public string m550;
            public string m575;
            public string m600;
            public string m625;
            public string m650;
            public string m675;
            public string m700;
            public string m725;
            public string m750;
            public string m775;
            public string m800;
            public string m825;
            public string m850;
            public string m875;
            public string m900;
            public string m925;
            public string m950;
            public string m975;
            public string m1000;
            public string m1025;
            public string m1050;
            public string m1075;
            public string m1100;
            public string m1125;
            public string m1150;
            public string m1175;
            public string m1200;
            public string m1225;
            public string m1250;
            public string m1275;
            public string m1300;
            public string m1325;
            public string m1350;
            public string m1375;
            public string m1400;
            public string m1425;
            public string m1450;
            public string m1475;
            public string m1500;
        }

        public class ProgramNoMap
        {
            static Dictionary<int, int> prgNoMap = new();
            public static void AddDict(int oldPrgNo, int newPrgNo)
            {
                prgNoMap[oldPrgNo] = newPrgNo;

            }
            public static int GetNewPrgNo(int oldPrgNo)
            {
                return prgNoMap[oldPrgNo];
            }

        }
        // 競技番号の再割り当てマップ: 旧競技番号 -> 新競技番号


        public static void MergePrograms(int eventNo)
        {
            string connectionString = GlobalV.MagicHead + GlobalV.ServerName + GlobalV.MagicWord;


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                CreateNewProgramTable(conn, eventNo);

                CreateNewRecordTable(conn, eventNo);
                CreateNewLapTable(conn, eventNo);

                string deleteQuery = @"
                     DELETE from プログラム where 大会番号=@大会番号";

                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@大会番号", eventNo);
                    deleteCmd.ExecuteNonQuery();
                }
                string moveQuery = @"
                     UPDATE プログラム set 大会番号=@大会番号 where 大会番号=@temp大会番号";

                using (SqlCommand moveCmd = new SqlCommand(moveQuery, conn))
                {
                    moveCmd.Parameters.AddWithValue("@大会番号", eventNo);
                    moveCmd.Parameters.AddWithValue("@temp大会番号", 1000);
                    moveCmd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateNewProgramTable(SqlConnection conn, int eventNo)
        {
            // 統合キー: 種目コード, 距離コード, 性別コード, 予決コード
            Dictionary<string, List<ProgramRecord>> mergeGroups = new();


            // プログラムテーブルから指定された大会番号のレコードを取得
            string selectProgramsQuery = @"
            SELECT * FROM プログラム
            WHERE 大会番号 = @eventNo
            ORDER BY 競技番号";

            using (SqlCommand cmd = new SqlCommand(selectProgramsQuery, conn))
            {
                cmd.Parameters.AddWithValue("@eventNo", eventNo);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProgramRecord record = new ProgramRecord
                        {
                            大会番号 = (short)reader["大会番号"],
                            競技番号 = (short)reader["競技番号"],
                            表示用競技番号 = (short)reader["表示用競技番号"],
                            組数 = (short)reader["組数"],
                            クラス番号 = (short)reader["クラス番号"],
                            種目コード = (short)reader["種目コード"],
                            距離コード = (short)reader["距離コード"],
                            性別コード = (short)reader["性別コード"],
                            予決コード = (short)reader["予決コード"],
                            日付 = (string)reader["日付"],
                            時間 = (string)reader["時間"],
                            ポイント設定番号 = (short)reader["ポイント設定番号"],
                            ポイント計算 = (bool)reader["ポイント計算"],
                            競技面 = (short)reader["競技面"],
                            午前午後 = (short)reader["午前午後"],
                            進行フラグ = (short)reader["進行フラグ"],
                            備考 = (string)reader["備考"]
                        };

                        string key = $"{record.種目コード}_{record.距離コード}_{record.性別コード}_{record.予決コード}";

                        if (!mergeGroups.ContainsKey(key))
                        {
                            mergeGroups[key] = new List<ProgramRecord>();
                        }
                        mergeGroups[key].Add(record);
                    }
                }
            }
            // 新しい競技番号のカウンター
            int newPrgNo = 1;

            // プログラムテーブルの更新
            foreach (KeyValuePair<string, List<ProgramRecord>> group in mergeGroups)
            {
                List<ProgramRecord> records = group.Value;
                int totalGroups = 0;

                foreach (ProgramRecord rec in records)
                {
                    totalGroups += rec.組数;
                }

                ProgramRecord firstRecord = records[0];

                string insertProgramQuery = @"
            INSERT INTO プログラム (大会番号, 競技番号, 表示用競技番号, 組数, クラス番号, 
                                    種目コード, 距離コード, 性別コード, 予決コード, 日付, 
                                    時間, ポイント設定番号, 予選競技番号, ポイント計算, 競技面,
                                    午前午後, 進行フラグ, 備考)
            VALUES (@大会番号, @競技番号, @表示用競技番号, @組数, @クラス番号, 
                    @種目コード, @距離コード, @性別コード, @予決コード,@日付,
                    @時間, @ポイント設定番号, @予選競技番号,@ポイント計算,
                    @競技面,@午前午後,@進行フラグ,@備考)";

                using (SqlCommand insertCmd = new SqlCommand(insertProgramQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@大会番号", 1000);
                    insertCmd.Parameters.AddWithValue("@競技番号", newPrgNo);
                    insertCmd.Parameters.AddWithValue("@表示用競技番号", newPrgNo);
                    insertCmd.Parameters.AddWithValue("@組数", totalGroups);
                    insertCmd.Parameters.AddWithValue("@クラス番号", 0);
                    insertCmd.Parameters.AddWithValue("@種目コード", firstRecord.種目コード);
                    insertCmd.Parameters.AddWithValue("@距離コード", firstRecord.距離コード);
                    insertCmd.Parameters.AddWithValue("@性別コード", firstRecord.性別コード);
                    insertCmd.Parameters.AddWithValue("@予決コード", firstRecord.予決コード);
                    insertCmd.Parameters.AddWithValue("@日付", firstRecord.日付);
                    insertCmd.Parameters.AddWithValue("@時間", firstRecord.時間);
                    insertCmd.Parameters.AddWithValue("@ポイント設定番号", firstRecord.ポイント設定番号);
                    insertCmd.Parameters.AddWithValue("@予選競技番号", firstRecord.予選競技番号);
                    insertCmd.Parameters.AddWithValue("@ポイント計算", firstRecord.ポイント計算);
                    insertCmd.Parameters.AddWithValue("@競技面", firstRecord.競技面);
                    insertCmd.Parameters.AddWithValue("@午前午後", firstRecord.午前午後);
                    insertCmd.Parameters.AddWithValue("@進行フラグ", firstRecord.進行フラグ);
                    insertCmd.Parameters.AddWithValue("@備考", firstRecord.備考);
                    insertCmd.ExecuteNonQuery();
                }

                // 旧競技番号を新競技番号にマッピング
                foreach (ProgramRecord rec in records)
                {
                    ProgramNoMap.AddDict(rec.競技番号, newPrgNo);

                }
                newPrgNo++;
            }
        }
        private static void CreateNewRecordTable(SqlConnection conn, int eventNo)
        {
            int newPrgNo;
            string myQuery = @"
            SELECT * FROM 記録
            WHERE 大会番号 = @eventNo
            ORDER BY 競技番号, 組";
            List<RecordRecord> listRecord = new();

            using (SqlCommand cmd = new SqlCommand(myQuery, conn))
            {
                cmd.Parameters.AddWithValue("@eventNo", eventNo);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecordRecord record = new RecordRecord
                        {
                            大会番号 = (short)reader["大会番号"],
                            競技番号 = (short)reader["競技番号"],
                            組 = (short)reader["組"],
                            水路 = (short)reader["水路"],
                            事由入力ステータス = (short)reader["事由入力ステータス"],
                            ゴール = (string)reader["ゴール"],
                            中間記録 = (string)reader["中間記録"],
                            選手番号 = (short)reader["選手番号"],
                            第１泳者 = (short)reader["第１泳者"],
                            第２泳者 = (short)reader["第２泳者"],
                            第３泳者 = (short)reader["第３泳者"],
                            第４泳者 = (short)reader["第４泳者"],
                            新記録印刷マーク = (string)reader["新記録印刷マーク"],
                            新記録電光マーク = (string)reader["新記録電光マーク"],
                            棄権印刷マーク = (string)reader["棄権印刷マーク"],
                            棄権電光マーク = (string)reader["棄権電光マーク"],
                            中間新記録マーク = (string)reader["中間新記録マーク"],
                            エントリータイム = (string)reader["エントリータイム"],
                            予選タイム = (string)reader["予選タイム"],
                            失格泳者 = (short)reader["失格泳者"],
                            引継タイム１ = (string)reader["引継タイム１"],
                            引継タイム２ = (string)reader["引継タイム２"],
                            引継タイム３ = (string)reader["引継タイム３"],
                            新記録判定クラス = (short)reader["新記録判定クラス"],
                            標準記録判定クラス = (short)reader["標準記録判定クラス"],
                            予備 = (string)reader["予備"],
                            リアクション = (string)reader["リアクション"],
                            引継ぎ１ = (string)reader["引継ぎ１"],
                            引継ぎ２ = (string)reader["引継ぎ２"],
                            引継ぎ３ = (string)reader["引継ぎ３"],
                            ラップカウント = (short)reader["ラップカウント"],
                            資格級 = (string)reader["資格級"],
                            事由表示 = (short)reader["事由表示"],
                            事由予備 = (string)reader["事由予備"],
                            標準１ = (string)reader["標準１"],
                            標準２ = (string)reader["標準２"],
                            標準３ = (string)reader["標準３"],
                            標準４ = (string)reader["標準４"],
                            標準５ = (string)reader["標準５"],
                            中間標準１ = (string)reader["中間標準１"],
                            中間標準２ = (string)reader["中間標準２"],
                            中間標準３ = (string)reader["中間標準３"],
                            中間標準４ = (string)reader["中間標準４"],
                            中間標準５ = (string)reader["中間標準５"],
                            オープン = (short)reader["オープン"],
                            FINAポイント = (string)reader["FINAポイント"],
                            中間新記録電光マーク = (string)reader["中間新記録電光マーク"],
                            第一泳者新記録判定クラス = (short)reader["第一泳者新記録判定クラス"],
                            第一泳者標準記録判定クラス = (short)reader["第一泳者標準記録判定クラス"],
                            泳力級 = (string)reader["泳力級"]
                        };
                        listRecord.Add(record);
                    }
                }
            }
            int kumiNo = 1;
            int prevNewPrgNo = 0;
            string insertQuery = @"
                  INSERT INTO 記録 (大会番号,
                                競技番号,
                                組,
                                水路,
                                事由入力ステータス,
                                ゴール,
                                中間記録,
                                選手番号,
                                第１泳者,
                                第２泳者,
                                第３泳者,
                                第４泳者,
                                新記録印刷マーク,
                                新記録電光マーク,
                                棄権印刷マーク,
                                棄権電光マーク,
                                中間新記録マーク,
                                エントリータイム,
                                予選タイム,
                                失格泳者,
                                引継タイム１,
                                引継タイム２,
                                引継タイム３,
                                新記録判定クラス,
                                標準記録判定クラス,
                                予備,
                                リアクション,
                                引継ぎ１,
                                引継ぎ２,
                                引継ぎ３,
                                ラップカウント,
                                資格級,
                                事由表示,
                                事由予備,
                                標準１,
                                標準２,
                                標準３,
                                標準４,
                                標準５,
                                中間標準１,
                                中間標準２,
                                中間標準３,
                                中間標準４,
                                中間標準５,
                                オープン,
                                FINAポイント,
                                中間新記録電光マーク,
                                第一泳者新記録判定クラス,
                                第一泳者標準記録判定クラス,
                                泳力級 )  VALUES ( @大会番号,
                                @競技番号,
                                @組,
                                @水路,
                                @事由入力ステータス,
                                @ゴール,
                                @中間記録,
                                @選手番号,
                                @第１泳者,
                                @第２泳者,
                                @第３泳者,
                                @第４泳者,
                                @新記録印刷マーク,
                                @新記録電光マーク,
                                @棄権印刷マーク,
                                @棄権電光マーク,
                                @中間新記録マーク,
                                @エントリータイム,
                                @予選タイム,
                                @失格泳者,
                                @引継タイム１,
                                @引継タイム２,
                                @引継タイム３,
                                @新記録判定クラス,
                                @標準記録判定クラス,
                                @予備,
                                @リアクション,
                                @引継ぎ１,
                                @引継ぎ２,
                                @引継ぎ３,
                                @ラップカウント,
                                @資格級,
                                @事由表示,
                                @事由予備,
                                @標準１,
                                @標準２,
                                @標準３,
                                @標準４,
                                @標準５,
                                @中間標準１,
                                @中間標準２,
                                @中間標準３,
                                @中間標準４,
                                @中間標準５,
                                @オープン,
                                @FINAポイント,
                                @中間新記録電光マーク,
                                @第一泳者新記録判定クラス,
                                @第一泳者標準記録判定クラス,
                                @泳力級 ) ";
            foreach (RecordRecord laneOrder in listRecord)
            {
                int laneNo = laneOrder.水路;
                newPrgNo = ProgramNoMap.GetNewPrgNo(laneOrder.競技番号);

                if (laneNo == 1)
                {
                    if (newPrgNo != prevNewPrgNo)
                    {
                        prevNewPrgNo = newPrgNo;
                        kumiNo = 1;
                    }
                    else
                        kumiNo++;

                }
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@大会番号", 1000);
                    insertCmd.Parameters.AddWithValue("@競技番号", newPrgNo);
                    insertCmd.Parameters.Add("@組", SqlDbType.Int).Value = kumiNo;
                    insertCmd.Parameters.Add("@水路", SqlDbType.Int).Value = laneOrder.水路;
                    insertCmd.Parameters.Add("@事由入力ステータス", SqlDbType.Int).Value = laneOrder.事由入力ステータス;
                    insertCmd.Parameters.Add("@ゴール", SqlDbType.NVarChar).Value = laneOrder.ゴール;
                    insertCmd.Parameters.Add("@中間記録", SqlDbType.NVarChar).Value = laneOrder.中間記録;
                    insertCmd.Parameters.Add("@選手番号", SqlDbType.Int).Value = laneOrder.選手番号;
                    insertCmd.Parameters.Add("@第１泳者", SqlDbType.Int).Value = laneOrder.第１泳者;
                    insertCmd.Parameters.Add("@第２泳者", SqlDbType.Int).Value = laneOrder.第２泳者;
                    insertCmd.Parameters.Add("@第３泳者", SqlDbType.Int).Value = laneOrder.第３泳者;
                    insertCmd.Parameters.Add("@第４泳者", SqlDbType.Int).Value = laneOrder.第４泳者;
                    insertCmd.Parameters.Add("@新記録印刷マーク", SqlDbType.NVarChar).Value = laneOrder.新記録印刷マーク;
                    insertCmd.Parameters.Add("@新記録電光マーク", SqlDbType.NVarChar).Value = laneOrder.新記録電光マーク;
                    insertCmd.Parameters.Add("@棄権印刷マーク", SqlDbType.NVarChar).Value = laneOrder.棄権印刷マーク;
                    insertCmd.Parameters.Add("@棄権電光マーク", SqlDbType.NVarChar).Value = laneOrder.棄権電光マーク;
                    insertCmd.Parameters.Add("@中間新記録マーク", SqlDbType.NVarChar).Value = laneOrder.中間新記録マーク;
                    insertCmd.Parameters.Add("@エントリータイム", SqlDbType.NVarChar).Value = laneOrder.エントリータイム;
                    insertCmd.Parameters.Add("@予選タイム", SqlDbType.NVarChar).Value = laneOrder.予選タイム;
                    insertCmd.Parameters.Add("@失格泳者", SqlDbType.Int).Value = laneOrder.失格泳者;
                    insertCmd.Parameters.Add("@引継タイム１", SqlDbType.NVarChar).Value = laneOrder.引継タイム１;
                    insertCmd.Parameters.Add("@引継タイム２", SqlDbType.NVarChar).Value = laneOrder.引継タイム２;
                    insertCmd.Parameters.Add("@引継タイム３", SqlDbType.NVarChar).Value = laneOrder.引継タイム３;
                    insertCmd.Parameters.Add("@新記録判定クラス", SqlDbType.Int).Value = laneOrder.新記録判定クラス;
                    insertCmd.Parameters.Add("@標準記録判定クラス", SqlDbType.Int).Value = laneOrder.標準記録判定クラス;
                    insertCmd.Parameters.Add("@予備", SqlDbType.NVarChar).Value = laneOrder.予備;
                    insertCmd.Parameters.Add("@リアクション", SqlDbType.NVarChar).Value = laneOrder.リアクション;
                    insertCmd.Parameters.Add("@引継ぎ１", SqlDbType.NVarChar).Value = laneOrder.引継ぎ１;
                    insertCmd.Parameters.Add("@引継ぎ２", SqlDbType.NVarChar).Value = laneOrder.引継ぎ２;
                    insertCmd.Parameters.Add("@引継ぎ３", SqlDbType.NVarChar).Value = laneOrder.引継ぎ３;
                    insertCmd.Parameters.Add("@ラップカウント", SqlDbType.Int).Value = laneOrder.ラップカウント;
                    insertCmd.Parameters.Add("@資格級", SqlDbType.NVarChar).Value = laneOrder.資格級;
                    insertCmd.Parameters.Add("@事由表示", SqlDbType.Int).Value = laneOrder.事由表示;
                    insertCmd.Parameters.Add("@事由予備", SqlDbType.NVarChar).Value = laneOrder.事由予備;
                    insertCmd.Parameters.Add("@標準１", SqlDbType.NVarChar).Value = laneOrder.標準１;
                    insertCmd.Parameters.Add("@標準２", SqlDbType.NVarChar).Value = laneOrder.標準２;
                    insertCmd.Parameters.Add("@標準３", SqlDbType.NVarChar).Value = laneOrder.標準３;
                    insertCmd.Parameters.Add("@標準４", SqlDbType.NVarChar).Value = laneOrder.標準４;
                    insertCmd.Parameters.Add("@標準５", SqlDbType.NVarChar).Value = laneOrder.標準５;
                    insertCmd.Parameters.Add("@中間標準１", SqlDbType.NVarChar).Value = laneOrder.中間標準１;
                    insertCmd.Parameters.Add("@中間標準２", SqlDbType.NVarChar).Value = laneOrder.中間標準２;
                    insertCmd.Parameters.Add("@中間標準３", SqlDbType.NVarChar).Value = laneOrder.中間標準３;
                    insertCmd.Parameters.Add("@中間標準４", SqlDbType.NVarChar).Value = laneOrder.中間標準４;
                    insertCmd.Parameters.Add("@中間標準５", SqlDbType.NVarChar).Value = laneOrder.中間標準５;
                    insertCmd.Parameters.Add("@オープン", SqlDbType.Int).Value = laneOrder.オープン;
                    insertCmd.Parameters.Add("@FINAポイント", SqlDbType.NVarChar).Value = laneOrder.FINAポイント;
                    insertCmd.Parameters.Add("@中間新記録電光マーク", SqlDbType.NVarChar).Value = laneOrder.中間新記録電光マーク;
                    insertCmd.Parameters.Add("@第一泳者新記録判定クラス", SqlDbType.Int).Value = laneOrder.第一泳者新記録判定クラス;
                    insertCmd.Parameters.Add("@第一泳者標準記録判定クラス", SqlDbType.Int).Value = laneOrder.第一泳者標準記録判定クラス;
                    insertCmd.Parameters.Add("@泳力級", SqlDbType.NVarChar).Value = laneOrder.泳力級;

                    insertCmd.ExecuteNonQuery();
                }
            }

        }
        private static void CreateNewLapTable(SqlConnection conn, int eventNo)
        {

            string lapQuery = @" SELECT * FROM ラップ WHERE 大会番号 = @eventNo ";
            List<LapRecord> listLapRecord = new();
            using (SqlCommand cmd = new SqlCommand(lapQuery, conn))
            {
                cmd.Parameters.AddWithValue("@eventNo", eventNo);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LapRecord record = new LapRecord
                        {
                            大会番号 = (short)reader["大会番号"],
                            ラップ区分 = (short)reader["ラップ区分"],
                            競技番号 = (short)reader["競技番号"],
                            組 = (short)reader["組"],
                            水路 = (short)reader["水路"],
                            記録区分番号 = (short)reader["記録区分番号"],
                            記録名称番号 = (short)reader["記録名称番号"],

                            m25 = reader["25m"] as string,
                            m50 = reader["50m"] as string,
                            m75 = reader["75m"] as string,
                            m100 = reader["100m"] as string,
                            m125 = reader["125m"] as string,
                            m150 = reader["150m"] as string,
                            m175 = reader["175m"] as string,
                            m200 = reader["200m"] as string,
                            m225 = reader["225m"] as string,
                            m250 = reader["250m"] as string,
                            m275 = reader["275m"] as string,
                            m300 = reader["300m"] as string,
                            m325 = reader["325m"] as string,
                            m350 = reader["350m"] as string,
                            m375 = reader["375m"] as string,
                            m400 = reader["400m"] as string,
                            m425 = reader["425m"] as string,
                            m450 = reader["450m"] as string,
                            m475 = reader["475m"] as string,
                            m500 = reader["500m"] as string,
                            m525 = reader["525m"] as string,
                            m550 = reader["550m"] as string,
                            m575 = reader["575m"] as string,
                            m600 = reader["600m"] as string,
                            m625 = reader["625m"] as string,
                            m650 = reader["650m"] as string,
                            m675 = reader["675m"] as string,
                            m700 = reader["700m"] as string,
                            m725 = reader["725m"] as string,
                            m750 = reader["750m"] as string,
                            m775 = reader["775m"] as string,
                            m800 = reader["800m"] as string,
                            m825 = reader["825m"] as string,
                            m850 = reader["850m"] as string,
                            m875 = reader["875m"] as string,
                            m900 = reader["900m"] as string,
                            m925 = reader["925m"] as string,
                            m950 = reader["950m"] as string,
                            m975 = reader["975m"] as string,
                            m1000 = reader["1000m"] as string,
                            m1025 = reader["1025m"] as string,
                            m1050 = reader["1050m"] as string,
                            m1075 = reader["1075m"] as string,
                            m1100 = reader["1100m"] as string,
                            m1125 = reader["1125m"] as string,
                            m1150 = reader["1150m"] as string,
                            m1175 = reader["1175m"] as string,
                            m1200 = reader["1200m"] as string,
                            m1225 = reader["1225m"] as string,
                            m1250 = reader["1250m"] as string,
                            m1275 = reader["1275m"] as string,
                            m1300 = reader["1300m"] as string,
                            m1325 = reader["1325m"] as string,
                            m1350 = reader["1350m"] as string,
                            m1375 = reader["1375m"] as string,
                            m1400 = reader["1400m"] as string,
                            m1425 = reader["1425m"] as string,
                            m1450 = reader["1450m"] as string,
                            m1475 = reader["1475m"] as string,
                            m1500 = reader["1500m"] as string,
                        };

                        listLapRecord.Add(record);
                    }
                }
            }
            int kumiNo = 1;
            int prevNewPrgNo = 0;
            string insertQuery = @"
                  INSERT INTO ラップ (大会番号,
                            ラップ区分,
                            競技番号,
                            組 ,
                            水路,
                            記録区分番号,
                            記録名称番号,
                            [25m], [50m], [75m] , [100m] ,
                          [125m] , [150m] , [175m] , [200m] ,
                          [225m] , [250m] , [275m] , [300m] ,
                          [325m] , [350m] , [375m] , [400m] ,
                          [425m] , [450m] , [475m] , [500m] ,
                          [525m] , [550m] , [575m] , [600m] ,
                          [625m] , [650m] , [675m] , [700m] ,
                          [725m] , [750m] , [775m] , [800m] ,
                          [825m] , [850m] , [875m] , [900m] ,
                          [925m] , [950m] , [975m] , [1000m] ,
                          [1025m] , [1050m] , [1075m] , [1100m] ,
                          [1125m] , [1150m] , [1175m] , [1200m] ,
                          [1225m] , [1250m] , [1275m] , [1300m] ,
                          [1325m] , [1350m] , [1375m] , [1400m] ,
                          [1425m] , [1450m] , [1475m], [1500m] ) VALUES (
                            @大会番号,
                            @ラップ区分,
                            @競技番号,
                            @組 ,
                            @水路,
                            @記録区分番号,
                            @記録名称番号,
                         @m25 , @m50 , @m75 , @m100 ,
                          @m125 , @m150 , @m175 , @m200 ,
                          @m225 , @m250 , @m275 , @m300 ,
                          @m325 , @m350 , @m375 , @m400 ,
                          @m425 , @m450 , @m475 , @m500 ,
                          @m525 , @m550 , @m575 , @m600 ,
                          @m625 , @m650 , @m675 , @m700 ,
                          @m725 , @m750 , @m775 , @m800 ,
                          @m825 , @m850 , @m875 , @m900 ,
                          @m925 , @m950 , @m975 , @m1000 ,
                          @m1025 , @m1050 , @m1075 , @m1100 ,
                          @m1125 , @m1150 , @m1175 , @m1200 ,
                          @m1225 , @m1250 , @m1275 , @m1300 ,
                          @m1325 , @m1350 , @m1375 , @m1400 ,
                          @m1425 , @m1450 , @m1475 , @m1500 )";
            foreach (LapRecord laneOrder in listLapRecord)
            {
                int laneNo = laneOrder.水路;
                int newPrgNo = ProgramNoMap.GetNewPrgNo(laneOrder.競技番号);

                if (laneNo == 1)
                {
                    if (newPrgNo != prevNewPrgNo)
                    {
                        prevNewPrgNo = newPrgNo;
                        kumiNo = 1;
                    }
                    else
                        kumiNo++;

                }
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@大会番号", 1000);
                    insertCmd.Parameters.AddWithValue("@競技番号", newPrgNo);
                    insertCmd.Parameters.Add("@ラップ区分", SqlDbType.NVarChar).Value = laneOrder.ラップ区分;
                    insertCmd.Parameters.Add("@組", SqlDbType.Int).Value = kumiNo;
                    insertCmd.Parameters.Add("@水路", SqlDbType.Int).Value = laneOrder.水路;
                    insertCmd.Parameters.Add("@記録区分番号", SqlDbType.Int).Value = laneOrder.記録区分番号;
                    insertCmd.Parameters.Add("@記録名称番号", SqlDbType.Int).Value = laneOrder.記録名称番号;
                    insertCmd.Parameters.Add("m25", SqlDbType.NVarChar).Value = laneOrder.m25;
                    insertCmd.Parameters.Add("m50", SqlDbType.NVarChar).Value = laneOrder.m50;
                    insertCmd.Parameters.Add("m75", SqlDbType.NVarChar).Value = laneOrder.m75;
                    insertCmd.Parameters.Add("m100", SqlDbType.NVarChar).Value = laneOrder.m100;
                    insertCmd.Parameters.Add("m125", SqlDbType.NVarChar).Value = laneOrder.m125;
                    insertCmd.Parameters.Add("m150", SqlDbType.NVarChar).Value = laneOrder.m150;
                    insertCmd.Parameters.Add("m175", SqlDbType.NVarChar).Value = laneOrder.m175;
                    insertCmd.Parameters.Add("m200", SqlDbType.NVarChar).Value = laneOrder.m200;
                    insertCmd.Parameters.Add("m225", SqlDbType.NVarChar).Value = laneOrder.m225;
                    insertCmd.Parameters.Add("m250", SqlDbType.NVarChar).Value = laneOrder.m250;
                    insertCmd.Parameters.Add("m275", SqlDbType.NVarChar).Value = laneOrder.m275;
                    insertCmd.Parameters.Add("m300", SqlDbType.NVarChar).Value = laneOrder.m300;
                    insertCmd.Parameters.Add("m325", SqlDbType.NVarChar).Value = laneOrder.m325;
                    insertCmd.Parameters.Add("m350", SqlDbType.NVarChar).Value = laneOrder.m350;
                    insertCmd.Parameters.Add("m375", SqlDbType.NVarChar).Value = laneOrder.m375;
                    insertCmd.Parameters.Add("m400", SqlDbType.NVarChar).Value = laneOrder.m400;
                    insertCmd.Parameters.Add("m425", SqlDbType.NVarChar).Value = laneOrder.m425;
                    insertCmd.Parameters.Add("m450", SqlDbType.NVarChar).Value = laneOrder.m450;
                    insertCmd.Parameters.Add("m475", SqlDbType.NVarChar).Value = laneOrder.m475;
                    insertCmd.Parameters.Add("m500", SqlDbType.NVarChar).Value = laneOrder.m500;
                    insertCmd.Parameters.Add("m525", SqlDbType.NVarChar).Value = laneOrder.m525;
                    insertCmd.Parameters.Add("m550", SqlDbType.NVarChar).Value = laneOrder.m550;
                    insertCmd.Parameters.Add("m575", SqlDbType.NVarChar).Value = laneOrder.m575;
                    insertCmd.Parameters.Add("m600", SqlDbType.NVarChar).Value = laneOrder.m600;
                    insertCmd.Parameters.Add("m625", SqlDbType.NVarChar).Value = laneOrder.m625;
                    insertCmd.Parameters.Add("m650", SqlDbType.NVarChar).Value = laneOrder.m650;
                    insertCmd.Parameters.Add("m675", SqlDbType.NVarChar).Value = laneOrder.m675;
                    insertCmd.Parameters.Add("m700", SqlDbType.NVarChar).Value = laneOrder.m700;
                    insertCmd.Parameters.Add("m725", SqlDbType.NVarChar).Value = laneOrder.m725;
                    insertCmd.Parameters.Add("m750", SqlDbType.NVarChar).Value = laneOrder.m750;
                    insertCmd.Parameters.Add("m775", SqlDbType.NVarChar).Value = laneOrder.m775;
                    insertCmd.Parameters.Add("m800", SqlDbType.NVarChar).Value = laneOrder.m800;
                    insertCmd.Parameters.Add("m825", SqlDbType.NVarChar).Value = laneOrder.m825;
                    insertCmd.Parameters.Add("m850", SqlDbType.NVarChar).Value = laneOrder.m850;
                    insertCmd.Parameters.Add("m875", SqlDbType.NVarChar).Value = laneOrder.m875;
                    insertCmd.Parameters.Add("m900", SqlDbType.NVarChar).Value = laneOrder.m900;
                    insertCmd.Parameters.Add("m925", SqlDbType.NVarChar).Value = laneOrder.m925;
                    insertCmd.Parameters.Add("m950", SqlDbType.NVarChar).Value = laneOrder.m950;
                    insertCmd.Parameters.Add("m975", SqlDbType.NVarChar).Value = laneOrder.m975;
                    insertCmd.Parameters.Add("m1000", SqlDbType.NVarChar).Value = laneOrder.m1000;
                    insertCmd.Parameters.Add("m1025", SqlDbType.NVarChar).Value = laneOrder.m1025;
                    insertCmd.Parameters.Add("m1050", SqlDbType.NVarChar).Value = laneOrder.m1050;
                    insertCmd.Parameters.Add("m1075", SqlDbType.NVarChar).Value = laneOrder.m1075;
                    insertCmd.Parameters.Add("m1100", SqlDbType.NVarChar).Value = laneOrder.m1100;
                    insertCmd.Parameters.Add("m1125", SqlDbType.NVarChar).Value = laneOrder.m1125;
                    insertCmd.Parameters.Add("m1150", SqlDbType.NVarChar).Value = laneOrder.m1150;
                    insertCmd.Parameters.Add("m1175", SqlDbType.NVarChar).Value = laneOrder.m1175;
                    insertCmd.Parameters.Add("m1200", SqlDbType.NVarChar).Value = laneOrder.m1200;
                    insertCmd.Parameters.Add("m1225", SqlDbType.NVarChar).Value = laneOrder.m1225;
                    insertCmd.Parameters.Add("m1250", SqlDbType.NVarChar).Value = laneOrder.m1250;
                    insertCmd.Parameters.Add("m1275", SqlDbType.NVarChar).Value = laneOrder.m1275;
                    insertCmd.Parameters.Add("m1300", SqlDbType.NVarChar).Value = laneOrder.m1300;
                    insertCmd.Parameters.Add("m1325", SqlDbType.NVarChar).Value = laneOrder.m1325;
                    insertCmd.Parameters.Add("m1350", SqlDbType.NVarChar).Value = laneOrder.m1350;
                    insertCmd.Parameters.Add("m1375", SqlDbType.NVarChar).Value = laneOrder.m1375;
                    insertCmd.Parameters.Add("m1400", SqlDbType.NVarChar).Value = laneOrder.m1400;
                    insertCmd.Parameters.Add("m1425", SqlDbType.NVarChar).Value = laneOrder.m1425;
                    insertCmd.Parameters.Add("m1450", SqlDbType.NVarChar).Value = laneOrder.m1450;
                    insertCmd.Parameters.Add("m1475", SqlDbType.NVarChar).Value = laneOrder.m1475;
                    insertCmd.Parameters.Add("m1500", SqlDbType.NVarChar).Value = laneOrder.m1500;
                    insertCmd.ExecuteNonQuery();
                }
            }

        }

    }


}

