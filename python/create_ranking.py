#!/usr/bin/env python3

import sys
import pyodbc
from openpyxl import load_workbook


# ===== 設定 =====

TEMPLATE = "template/種目別競技結果template.xlsx"
lapinterval = 4

# SQL Server 接続文字列
CONNECTION_STR = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=localhost;"
    "DATABASE=sw;"
    "Trusted_Connection=yes;"
    "TrustServerCertificate=yes;"
)

# データを配置する行間隔
rowInterval = 4


# ===== Excel の名前付きセルを取得 =====
def get_cell_info(wb, ws, name):
    """
    名前付きセルから row と column を取得する。
    Workbook scope と Worksheet scope の両方に対応。
    """

    # Worksheet scope を優先
    if name in ws.defined_names:
        defined_name = ws.defined_names[name]

    # Workbook scope
    elif name in wb.defined_names:
        defined_name = wb.defined_names[name]

    else:
        raise KeyError(f"名前付きセル '{name}' が見つかりません")

    # 名前付きセルの参照先を取得
    sheet_name, cell_address = list(defined_name.destinations)[0]

    from openpyxl.utils.cell import coordinate_from_string
    from openpyxl.utils.cell import column_index_from_string

    column_letter, row = coordinate_from_string(
        cell_address.replace("$", "")
    )

    column = column_index_from_string(column_letter)

    return row, column

# ===== メイン処理 =====

def main():

    # 引数取得
    eventNo = int(sys.argv[1])
    prgNo = int(sys.argv[2])

    # Excel template 読み込み
    wb = load_workbook(TEMPLATE)

    ws = wb.active

    # 名前付きセルから column を取得
    順位col = get_cell_info(wb,ws, "順位")[1]
    組_水路col = get_cell_info(wb,ws, "組_水路")[1]
    氏名col = get_cell_info(wb,ws, "氏名")[1]
    所属col = get_cell_info(wb,ws, "所属")[1]
    学年col = get_cell_info(wb,ws, "学年")[1]
    記録col = get_cell_info(wb,ws, "記録")[1]
    ポイントcol = get_cell_info(wb,ws, "ﾎﾟｲﾝﾄ")[1]
    リアクションcol = get_cell_info(wb,ws, "リアクション")[1]

    種目row, 種目col = get_cell_info(wb,ws,"種目")
    距離row, 距離col = get_cell_info(wb,ws,"距離")
    性別row, 性別col = get_cell_info(wb,ws,"性別")
    予決row, 予決col = get_cell_info(wb,ws,"予決")

    # start の row
    startRow = get_cell_info(wb,ws, "start")[0]
    sql = """
        SELECT 
            性別,
            種目,
            距離,
            予決,
            LAPUNIT
        FROM vプログラム
        WHERE 大会番号 = ?
          AND 表示用競技番号 = ?
    """
    # SQL Server 接続
    with pyodbc.connect(CONNECTION_STR) as conn:
        cursor = conn.cursor()
        cursor.execute(sql, eventNo, prgNo)
        row = cursor.fetchone()
    # excelのヘッダー部分に書き込み
    distance = row.距離
    ws.cell(種目row, 種目col).value = row.種目
    ws.cell(距離row, 距離col).value = distance
    ws.cell(性別row, 性別col).value = row.性別
    ws.cell(予決row, 予決col).value = row.予決
    lapunit = row.LAPUNIT
    s1500 = get_cell_info(wb,ws,"L1500start")[1]
    s800 = get_cell_info(wb,ws,"L800start")[1]
    s400 = get_cell_info(wb,ws,"L400start")[1]
    s200 = get_cell_info(wb,ws,"L200start")[1]
    s100 = get_cell_info(wb,ws,"L100start")[1]



    # SQL
    sql = """
        SELECT
	    [25m], [50m], [75m], [100m], [125m], [150m], [175m], [200m],
	   [225m], [250m], [275m], [300m], [325m], [350m], [375m], [400m], 
	   [425m], [450m], [475m], [500m], [525m], [550m], [575m], [600m],
	   [625m], [650m], [675m], [700m], [725m], [750m], [775m], [800m],
	   [825m], [850m], [875m], [900m], [925m], [950m], [975m], [1000m],
	   [1025m],[1050m],[1075m],[1100m],[1125m],[1150m],[1175m],[1200m],
	   [1225m],[1250m],[1275m],[1300m],[1325m],[1350m],[1375m],[1400m],
	   [1425m],[1450m],[1475m],[1500m],
            順位,
            組,
            水路,
            氏名,
            所属名,
            学種,
            学年,
            ゴール,
            ポイント,
            リアクション
        FROM vポイントランキング
        WHERE 大会番号 = ?
          AND 表示用競技番号 = ?
    """

    # SQL Server 接続
    with pyodbc.connect(CONNECTION_STR) as conn:
        cursor = conn.cursor()
        cursor.execute(sql, eventNo, prgNo)
        rows = cursor.fetchall()

    # Excel に書き込み
    for i, row in enumerate(rows):

        excel_row = startRow + i * rowInterval


        ws.cell(excel_row, 順位col).value = str(row.順位)
        ws.cell(excel_row, 組_水路col).value = f"{row.組}/{row.水路}"
        ws.cell(excel_row, 氏名col).value = row.氏名
        ws.cell(excel_row, 所属col).value = row.所属名
        if row.学種=="一般" :
            ws.cell(excel_row, 学年col).value = row.学種
        else:
            ws.cell(excel_row, 学年col).value = f"{row.学種[0]}{row.学年}"
        ws.cell(excel_row, 記録col).value = row.ゴール
        ws.cell(excel_row, ポイントcol).value = str(row.ポイント)
        # 既存の書式を維持しながら左寄せに変更
        from copy import copy
        cell = ws.cell(excel_row,ポイントcol)
        alignment = copy(cell.alignment)
        alignment.horizontal = "left"
        cell.alignment = alignment
        ws.cell(excel_row+1, リアクションcol).value = row.リアクション
        match lapunit:
            case 50:
                match distance:
                    case "1500m" :
                        for i  in range(0,10) :
                            ws.cell(excel_row+1, s1500+i*lapinterval).value = row[i*2+1]
                            ws.cell(excel_row+2, s1500+i*lapinterval).value = row[i*2+21]
                            ws.cell(excel_row+3, s1500+i*lapinterval).value = row[i*2+41]
                    case " 800m" :
                        for i in range(0,8) :
                            ws.cell(excel_row+1, s800+i*lapinterval).value = row[i*2+1]
                            ws.cell(excel_row+2, s800+i*lapinterval).value = row[i*2+17]
                    case " 400m" :
                        for i in range(0,8):
                            ws.cell(excel_row+1, s400+i*lapinterval).value = row[i*2+1]
                    case " 200m" :
                        for i in range(0,4):
                            ws.cell(excel_row+1, s200+i*lapinterval).value = row[i*2+1]
                    case " 100m" :
                        for i in range(0,2):
                            ws.cell(excel_row+1, s100+i*lapinterval).value = row[i*2+1]




    # 保存
    output = f"output/ランキング{eventNo}_{prgNo}.xlsx"
    wb.save(output)

    print(f"{output} を作成しました。")


if __name__ == "__main__":
    main()
