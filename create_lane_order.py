#!/usr/bin/env python3

import json
import pyodbc
import os

from pathlib import Path
from openpyxl import load_workbook
from openpyxl.styles import Side
from copy import copy
from itertools import groupby
from openpyxl.utils.cell import coordinate_from_string
from openpyxl.utils import column_index_from_string


# ==========================================
# 設定読み込み
# ==========================================

BASE_DIR = Path(__file__).resolve().parent

with open(BASE_DIR / "config.json", encoding="utf-8") as f:
    CONFIG = json.load(f)


DB = CONFIG["database"]
OPT = CONFIG["lane_order"]

EVENT_NO = int(OPT["event_no"])
START_ROW = int(OPT["start_row"])
PARA_MODE = int(OPT["para_mode"])

TEMPLATE_FILE = BASE_DIR / OPT["template_file"]
OUTPUT_FILE = BASE_DIR / OPT["output_file"]
#-------------------------------------------------------


# ==========================================
# 設定
# ==========================================


def get_connection_string():

    conn = (
        "DRIVER={ODBC Driver 18 for SQL Server};"
        f"Server={DB['server']};"
        f"Database={DB['database']};"
        "TrustServerCertificate=yes;"
    )

    if DB["authentication"] == "Windows":

        conn += "Trusted_Connection=yes;"

    elif DB["authentication"] == "SQL":

        conn += (
            f"UID={DB['username']};"
            f"PWD={DB['password']};"
        )

    else:
        raise ValueError("不明な認証方式です。")

    return conn


CONNECTION_STR = get_connection_string()






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
    時間 as startTime
from vレーン順
where 大会番号=?
order by prgNo, kumi, lane
"""


SQL_PROGRAM_INFO = """
select
    表示用競技番号 as prgNo,
    MAXLANE as maxlane,
    ゼロコース使用 as zeroLane,
    組数 as kumiCount
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
# 名前付きセルの列番号
# ==========================================

def get_columns(wb):

    names = [
        "PrgNo",
        "gender",
        "distance",
        "style",
        "yoketsu",
        "時間",
        "kumi",
        "lane",
        "氏名",
        "所属",
        "クラス",
        "St",
        "CoE",
        "氏名2",
        "EndOfLine",
    ]

    result = {}

    for name in names:

        defined_name = wb.defined_names[name]

        sheet_name, coordinate = next(
            defined_name.destinations
        )

        col_letter, row = coordinate_from_string(
            coordinate
        )

        result[name] = column_index_from_string(
            col_letter
        )

    return result


# ==========================================
# LaneWriter
# ==========================================

class LaneWriter:

    # ページ設定
    PAGE_ROWS = 61

    # 左右それぞれの使用行数
    ROWS_PER_SIDE = 56

    # 最初のデータ行
    def __init__(self, ws, columns, programs):

        self.ws = ws
        self.col = columns
        self.programs = programs

        # 右側への列オフセット
        self.offset = (
            self.col["氏名2"] - self.col["氏名"]
        )

        # 左右・ページ位置
        self.segment = 0

        # 現在の段で使用した行数
        self.used = 0

        # テンプレートのページ数
        self.pages = len(ws.row_breaks.brk) + 1

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
    # 次の段へ
    # --------------------------------------

    def next_segment(self):

        self.segment += 1
        self.used = 0

        if self.segment >= self.pages * 2:

            raise ValueError(
                "テンプレートのページ数を超えました。"
            )

    # --------------------------------------
    # セル書き込み
    # --------------------------------------

    def write(self, name, row, offset, value, 
              font_size=None,horizontal=None,vertical="center"):

        col = self.col[name] + offset

        cell=self.ws.cell(
            row=row,
            column=col
        )
        cell.value = value
        if font_size is not None:
            font = copy(cell.font)
            font.sz = font_size
            cell.font = font
        
        if horizontal is not None or vertical is not None:
            alignment = copy(cell.alignment)

            if horizontal is not None:
                alignment.horizontal = horizontal

            if vertical is not None:
                alignment.vertical = vertical

            cell.alignment = alignment

    # --------------------------------------
    # 競技ヘッダー
    # --------------------------------------

    def write_program(self, data):

        row, offset = self.position()

        self.write("PrgNo", row, offset, data["prgNo"],8)
        self.write("gender", row, offset, data["gender"],8)
        self.write("distance", row, offset, data["distance"],8)
        self.write("style", row, offset, data["style"],8)
        self.write("yoketsu", row, offset, data["yoketsu"],8)
        self.write("時間", row, offset,  data["startTime"],8)

        # 上下罫線
        side = Side(style="thin")

        start_col = self.col["kumi"] + offset
        end_col = self.col["EndOfLine"] + offset

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
            f"{kumi}組",8
        )

        self.used += 1

    # --------------------------------------
    # レーン情報
    # --------------------------------------

    def write_lane(self, lane, data=None):

        row, offset = self.position()

        # レーン番号は必ず書く
        self.write("lane", row, offset, lane,8)

        if data is not None:

            self.write(
                "氏名",
                row,
                offset,
                data["name"],8
            )

            belongsto = data["belongsto"]

            self.write(
                "所属",
                row,
                offset,
                f"({belongsto})" if belongsto else "",
                font_size=7 if len(belongsto)>12 else 8
            )

            self.write(
                "クラス",
                row,
                offset,
                data["class"],8
            )
            if PARA_MODE == 1:

                self.write(
                    "St",
                    row,
                    offset,
                    data["st"],8,"left"
                )

                self.write(
                    "CoE",
                    row,
                    offset,
                    data["coe"],8
                )

        self.used += 1

    # --------------------------------------
    # 1組に必要なレーン数
    # --------------------------------------

    def lane_count(self, info):

        first_lane = (
            0 if info["zeroLane"] else 1
        )

        return info["maxlane"] - first_lane + 1

    # --------------------------------------
    # 1組に必要な行数
    # --------------------------------------

    def group_rows(self, info):

        return 1 + self.lane_count(info)

    # --------------------------------------
    # 競技・組の書き込み
    # --------------------------------------

    def write_program_groups(self, prg, groups):

        info = self.programs[prg["prgNo"]]

        maxlane = info["maxlane"]

        first_lane = (
            0 if info["zeroLane"] else 1
        )

        # 組数
        kumi_count = info["kumiCount"]

        if len(groups) != kumi_count:

            raise ValueError(
                f"競技 {prg['prgNo']}: "
                f"vレーン順の組数({len(groups)})と"
                f"vプログラムの組数({kumi_count})が一致しません。"
            )

        # ----------------------------------
        # 組ごとに処理
        # ----------------------------------

        for index, (kumi, swimmers) in enumerate(groups):

            # 最終組かどうか
            last_kumi = (
                index == kumi_count - 1
            )

            # 組の後の空白
            gap = 2 if last_kumi else 3

            # 組番号 + 全レーン + 空白
            needed = (
                self.group_rows(info)
                + gap
            )

            # 競技ヘッダーが必要か
            need_header = (
                index == 0
                or self.used == 0
            )

            if need_header:
                needed += 1

            # 現在の段に収まるか
            if needed > self.remaining():

                self.next_segment()

                # 新しい段ではヘッダーが必要
                need_header = True

                needed = (
                    1
                    + self.group_rows(info)
                    + gap
                )

            if needed > self.remaining():

                raise ValueError(
                    f"競技 {prg['prgNo']} "
                    f"{kumi}組が1段に収まりません。"
                )

            # 競技ヘッダー
            if need_header:

                self.write_program(prg)

            # 組番号
            self.write_kumi(kumi)

            # 全レーン
            for lane in range(first_lane, maxlane + 1):

                self.write_lane(
                    lane,
                    swimmers.get(lane)
                )

            # 組間の空白
            self.used += gap


