#!/usr/bin/env python3

import sys
import pyodbc

# ===== SQL SERVER =====
CONNECTION_STR = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=localhost;"
    "DATABASE=sw;"
    "Trusted_Connection=yes;"
    "TrustServerCertificate=yes;"
)




def get_prg_no(eventNo, displayPrgNo):
    """表示用競技番号からDB上の競技番号を取得"""

    sql = """
        SELECT 競技番号
        FROM プログラム
        WHERE 大会番号 = ?
          AND 表示用競技番号 = ?
    """

    with pyodbc.connect(CONNECTION_STR) as conn:
        cur = conn.cursor()
        cur.execute(sql, eventNo, displayPrgNo)
        row = cur.fetchone()

    if row is None:
        raise ValueError(
            f"表示用競技番号 {displayPrgNo} が見つかりません "
            f"(大会番号={eventNo})"
        )

    return row[0]


def exchange_lane(eventNo, prg1, kumi1, lane1, prg2, kumi2, lane2):

    sql = """
        SELECT *
        FROM 記録
        WHERE 大会番号 = ?
          AND 競技番号 = ?
          AND 組 = ?
          AND 水路 = ?
    """

    with pyodbc.connect(CONNECTION_STR) as conn:
        cur = conn.cursor()

        cur.execute("SELECT TOP 0 * FROM 記録")
        columns = [c[0] for c in cur.description]

        cur.execute(sql, eventNo, prg1, kumi1, lane1)
        data1 = cur.fetchone()

        cur.execute(sql, eventNo, prg2, kumi2, lane2)
        data2 = cur.fetchone()

        if data1 is None:
            raise ValueError(
                f"交換元データがありません: "
                f"{eventNo}, {prg1}, {kumi1}, {lane1}"
            )

        pk = ["大会番号", "競技番号", "組", "水路"]
        update_columns = [c for c in columns if c not in pk]

        values1 = dict(zip(columns, data1))
        values2 = dict(zip(columns, data2)) if data2 else None

        if data2 is None:
            # 移動先にデータがない場合は INSERT
            insert_values = [
                values1[c] if c not in pk else {
                    "大会番号": eventNo,
                    "競技番号": prg2,
                    "組": kumi2,
                    "水路": lane2
                }[c]
                for c in columns
            ]

            col_names = ", ".join(f"[{c}]" for c in columns)
            placeholders = ", ".join("?" for _ in columns)

            cur.execute(
                f"INSERT INTO 記録 ({col_names}) VALUES ({placeholders})",
                *insert_values
            )

            cur.execute(
                """
                DELETE FROM 記録
                WHERE 大会番号 = ?
                  AND 競技番号 = ?
                  AND 組 = ?
                  AND 水路 = ?
                """,
                eventNo, prg1, kumi1, lane1
            )

        else:
            # 両方にデータがある場合は交換
            set_clause = ", ".join(
                f"[{c}] = ?" for c in update_columns
            )

            update_sql = f"""
                UPDATE 記録
                SET {set_clause}
                WHERE 大会番号 = ?
                  AND 競技番号 = ?
                  AND 組 = ?
                  AND 水路 = ?
            """

            cur.execute(
                update_sql,
                *[values2[c] for c in update_columns],
                eventNo, prg1, kumi1, lane1
            )

            cur.execute(
                update_sql,
                *[values1[c] for c in update_columns],
                eventNo, prg2, kumi2, lane2
            )

        conn.commit()


if __name__ == "__main__":

    if len(sys.argv) != 8:
        print(
            "Usage: exchangeLane.py "
            "EventNo DisplayPrgNo1 Kumi1 Lane1 "
            "DisplayPrgNo2 Kumi2 Lane2"
        )
        sys.exit(1)

    try:
        eventNo, displayPrg1, kumi1, lane1, displayPrg2, kumi2, lane2 = \
            map(int, sys.argv[1:])

        # 表示用競技番号から実際の競技番号を取得
        prg1 = get_prg_no(eventNo, displayPrg1)
        prg2 = get_prg_no(eventNo, displayPrg2)

        exchange_lane(
            eventNo, prg1, kumi1, lane1,
            prg2, kumi2, lane2
        )

        print("レーン移動・交換完了")

    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)
