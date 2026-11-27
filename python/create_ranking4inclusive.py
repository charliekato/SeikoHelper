#!/usr/bin/env python3

import sys
import os
import pyodbc
import json
from openpyxl import load_workbook
#改ページ挿入
from openpyxl.worksheet.pagebreak import Break
from openpyxl.styles import Font
import win32com.client

sys.stdout.reconfigure(encoding='utf-8')
sys.stderr.reconfigure(encoding='utf-8')
# ===== 設定 =====

#ラップを配置するカラム間隔
lapinterval = 4 

# データを配置する行間隔
rowInterval = 4
# SQL Server 接続文字列 (not used )
CONNECTION_STR = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=localhost;"
    "DATABASE=sw;"
    "Trusted_Connection=yes;"
    "TrustServerCertificate=yes;"
)
CONFIG_FILE = os.path.join(
        os.path.expanduser("~"),
        "AppData",
        "Local",
        "SeikoHelper",
        "create_ranking.config"
        )
def load_config():
    with open(CONFIG_FILE, "r", encoding="utf-8") as f:
        return json.load(f)


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

def write_excel_cell(ws, row, column, data,fontsize=11):
    ws.cell(row,column).value = data
    ws.cell(row,column).font = Font(size=fontsize)

def cell_horizontal_alignment(ws, row, column, align="center"):
    from copy import copy
    cell = ws.cell(row,column)
    alignment = copy(cell.alignment)
    alignment.horizontal = align
    cell.alignment = alignment


# ===== メイン処理 =====

def main():

    config = load_config()
    # 引数取得
    eventNo = int(config["EventNo"])
    prgNo = int(config["PrgNo"])
    template = config["TemplateFile"]
    connectionString = config["ConnectionString"]

    # Excel template 読み込み
    wb = load_workbook(template)

    ws = wb.active

    # 名前付きセルから column を取得
    順位col = get_cell_info(wb,ws, "順位")[1]
    組_水路col = get_cell_info(wb,ws, "組_水路")[1]
    氏名col = get_cell_info(wb,ws, "氏名")[1]
    所属col = get_cell_info(wb,ws, "所属")[1]
