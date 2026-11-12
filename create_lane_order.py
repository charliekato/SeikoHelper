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


TEMPLATE_FILE = "インクルーシブ大会レーン順template.xlsx"

OUTPUT_FILE = f"インクルーシブ大会レーン順_{EVENT_NO}.xlsx"


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

    # ページ設定
    PAGE_ROWS = 61

    # 1段の使用行数
    ROWS_PER_SIDE = 56

    # 1ページの使用行数（左右合計）
    ROWS_PER_PAGE = ROWS_PER_SIDE * 2

    # 最初のデータ行
    START_ROW = 5

    # テンプレートのページ数
    TEMPLATE_PAGES = 8

    def __init__(self, ws, columns):

        self.ws = ws
        self.col = columns

        # 右側への列オフセット
        self.offset = (
            self.col["氏名2"] - self.col["氏名"]
        )

        # 書き込み行カウンター
        self.count = 0

        self.last_prg = None
        self.last_kumi = None

    # --------------------------------------
    # 現在の書き込み位置を取得
    # --------------------------------------

    def position(self):

        page = self.count // self.ROWS_PER_PAGE

        if page >= self.TEMPLATE_PAGES:
            raise ValueError(
                "テンプレートのページ数を超えました。"
            )

        within_page = self.count % self.ROWS_PER_PAGE

        side = within_page // self.ROWS_PER_SIDE

        row_offset = within_page % self.ROWS_PER_SIDE

        row = (
            self.START_ROW
            + page * self.PAGE_ROWS
            + row_offset
        )

        col_offset = side * self.offset

        return row, col_offset

    # --------------------------------------
    # セルに書き込む
    # --------------------------------------

    def write(self, name, row, col_offset, value):

        col = self.col[name] + col_offset

        self.ws.cell(row=row, column=col).value = value

    # --------------------------------------
    # 1行進める
    # --------------------------------------

    def advance(self):

        self.count += 1

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

        # ヘッダー行に下線を引く
        side = Side(style="thin")

        start_col = self.col["kumi"] + offset
        end_col = self.col["CoE"] + offset

        for col in range(start_col, end_col + 1):

            cell = self.ws.cell(row=row, column=col)

            border = copy(cell.border)
            border.bottom = side

            cell.border = border

        self.advance()

    # --------------------------------------
    # 組
    # --------------------------------------

    def write_kumi(self, kumi):

        row, offset = self.position()

        self.write(
            "kumi",
            row,
            offset,
            f"{kumi}組"
        )

        self.advance()

    # --------------------------------------
    # レーン情報
    # --------------------------------------

    def write_lane(self, data):

        row, offset = self.position()

        self.write("lane", row, offset, data["lane"])

        self.write("氏名", row, offset, data["name"])

        self.write(
            "所属",
            row,
            offset,
            f"({data['belongsto']})"
            if data["belongsto"] else ""
        )

        self.write("クラス", row, offset, data["class"])

        self.write("St", row, offset, data["st"])

        self.write("CoE", row, offset, data["coe"])

        self.advance()

    # --------------------------------------
    # 1レコード処理
    # --------------------------------------

    def add(self, data):

        prg = data["prgNo"]
        kumi = data["kumi"]

        # 競技番号が変わった
        if prg != self.last_prg:

            self.write_program(data)

            self.last_prg = prg
            self.last_kumi = None

        # 組が変わった
        if kumi != self.last_kumi:

            self.write_kumi(kumi)

            self.last_kumi = kumi

        # レーン情報
        self.write_lane(data)


# ==========================================
# メイン
# ==========================================

def main():

    # SQL実行
    records = execute(SQL, EVENT_NO)

    print(f"{len(records)} records")

    if not records:
        print("データがありません。")
        return

    # Excelテンプレート読み込み
    wb = load_workbook(TEMPLATE_FILE)

    ws = wb["Sheet1"]

    # 名前付きセルの列番号
    columns = get_columns(wb)

    # 書き込み
    writer = LaneWriter(ws, columns)

    for data in records:

        writer.add(data)

    # 保存
    wb.save(OUTPUT_FILE)

    print(f"Saved: {OUTPUT_FILE}")


if __name__ == "__main__":

    main()
