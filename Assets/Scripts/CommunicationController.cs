using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;
using System.IO;
using UnityEditor.PackageManager;

public class CommunicationController : MonoBehaviour
{
    public Text msgReciver;
    public TMP_InputField msgSender;

    private BackgroundWorker udpBw;
    private BackgroundWorker tcpBw;
    private TcpClient client;
    private NetworkStream stream;

    private string recivedText;

    void Start()
    {
        udpBw = new BackgroundWorker();
        tcpBw = new BackgroundWorker();

        udpBw.DoWork += new DoWorkEventHandler(bw_DoWork_udp);
        tcpBw.DoWork += new DoWorkEventHandler(bw_DoWork_tcp);

        udpBw.RunWorkerAsync();
        tcpBw.RunWorkerAsync();
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
}