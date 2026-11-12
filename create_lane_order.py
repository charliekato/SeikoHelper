#!/usr/bin/env python3

import pyodbc
from openpyxl import load_workbook
from openpyxl.styles import Side
from copy import copy
from pathlib import Path


# ==========================================
# 設定
# ==========================================

EVENT_NO = 39       # 大会番号
CONNECTION_STR = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=localhost;"
    "DATABASE=sw;"
    "Trusted_Connection=yes;"
    "TrustServerCertificate=yes;"
)


TEMPLATE_FILE = "template/インクルーシブ大会レーン順template.xlsx"

OUTPUT_FILE = f"output/インクルーシブ大会レーン順_{EVENT_NO}.xlsx"


# ==========================================
# SQL
# ==========================================

SQL = """
select
    表示用競技番号 as prgNo,
    組 as kumi,
    水路 as lane,
    氏名 as name,
    所属名 as belongsto,
    St as st,
    CoE as coe,
    クラス名称 as class,
    性別 as gender,
    距離 as distance,
    種目 as style,
    予決 as yoketsu,
    リレーフラグ as relayFlag,
    第1泳者 as first,
    第2泳者 as second,
    第3泳者 as third,
    第4泳者 as forth
from vレーン順
where 大会番号=?
order by prgNo, kumi, lane
"""

SQL_LANE_INFO = """
select MAXLANE
from vプログラム
where 大会番号=? and 表示用競技番号=?
"""

SQL_ZERO_LANE = """
select distinct ゼロコース使用
from vプログラム
where 大会番号=?
"""

# ==========================================
# SQL実行
# ==========================================

def execute(sql, *params):

    with pyodbc.connect(CONNECTION_STR) as conn:

        cur = conn.cursor()
        cur.execute(sql, *params)

        columns = [c[0] for c in cur.description]

        return [
            dict(zip(columns, row))
            for row in cur.fetchall()
        ]


# ==========================================
# 名前付きセルから列番号を取得
# ==========================================

def get_columns(wb):

    names = [
        "PrgNo",
        "gender",
        "distance",
        "style",
        "yoketsu",
        "kumi",
        "lane",
        "氏名",
        "所属",
        "クラス",
        "St",
        "CoE",
        "氏名2",
    ]

    result = {}

    for name in names:

        defined_name = wb.defined_names[name]

        sheet_name, coordinate = next(
            defined_name.destinations
        )

        from openpyxl.utils.cell import coordinate_from_string
        from openpyxl.utils import column_index_from_string

        col_letter, row = coordinate_from_string(coordinate)

        result[name] = column_index_from_string(col_letter)

    return result


# ==========================================
# Excelへの書き込み
# ==========================================

