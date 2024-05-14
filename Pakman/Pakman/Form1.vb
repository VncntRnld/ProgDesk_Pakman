Imports System.Drawing
Imports System.Windows.Forms

Public Class Form1

    Private Sub defaultVal()
        tsz = 40 'tile size (ukuran grid/tilenya)
        pacx = 1 'pakman itu di petak x mana sekarang
        pacy = 1 'pakman itu di petak y mana sekarang

        enmx = 9 'musuh itu di petak x berapa
        enmy = 7 'musuh itu di petak y berapa
        enmx2 = 5
        enmy2 = 3
        enmx3 = 7
        enmx = 5


        goalx = 1 'goal (pintu keluar pakman di petak x brp
        goaly = 7 'goal di petak y berapa
        oldpacx = 0
        oldpacy = 0
        Redraw()
        Timer1.Enabled = True

    End Sub

    'map (peta papan permainan)
    Dim map = {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0},
        {0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0},
        {0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 0},
        {0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}
    }

    'kumpulan variabel
    Dim tsz = 40 'tile size (ukuran grid/tilenya)
    Dim pacx = 1 'pakman itu di petak x mana sekarang
    Dim pacy = 1 'pakman itu di petak y mana sekarang

    Dim enmx = 9 'musuh itu di petak x berapa
    Dim enmy = 7 'musuh itu di petak y berapa
    Dim enmx2 = 5 ' Second enemy x position
    Dim enmy2 = 3 ' Second enemy y position
    Dim enmx3 = 7 ' Third enemy x position
    Dim enmy3 = 5 ' Third enemy y position


    Dim goalx = 1 'goal (pintu keluar pakman di petak x brp
    Dim goaly = 7 'goal di petak y berapa
    Dim bmp As Bitmap
    Dim oldpacx = 0
    Dim oldpacy = 0

    Dim nyawa = 3

    'deklarasi sprite citra yang digunakan Image.FromFile("")
    Dim wall As Image = My.Resources.bata
    Dim way As Image = My.Resources.rumput
    Dim pac As Image = My.Resources.pakman
    Dim enm As Image = My.Resources.hantu3
    Dim goal As Image = My.Resources.omah
    Dim hati As Image = My.Resources.hati

    Private Sub Redraw()
        Dim g As Graphics = Graphics.FromImage(PictureBox1.Image)
        'gambarkan background (jalan dan tembok)
        For y = 0 To (map.GetUpperBound(0)) '0 to 8 yaitu tinggi map - 1
            For x = 0 To (map.GetUpperBound(1)) '0 to 10 yaitu lebar map - 1
                If map(y, x) = 1 Then
                    g.DrawImage(way, x * tsz, y * tsz, tsz, tsz)
                Else
                    g.DrawImage(wall, x * tsz, y * tsz, tsz, tsz)
                End If
            Next
        Next

        'nyawa
        For i = 0 To nyawa - 1
            g.DrawImage(hati, i * tsz, map.GetUpperBound(0) * tsz, tsz, tsz)
        Next

        'gambarkan pacman
        g.DrawImage(pac, pacx * tsz, pacy * tsz, tsz, tsz)

        'gambarkan musuh
        g.DrawImage(enm, enmx * tsz, enmy * tsz, tsz, tsz)
        ' Draw second enemy
        g.DrawImage(enm, enmx2 * tsz, enmy2 * tsz, tsz, tsz)
        ' Draw third enemy
        g.DrawImage(enm, enmx3 * tsz, enmy3 * tsz, tsz, tsz)

        'gambarkan goal
        g.DrawImage(goal, goalx * tsz, goaly * tsz, tsz, tsz)
        PictureBox1.Refresh()
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If map(pacy - 1, pacx) = 1 Then
                    pacy = pacy - 1
                    pac = My.Resources.pakman4
                End If
            Case Keys.Down
                If map(pacy + 1, pacx) = 1 Then
                    pacy = pacy + 1
                    pac = My.Resources.pakman2
                End If
            Case Keys.Right
                If map(pacy, pacx + 1) = 1 Then
                    pacx = pacx + 1
                    pac = My.Resources.pakman
                End If
            Case Keys.Left
                If map(pacy, pacx - 1) = 1 Then
                    pacx = pacx - 1
                    pac = My.Resources.pakman3
                End If
        End Select
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'sesuaikan dulu ukuran picturebox dan form
        PictureBox1.Width = (map.length / (map.GetUpperBound(0) + 1)) * tsz
        PictureBox1.Height = (map.GetUpperBound(0) + 1) * tsz
        Me.Width = PictureBox1.Width + tsz
        Me.Height = PictureBox1.Height + tsz
        bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        PictureBox1.Image = bmp
        Redraw()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'engine untuk musuh
        Dim jarakx, jaraky As Integer
        Dim arah As Integer 'arah musuh, 0 atas, 1 kanan, 2 bawah, 3 kiri
        'cek jarak pakman dg musuh. kalau lebih jauh di sb x, kejar di x dulu
        'kalau lebih dekat di sb y, kejar ke sb y dulu
        jarakx = Math.Abs(pacx - enmx)
        jaraky = Math.Abs(pacy - enmy)
        If jarakx > jaraky Then 'jika lebih jauh jarak kejar di x
            If (pacx - enmx > 0) Then 'jika pakman di kanan
                arah = 1 'arah kanan
            Else 'jika tidak
                arah = 3 'arah kiri
            End If
        End If
        If jarakx < jaraky Then
            If (pacy - enmy > 0) Then 'jika pakman di bawah
                arah = 2 'arah bawah
            Else 'jika tidak
                arah = 0 'arah atas
            End If
        End If
        If (oldpacx = pacx) And (oldpacy = pacy) Then 'jika stop
            arah = Math.Floor(Rnd() * 4)
        End If

        Select Case arah
            Case 0
                If map(enmy - 1, enmx) = 1 Then
                    enmy = enmy - 1
                End If
            Case 2
                If map(enmy + 1, enmx) = 1 Then
                    enmy = enmy + 1
                End If
            Case 1
                If map(enmy, enmx + 1) = 1 Then
                    enmx = enmx + 1
                End If
            Case 3
                If map(enmy, enmx - 1) = 1 Then
                    enmx = enmx - 1
                End If
        End Select

        ' Engine for second enemy
        Dim arah2 As Integer = Math.Floor(Rnd() * 4)
        Select Case arah2
            Case 0
                If map(enmy2 - 1, enmx2) = 1 Then
                    enmy2 = enmy2 - 1
                End If
            Case 1
                If map(enmy2 + 1, enmx2) = 1 Then
                    enmy2 = enmy2 + 1
                End If
            Case 2
                If map(enmy2, enmx2 + 1) = 1 Then
                    enmx2 = enmx2 + 1
                End If
            Case 3
                If map(enmy2, enmx2 - 1) = 1 Then
                    enmx2 = enmx2 - 1
                End If
        End Select

        ' Engine for third enemy
        Dim arah3 As Integer = Math.Floor(Rnd() * 4)
        Select Case arah3
            Case 0
                If map(enmy3 - 1, enmx3) = 1 Then
                    enmy3 = enmy3 - 1
                End If
            Case 1
                If map(enmy3 + 1, enmx3) = 1 Then
                    enmy3 = enmy3 + 1
                End If
            Case 2
                If map(enmy3, enmx3 + 1) = 1 Then
                    enmx3 = enmx3 + 1
                End If
            Case 3
                If map(enmy3, enmx3 - 1) = 1 Then
                    enmx3 = enmx3 - 1
                End If
        End Select


        oldpacx = pacx
        oldpacy = pacy
        Redraw()

        'cek apakah posisi pakman sama dg musuh
        If (pacx = enmx) And (pacy = enmy) Then
            Timer1.Enabled = False
            nyawa -= 1
            defaultVal()
        ElseIf (pacx = enmx2) And (pacy = enmy2) Then
            Timer1.Enabled = False
            nyawa -= 1
            defaultVal()
        ElseIf (pacx = enmx3) And (pacy = enmy3) Then
            Timer1.Enabled = False
            nyawa -= 1
            defaultVal()
        End If
        'cek apakah posisi pakman sama dg rumah
        If (pacx = goalx) And (pacy = goaly) Then
            Timer1.Enabled = False
            MsgBox("Pakman safe at Home!")
        End If

        'cek nyawa habis
        If nyawa = 0 Then
            Timer1.Enabled = False
            MsgBox("Pakman Dies!")
        End If
    End Sub
End Class