Attribute VB_Name = "templateTool"



Const StartRow As Long = 5
Const ClassCol As Long = 1
Const StyleCol As Long = 2
Const DistanceCol As Long = 3
Const TimeCol As Long = 4
Const NameCol As Long = 5
Const BelongsToCol As Long = 6
Const TournamentRecordCol As Long = 7
Const Offset As Long = 4

Function last_row(column As Long) As Long
  last_row = Cells(Rows.Count, column).End(xlUp).row
End Function
Function NotValid(value As String) As Boolean
    NotValid = False
    If Trim(value) = "25m" Then Exit Function
    If Trim(value) = "50m" Then Exit Function
    If Trim(value) = "100m" Then Exit Function
    If Trim(value) = "200m" Then Exit Function
    If Trim(value) = "400m" Then Exit Function
    If Trim(value) = "800m" Then Exit Function
    If Trim(value) = "1500m" Then Exit Function
    If Trim(value) = "4x25m" Then Exit Function
    If Trim(value) = "4x50m" Then Exit Function
    If Trim(value) = "4x100m" Then Exit Function
    If Trim(value) = "4x200m" Then Exit Function
    NotValid = True
    Exit Function
End Function
Function GetClassNoByName(className As String) As Variant
    On Error GoTo ErrHandler

    GetClassNoByName = Application.WorksheetFunction.VLookup( _
        className, _
        Range("classTable"), _
        2, _
        False)

    Exit Function

ErrHandler:
    GetClassNoByName = CVErr(xlErrNA)
End Function


Function GetStyleCodeByName(style As String) As Variant
    GetStyleCodeByName = 0
    If style = "自由形" Then GetStyleCodeByName = 1
    If style = "背泳ぎ" Then GetStyleCodeByName = 2
    If style = "平泳ぎ" Then GetStyleCodeByName = 3
    If style = "バタフライ" Then GetStyleCodeByName = 4
    If style = "個人メドレー" Then GetStyleCodeByName = 5
    If style = "フリーリレー" Then GetStyleCodeByName = 6
    If style = "メドレーリレー" Then GetStyleCodeByName = 7
End Function
Function GetGenderCodeByName(gender As String) As Variant
    GetGenderCodeByName = 0
    If gender = "男子" Then GetGenderCodeByName = 1
    If gender = "女子" Then GetGenderCodeByName = 2
    If gender = "混成" Then GetGenderCodeByName = 3
    If gender = "混合" Then GetGenderCodeByName = 4
    

End Function

Function GetDistanceCodeByName(distance As String) As Variant
    GetDistanceCodeByName = 0
    If Trim(distance) = "25m" Then GetDistanceCodeByName = 1
    If Trim(distance) = "50m" Then GetDistanceCodeByName = 2
    If Trim(distance) = "100m" Then GetDistanceCodeByName = 3
    If Trim(distance) = "200m" Then GetDistanceCodeByName = 4
    If Trim(distance) = "400m" Then GetDistanceCodeByName = 5
    If Trim(distance) = "800m" Then GetDistanceCodeByName = 6
    If Trim(distance) = "1500m" Then GetDistanceCodeByName = 7
End Function
Sub AssignName()
    Dim row As Long
    Dim class As String
    Dim style As Integer
    Dim distance As Integer
    Dim classNo As Variant
    Dim rowtop As Long
    For row = StartRow To last_row(3)
        If NotValid(Cells(row, DistanceCol)) Then GoTo ENDLOOP
        If Cells(row, ClassCol).value <> "" Then
            class = Cells(row, ClassCol).value
            classNo = GetClassNoByName(class)
        End If
        If Cells(row, StyleCol).value <> "" Then
            style = GetStyleCodeByName(Cells(row, StyleCol).value)
        End If
        distance = GetDistanceCodeByName(Cells(row, DistanceCol).value)
        
        Cells(row, TimeCol).Name = "男子" & classNo & "_" & distance & "_" & style & "Time"
        Cells(row, NameCol).Name = "男子" & classNo & "_" & distance & "_" & style & "Name"
        Cells(row, BelongsToCol).Name = "男子" & classNo & "_" & distance & "_" & style & "Team"
        Cells(row, TournamentRecordCol).Name = "男子" & classNo & "_" & distance & "_" & style & "GR"
        Cells(row, TimeCol + Offset).Name = "女子" & classNo & "_" & distance & "_" & style & "Time"
        Cells(row, NameCol + Offset).Name = "女子" & classNo & "_" & distance & "_" & style & "Name"
        Cells(row, BelongsToCol + Offset).Name = "女子" & classNo & "_" & distance & "_" & style & "Team"
        Cells(row, TournamentRecordCol + Offset).Name = "女子" & classNo & "_" & distance & "_" & style & "GR"
        
        
ENDLOOP:
        Next row
End Sub


Sub trimall()
    Dim col, row As Long
    For col = 6 To 10 Step 4
        For row = 1 To last_row(6)
            Cells(row, col).value = Trim(Cells(row, col).value)
        Next row
    Next col
End Sub

Sub alttimego()
  Dim col As Long
  Dim row As Long
  col = 4
  For row = 5 To last_row(col)
    Cells(row, 13).value = timeint2str(Cells(row, col).value)
  Next row

  col = 8
  For row = 5 To last_row(col)
    Cells(row, 14).value = timeint2str(Cells(row, col).value)
  Next row

End Sub
Function timeint2str(myTime As Long) As String
  Dim minutes As String
  Dim intminutes As Integer
  Dim seconds As String
  Dim centiSecond As String
  Dim orgMyTime As Long
  If (myTime = 0) Then
    timeint2str = ""
    Exit Function
  End If

  orgMyTime = myTime

  minutes = Right(" " + CStr(Int(myTime / 10000)), 2)
  myTime = myTime Mod 10000
  seconds = Right("0" + CStr(Int(myTime / 100)), 2)
  myTime = myTime Mod 100
  centiSecond = Right("0" + CStr(myTime), 2)
  If orgMyTime >= 10000 Then
    timeint2str = minutes + ":" + seconds + "." + centiSecond
  Else
    timeint2str = "   " + seconds + "." + centiSecond
  End If
End Function