class LaneWriter:

    PAGE_ROWS = 61
    ROWS_PER_SIDE = 56
    START_ROW = 5

    TEMPLATE_PAGES = 8

    def __init__(self, ws, columns, maxlane_func, zero_lane):

        self.ws = ws
        self.col = columns

        self.offset = (
            self.col["氏名2"] - self.col["氏名"]
        )

        self.maxlane_func = maxlane_func
        self.zero_lane = zero_lane

        # 段番号
        # 0: 1ページ左
        # 1: 1ページ右
        # 2: 2ページ左 ...
        self.segment = 0

        # 現在の段で使用した行数
        self.used = 0

        self.last_prg = None

    # --------------------------------------
    # 現在位置
    # --------------------------------------

    def position(self):

        page = self.segment // 2
        side = self.segment % 2

        row = (
            self.START_ROW
            + page * self.PAGE_ROWS
            + self.used
        )

        offset = side * self.offset

        return row, offset

    # --------------------------------------
    # 残り行数
    # --------------------------------------

    def remaining(self):

        return self.ROWS_PER_SIDE - self.used

    # --------------------------------------
    # 段を進める
    # --------------------------------------

    def next_segment(self):

        self.segment += 1
        self.used = 0

        if self.segment >= self.TEMPLATE_PAGES * 2:

            raise ValueError(
                "テンプレートのページ数を超えました。"
            )

    # --------------------------------------
    # セル書き込み
    # --------------------------------------

    def write(self, name, row, offset, value):

        col = self.col[name] + offset

        self.ws.cell(
            row=row,
            column=col
        ).value = value

    # --------------------------------------
    # 競技ヘッダー
    # --------------------------------------

    def write_program(self, data):

        row, offset = self.position()

        self.write("PrgNo", row, offset, data["prgNo"])
        self.write("gender", row, offset, data["gender"])
        self.write("distance", row, offset, data["distance"])
        self.write("style", row, offset, data["style"])
        self.write("yoketsu", row, offset, data["yoketsu"])

        # 上下に罫線
        side = Side(style="thin")

        start_col = self.col["kumi"] + offset
        end_col = self.col["CoE"] + offset

        for col in range(start_col, end_col + 1):

            cell = self.ws.cell(row=row, column=col)

            border = copy(cell.border)
            border.top = side
            border.bottom = side

            cell.border = border

        self.used += 1

    # --------------------------------------
    # 組番号
    # --------------------------------------

    def write_kumi(self, kumi):

        row, offset = self.position()

        self.write(
            "kumi",
            row,
            offset,
            f"{kumi}組"
        )

        self.used += 1

    # --------------------------------------
    # レーン情報
    # --------------------------------------

    def write_lane(self, lane, data=None):

        row, offset = self.position()

        # レーン番号は必ず書く
        self.write("lane", row, offset, lane)

        # 空きレーンならここで終了
        if data is not None:

            self.write("氏名", row, offset, data["name"])

            belongsto = data["belongsto"]

            self.write(
                "所属",
                row,
                offset,
                f"({belongsto})" if belongsto else ""
            )

            self.write("クラス", row, offset, data["class"])
            self.write("St", row, offset, data["st"])
            self.write("CoE", row, offset, data["coe"])

        self.used += 1

    # --------------------------------------
    # 組の必要行数
    # --------------------------------------

    def group_rows(self, maxlane):

        first_lane = 0 if self.zero_lane else 1

        lane_count = maxlane - first_lane + 1

        # 組番号1行 + 全レーン
        return 1 + lane_count

    # --------------------------------------
    # 組を出力
    # --------------------------------------

    def write_group(self, kumi, swimmers, maxlane,
                    new_program=False):

        # 組間の空行
        blank_rows = 0 if new_program else 1

        needed = blank_rows + self.group_rows(maxlane)

        # 現在の段に収まるか確認
        if needed > self.remaining():

            self.next_segment()

            # 段が変わった場合は競技ヘッダーを再表示
            # 新しい段でも競技番号が分かるようにする
            if self.last_prg is not None:

                self.write_program(self.current_program)

            needed = self.group_rows(maxlane)

        if needed > self.remaining():

            raise ValueError(
                f"{kumi}組が1段に収まりません。"
            )

        # 組間の空行
        self.used += blank_rows

        # 組番号
        self.write_kumi(kumi)

        # 全レーンを出力
        first_lane = 0 if self.zero_lane else 1

        for lane in range(first_lane, maxlane + 1):

            self.write_lane(
                lane,
                swimmers.get(lane)
            )

    # --------------------------------------
    # 競技単位の処理
    # --------------------------------------

    def write_program_groups(self, prg, groups):

        self.current_program = prg
        self.last_prg = prg["prgNo"]

        maxlane = self.maxlane_func(prg["prgNo"])

        first = True

        for kumi, swimmers in groups:

            # 最初の組はヘッダーも必要
            if first:

                needed = 1 + self.group_rows(maxlane)

                if needed > self.remaining():
                    self.next_segment()

                self.write_program(prg)

                self.write_group(
                    kumi,
                    swimmers,
                    maxlane,
                    new_program=True
                )

                first = False

            else:

                self.write_group(
                    kumi,
                    swimmers,
                    maxlane
                )
# ==========================================
# メイン
# ==========================================

from itertools import groupby


def main():

    # SQL実行
    records = execute(SQL, EVENT_NO)

    print(f"{len(records)} records")

    if not records:
        print("データがありません。")
        return

    # ゼロコース使用
    zero = execute(SQL_ZERO_LANE, EVENT_NO)

    zero_lane = bool(zero[0]["ゼロコース使用"])

    # MAXLANE取得関数
    def get_maxlane(prgNo):

        result = execute(
            SQL_LANE_INFO,
            EVENT_NO,
            prgNo
        )

        if not result:
            raise ValueError(
                f"競技番号 {prgNo} のMAXLANEが取得できません。"
            )

        return result[0]["MAXLANE"]

    # Excel読み込み
    wb = load_workbook(TEMPLATE_FILE)

    ws = wb["Sheet1"]

    columns = get_columns(wb)

    writer = LaneWriter(
        ws,
        columns,
        get_maxlane,
        zero_lane
    )

    # ----------------------------------
    # 競技番号ごとに処理
    # ----------------------------------

    for prgNo, prg_records in groupby(
        records,
        key=lambda x: x["prgNo"]
    ):

        prg_records = list(prg_records)

        prg = prg_records[0]

        # 組ごとにまとめる
        groups = []

        for kumi, group_records in groupby(
            prg_records,
            key=lambda x: x["kumi"]
        ):

            swimmers = {
                r["lane"]: r
                for r in group_records
            }

            groups.append((kumi, swimmers))

        # 競技単位で書き込み
        writer.write_program_groups(
            prg,
            groups
        )

    # 保存
    wb.save(OUTPUT_FILE)

    print(f"Saved: {OUTPUT_FILE}")



if __name__ == "__main__":

    main()
