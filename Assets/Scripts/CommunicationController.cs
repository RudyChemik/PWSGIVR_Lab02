using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System;

public class CommunicationController : MonoBehaviour
{
    public Text msgReciver;
    public TMP_InputField msgSender;
    public GameMenager _gameMenager;

    private bool isRunning = true;
    private string recivedText = "";

    private const string IP_O = "192.168.0.192";
    private const int PLAYER_DATA_PORT = 9002;
    private const int GENERAL_PORT = 9898;

    void Start()
    {
        StartUdpListener(4001);
        StartTcpListener(GENERAL_PORT);
        StartPlayerDataListener(PLAYER_DATA_PORT);
    }

    void Update()
    {
        msgReciver.text = recivedText;
    }

    public void SendClicked()
    {
        tcpSender();
    }

    private async void tcpSender()
    {
        try
        {
            using (TcpClient tcpClient = new TcpClient(IP_O, GENERAL_PORT))
            using (NetworkStream stream = tcpClient.GetStream())
            {
                byte[] buffer = Encoding.ASCII.GetBytes(msgSender.text + "\r\n");
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError($"{ex.Message}");
        }
    }

    public async Task SendPlayerData(PlayerData data)
    {
        try
        {
            using (TcpClient playerSender = new TcpClient(IP_O, PLAYER_DATA_PORT))
            using (NetworkStream stream = playerSender.GetStream())
            {
                string json = JsonUtility.ToJson(data);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError($"{ex.Message}");
        }
    }

    private async Task StartUdpListener(int port)
    {
        UdpClient udp = new UdpClient(port);

        while (isRunning)
        {
            try
            {
                UdpReceiveResult res = await udp.ReceiveAsync();
                recivedText = Encoding.ASCII.GetString(res.Buffer);
            }
            catch (SocketException ex)
            {
                Debug.LogError($"{ex.Message}");
            }
        }

        udp.Close();
    }

    private async Task StartTcpListener(int port)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();

        while (isRunning)
        {
            try
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        recivedText = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.Message}");
            }
        }

        tcpListener.Stop();
    }

    private async Task StartPlayerDataListener(int port)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();

        while (isRunning)
        {
            try
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        PlayerData pd = JsonUtility.FromJson<PlayerData>(json);

                        var exisiting = _gameMenager.playersData.FirstOrDefault(a => a.Id == pd.Id);
                        if (exisiting != null)
                        {
                            exisiting.X = pd.X;
                            exisiting.Y = pd.Y;
                            exisiting.Z = pd.Z;
                        }
                        else
                        {
                            _gameMenager.playersData.Add(pd);
                            _gameMenager.SpawnPlayer(pd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.Message}"); 
            }
        }

        tcpListener.Stop();
    }

    void OnApplicationQuit()
    {
        isRunning = false;
    }
}