# ==========================================
# メイン
# ==========================================


def main():

    print(f"大会番号: {EVENT_NO}")
    print(f"開始行: {START_ROW}")
    print(f"Para mode: {PARA_MODE}")
    print(f"SQL Server: {DB['server']}")

    # SQL実行
    records = execute(SQL, EVENT_NO)

    print(f"レーンデータ: {len(records)} records")

    if not records:
        print("データがありません。")
        return

    program_records = execute(
        SQL_PROGRAM_INFO,
        EVENT_NO
    )

    programs = {}

    for r in program_records:

        programs[r["prgNo"]] = {
            "maxlane": r["maxlane"],
            "zeroLane": r["zeroLane"],
            "kumiCount": r["kumiCount"],
        }

    zero_values = {
        p["zeroLane"]
        for p in programs.values()
    }

    if len(zero_values) != 1:
        raise ValueError(
            "ゼロコース使用が競技間で一致していません。"
        )

    print(f"競技数: {len(programs)}")

    # Excel読み込み
    wb = load_workbook(TEMPLATE_FILE)

    ws = wb["Sheet1"]

    columns = get_columns(wb)

    writer = LaneWriter(
        ws,
        columns,
        programs
    )

    # 開始行を設定
    writer.START_ROW = START_ROW

    # 競技番号ごとに処理
    for prgNo, prg_records in groupby(
        records,
        key=lambda x: x["prgNo"]
    ):

        prg_records = list(prg_records)

        prg = prg_records[0]

        group_dict = {}

        for kumi, group_records in groupby(
            prg_records,
            key=lambda x: x["kumi"]
        ):

            group_dict[kumi] = {
                r["lane"]: r
                for r in group_records
            }

        kumi_count = programs[prgNo]["kumiCount"]

        missing = (
            set(range(1, kumi_count + 1))
            - set(group_dict)
        )

        if missing:
            raise ValueError(
                f"競技 {prgNo}: "
                f"データが存在しない組があります: {sorted(missing)}"
            )

        groups = [
            (kumi, group_dict[kumi])
            for kumi in range(1, kumi_count + 1)
        ]

        print(
            f"PrgNo={prgNo}, "
            f"MAXLANE={programs[prgNo]['maxlane']}, "
            f"組数={kumi_count}"
        )

        writer.write_program_groups(
            prg,
            groups
        )

    # 保存先ディレクトリがなければ作成
    OUTPUT_FILE.parent.mkdir(
        parents=True,
        exist_ok=True
    )

    wb.save(OUTPUT_FILE)

    print()
    print(f"Saved: {OUTPUT_FILE}")


if __name__ == "__main__":

    main()