##    学年col = get_cell_info(wb,ws, "学年")[1]
    記録col = get_cell_info(wb,ws, "記録")[1]
    ポイントcol = get_cell_info(wb,ws, "ﾎﾟｲﾝﾄ")[1]
    リアクションcol = get_cell_info(wb,ws, "リアクション")[1]
    表示用競技番号row, 表示用競技番号col = get_cell_info(wb,ws, "表示用競技番号")
    クラスcol = get_cell_info(wb,ws,"クラス")[1]

    種目row, 種目col = get_cell_info(wb,ws,"種目")
    距離row, 距離col = get_cell_info(wb,ws,"距離")
    性別row, 性別col = get_cell_info(wb,ws,"性別")
    予決row, 予決col = get_cell_info(wb,ws,"予決")
    # start の row
    startRow = get_cell_info(wb,ws, "start")[0]

    # 大会名をヘッダーに
    sql = """
        SELECT
            大会名1,
            始期間,
            終期間
        FROM 大会設定
        WHERE 大会番号=?
    """
    with pyodbc.connect(connectionString) as conn:
        cursor = conn.cursor()
        cursor.execute(sql, eventNo)
        row = cursor.fetchone()

    if row.始期間 == row.終期間:
        hizuke = row.始期間
    else:
        hizuke = f"{row.始期間}～{row.終期間}"
    ws.oddHeader.right.text = hizuke
    ws.oddHeader.left.text = row.大会名1

    sql = """
        SELECT 
            性別,
            種目,
            距離,
            予決,
            表示用競技番号 ,
            LAPUNIT
        FROM vプログラム
        WHERE 大会番号 = ?
          AND 表示用競技番号 = ?
    """
    # SQL Server 接続
    with pyodbc.connect(connectionString) as conn:
        cursor = conn.cursor()
        cursor.execute(sql, eventNo, prgNo)
        row = cursor.fetchone()
    # excelのヘッダー部分に書き込み
    distance = row.距離
    write_excel_cell(ws,種目row,種目col,row.種目,12)
    write_excel_cell(ws,距離row,距離col,row.距離,12)
    write_excel_cell(ws,性別row,性別col,row.性別,12)
    write_excel_cell(ws,予決row,予決col,row.予決,12)
    
    write_excel_cell(ws,表示用競技番号row,表示用競技番号col,str(row.表示用競技番号),12)
    lapunit = row.LAPUNIT
    laprow, s1500 = get_cell_info(wb,ws,"L1500start")
    s800 = get_cell_info(wb,ws,"L800start")[1]
    s400 = get_cell_info(wb,ws,"L400start")[1]
    s200 = get_cell_info(wb,ws,"L200start")[1]
    s100 = get_cell_info(wb,ws,"L100start")[1]
    # ラップの距離をheader 部分に書き込む
    lapstr = ["50m","100m","150m","200m"]
    match distance:
        case " 200m":
            for j in range(0,4):
                this_column = s200+j*lapinterval
                write_excel_cell(ws,laprow,this_column,lapstr[j],10)
        case " 100m":
            for j in range(0,2):
                this_column = s100+j*lapinterval
                write_excel_cell(ws,laprow,this_column,lapstr[j],10)





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
            ゴール,
            クラス名称,
            ポイント,
            リアクション
        FROM vポイントランキング
        WHERE 大会番号 = ?
          AND 表示用競技番号 = ?
    """

    # SQL Server 接続
    with pyodbc.connect(connectionString) as conn:
        cursor = conn.cursor()
        cursor.execute(sql, eventNo, prgNo)
        rows = cursor.fetchall()

    # Excel に書き込み
    for i, row in enumerate(rows):

        excel_row = startRow + i * rowInterval


        write_excel_cell(ws,excel_row, 順位col, str(row.順位),12)
        write_excel_cell(ws,excel_row, 組_水路col, f"{row.組}/{row.水路}",12)
        write_excel_cell(ws,excel_row, 氏名col, row.氏名,12)
        write_excel_cell(ws,excel_row, 所属col, row.所属名,12)
        write_excel_cell(ws,excel_row, 所属col, row.所属名,12)
        write_excel_cell(ws,excel_row, クラスcol, row.クラス名称,12)
#        if row.学種=="一般" :
#            ws.cell(excel_row, 学年col).value = row.学種
#        else:
#            ws.cell(excel_row, 学年col).value = f"{row.学種[0]}{row.学年}"
        write_excel_cell(ws,excel_row, 記録col, row.ゴール,12)
        write_excel_cell(ws,excel_row, ポイントcol, str(row.ポイント))
        # 既存の書式を維持しながら左寄せに変更
        cell_horizontal_alignment(ws,excel_row,ポイントcol,"left")
        write_excel_cell(ws, excel_row+1,リアクションcol,f"({row.リアクション})",10)
        match lapunit:
            case 50:
                match distance:
                    case "1500m" :
                        for j  in range(0,10) :
                            this_row=excel_row+1
                            this_column = s1500+j*lapinterval
                            write_excel_cell(ws,this_row,this_column,row[j*2+1],10)
                            this_row +=1
                            write_excel_cell(ws,this_row,this_column,row[j*2+21],10)
                            this_row +=1
                            write_excel_cell(ws,this_row,this_column,row[j*2+41],10)
                    case " 800m" :
                        for j in range(0,8) :
                            this_row=excel_row+1
                            this_column = s800+j*lapinterval
                            write_excel_cell(ws,this_row,this_column,row[j*2+1],10)
                            this_row += 1
                            write_excel_cell(ws,this_row,this_column,row[j*2+17],10)
                    case " 400m" :
                        for j in range(0,8):
                            this_row=excel_row+1
                            this_column = s400+j*lapinterval
                            write_excel_cell(ws,this_row,this_column,row[j*2+1],10)
                    case " 200m" :
                        for j in range(0,4):
                            this_row=excel_row+1
                            this_column = s200+j*lapinterval
                            write_excel_cell(ws,this_row,this_column,row[j*2+1],10)
                    case " 100m" :
                        for j in range(0,2):
                            this_row=excel_row+1
                            this_column = s100+j*lapinterval
                            write_excel_cell(ws,this_row,this_column,row[j*2+1],10)
        if (i+1) % 11 == 0:
            # insert 改ページ
            ws.row_breaks.append(Break(id=excel_row+3))

    # 保存
    local_app_data = os.environ["LOCALAPPDATA"]

    data_dir = os.path.join(local_app_data, "SeikoHelper")
    output_dir = os.path.join(data_dir, "output")
    output = f"{output_dir}/ランキング{eventNo}_{prgNo}.xlsx"
    wb.save(output)

    print(f"{output} を作成しました。")

    excel = win32com.client.Dispatch("Excel.Application")
    excel.Visible = True

    try:
        wb = excel.Workbooks.Open(output)
        wb.Worksheets("Sheet1").PrintPreview()

        # ブック全体を印刷
        #wb.PrintOut()

        # 特定のシートだけ印刷する場合
        # wb.Worksheets("ランキング").PrintOut()

        wb.Close(SaveChanges=False)

    finally:
        excel.Quit()

if __name__ == "__main__":
    main()
