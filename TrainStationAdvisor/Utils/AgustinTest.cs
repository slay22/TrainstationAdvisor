using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TrainStationAdvisor.ClassLibrary
{
    class AgustinTest
    {
        /*
        Public Class GPS
            Private WithEvents ComPort As New OpenNETCF.IO.Serial.Port("COM7:")
            Private WithEvents ComPort2 As New System.IO.Ports.SerialPort()
            Dim buff() As String
            Dim str As String
            Dim enc As New System.Text.ASCIIEncoding
            Dim lineas() As String
            Dim linea As String
            Dim buff2() As Byte

            Public Sub Open()
                Try
                    ComPort.Open()
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Exclamation)
                End Try
            End Sub

            Public Sub Open2(ByVal PortName As String)
                Try
                    ComPort2.PortName = PortName
                    ComPort2.BaudRate = 4800
                    ComPort2.ReadTimeout = 5000
                    ComPort2.Open()
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Exclamation)
                End Try
            End Sub

            Public Sub Close()
                Try
                    ComPort.Close()
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Exclamation)
                End Try
            End Sub

            Public Sub Close2()
                Try
                    ComPort2.Close()
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Exclamation)
                End Try
            End Sub

            Private Sub ComPort_DataReceived() Handles ComPort.DataReceived
                str = enc.GetString(ComPort.Input, 0, 275)
                lineas = str.Split(Chr(10))
                linea = lineas(0).TrimEnd(Chr(13))
            End Sub

            ReadOnly Property IsOpen() As Boolean
                Get
                    IsOpen = ComPort.IsOpen
                End Get
            End Property

            ReadOnly Property IsOpen2() As Boolean
                Get
                    IsOpen2 = ComPort2.IsOpen
                End Get
            End Property

            Public Function GetData()
                Try
                    Dim buffprov() As String
                    buffprov = linea.Split(",")
                    If buffprov(0) = "$GPGGA" Then
                        buff = buffprov
                    End If
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
            End Function

            ReadOnly Property IsFixed() As Boolean
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(6) <> "" Then
                            IsFixed = CBool(buff(6))
                        End If
                    End If
                End Get
            End Property

            ReadOnly Property GetDecimalDegreesLongitude()
                Get
                    If buff Is Nothing Then
                        GetDecimalDegreesLongitude = 0
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(5) = "W" Then
                            GetDecimalDegreesLongitude = Replace(FormatNumber((Mid(buff(4), 1, 3)) + (Mid(buff(4), 4, 2) / 60) + ((Mid(buff(4), 7, 4) * 0.006) / 3600), 6) * -1, ",", ".")
                        ElseIf buff(5) = "E" Then
                            GetDecimalDegreesLongitude = Replace(FormatNumber((Mid(buff(4), 1, 3)) + (Mid(buff(4), 4, 2) / 60) + ((Mid(buff(4), 7, 4) * 0.006) / 3600), 6), ",", ".")
                        End If
                    Else
                        GetDecimalDegreesLongitude = 0
                    End If
                End Get
            End Property

            ReadOnly Property GetDecimalDegreesLatitude()
                Get
                    If buff Is Nothing Then
                        GetDecimalDegreesLatitude = 0
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(3) = "N" Then
                            GetDecimalDegreesLatitude = Replace(FormatNumber((Mid(buff(2), 1, 2)) + (Mid(buff(2), 3, 2) / 60) + ((Mid(buff(2), 6, 4) * 0.006) / 3600), 6), ",", ".")
                        ElseIf buff(3) = "S" Then
                            GetDecimalDegreesLatitude = Replace(FormatNumber((Mid(buff(2), 1, 2)) + (Mid(buff(2), 3, 2) / 60) + ((Mid(buff(2), 6, 4) * 0.006) / 3600), 6) * -1, ",", ".")
                        End If
                    Else
                        GetDecimalDegreesLatitude = 0
                    End If
                End Get
            End Property

            ReadOnly Property GetLongitude()
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(5) = "W" Then
                            GetLongitude = Replace(FormatNumber(buff(4), 6) * -1, ",", ".")
                        ElseIf buff(5) = "E" Then
                            GetLongitude = Replace(FormatNumber(buff(4), 6), ",", ".")
                        End If
                    End If
                End Get
            End Property

            ReadOnly Property GetLatitude()
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(3) = "N" Then
                            GetLatitude = Replace(FormatNumber(buff(2), 6), ",", ".")
                        ElseIf buff(3) = "S" Then
                            GetLatitude = Replace(FormatNumber(buff(2), 6) * -1, ",", ".")
                        End If
                    End If
                End Get
            End Property

            ReadOnly Property GetSatellites() As Integer
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(7) <> "" Then
                            GetSatellites = CInt(buff(7))
                        End If
                    Else
                        GetSatellites = 0
                    End If
                End Get
            End Property

            ReadOnly Property GetAltitude() As Integer
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(9) <> "" Then
                            GetAltitude = CInt(buff(9))
                        End If
                    End If
                End Get
            End Property

            ReadOnly Property GetUTCDateTime() As Date
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(1) <> "" Then
                            GetUTCDateTime = CDate(buff(1))
                        End If
                    End If
                End Get
            End Property

            ReadOnly Property GetNorthSouth() As String
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(3) <> "" Then
                            GetNorthSouth = CStr(buff(3))
                        End If
                    End If
                End Get
            End Property

            ReadOnly Property GetEastWest() As String
                Get
                    If buff Is Nothing Then
                        Exit Property
                    End If
                    If buff(0) = "$GPGGA" Then
                        If buff(5) <> "" Then
                            GetEastWest = CStr(buff(5))
                        End If
                    End If
                End Get
            End Property

            Private Sub ComPort2_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles ComPort2.DataReceived
                str = ComPort2.ReadLine()
                If InStr(str, "$GPGGA") Then linea = str
            End Sub

            Private Sub ComPort2_ErrorReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialErrorReceivedEventArgs) Handles ComPort2.ErrorReceived
                MsgBox(e.EventType.ToString, MsgBoxStyle.Exclamation)
            End Sub
        End Class 
         */


    }
}
