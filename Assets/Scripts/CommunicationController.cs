using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;
using System.IO;
using UnityEditor.PackageManager;
using System.Linq;

public class CommunicationController : MonoBehaviour
{
    public Text msgReciver;
    public TMP_InputField msgSender;
    public GameMenager _gameMenager;

    private BackgroundWorker udpBw;
    private BackgroundWorker tcpBw;
    private BackgroundWorker playersBw;

    private TcpClient client;
    private NetworkStream stream;

    private string recivedText;

    void Start()
    {
        udpBw = new BackgroundWorker();
        tcpBw = new BackgroundWorker();
        playersBw = new BackgroundWorker();

        udpBw.DoWork += new DoWorkEventHandler(bw_DoWork_udp);
        tcpBw.DoWork += new DoWorkEventHandler(bw_DoWork_tcp);
        playersBw.DoWork += new DoWorkEventHandler(bw_DoWork_Get_Players_Data);

        udpBw.RunWorkerAsync();
        tcpBw.RunWorkerAsync();
        playersBw.RunWorkerAsync();
    }

    void Update()
    {
        msgReciver.text = recivedText;
    }

    public void SendClicked()
    {
        //udpSender();
        tcpSender();
    }

    private void udpSender()
    {
        byte[] buffer = Encoding.ASCII.GetBytes($"{msgSender.text} \r\n");
        UdpClient udpClient = new UdpClient();

        udpClient.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, 4001));
        udpClient.Close();
    }

    private void tcpSender()
    {
        TcpClient tcpClient = new TcpClient("127.0.0.1", 9898);
        NetworkStream stream = tcpClient.GetStream();
        byte[] buffer = Encoding.ASCII.GetBytes(msgSender.text + "\r\n");
        stream.Write(buffer, 0, buffer.Length);
        stream.Close();
        tcpClient.Close();
    }

    public void SendPlayerData(int port, PlayerData data)
    {
        TcpClient tcpClient = new TcpClient("127.0.0.1", port);
        NetworkStream stream = tcpClient.GetStream();

        //string msg = $"{data.Id}|{data.X},{data.Y},{data.Z}";

        //byte[] buffer = Encoding.ASCII.GetBytes(msg);
        //stream.Write(buffer, 0, buffer.Length);

        string json = JsonUtility.ToJson(data);
        byte[] buffer = Encoding.UTF8.GetBytes(json);

        stream.Write(buffer, 0, buffer.Length);

        stream.Close();
        tcpClient.Close();
    }
    private void bw_DoWork_udp(object sender, DoWorkEventArgs ea)
    {
        UdpClient upd = new UdpClient(4001);
        while (true)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = upd.Receive(ref RemoteIpEndPoint);

            Debug.Log(string.Format("{0}", Encoding.ASCII.GetString(buffer)));

            if (buffer != null)
            {
                recivedText = string.Format("{0}", Encoding.ASCII.GetString(buffer));
                Debug.Log("Recived");
            }
        }
    }

    private void bw_DoWork_tcp(object sender, DoWorkEventArgs ea)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 9898);
        tcpListener.Start();

        while (true)
        {
            client = tcpListener.AcceptTcpClient();
            stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                recivedText = string.Format("{0}", Encoding.ASCII.GetString(buffer));
                Debug.Log("Recived");
            }

            client.Close();
        }
    }

    private void bw_DoWork_Get_Players_Data(object sender, DoWorkEventArgs ea)
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 9292);
        tcpListener.Start();

        while (true)
        {
            client = tcpListener.AcceptTcpClient();
            stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                PlayerData pd = JsonUtility.FromJson<PlayerData>(json);

                var exisiting = _gameMenager.playersData.FirstOrDefault(a=>a.Id == pd.Id);
                if(exisiting != null)
                {
                    exisiting.X = pd.X;
                    exisiting.Y = pd.Y;
                    exisiting.Z = pd.Z;
                }
                else
                {
                    _gameMenager.playersData.Add(pd);
                }

            }

            client.Close();
        }
    }

}