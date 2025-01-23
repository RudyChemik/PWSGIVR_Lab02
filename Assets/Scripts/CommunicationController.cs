using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

public class CommunicationController : MonoBehaviour
{
    public Text msgReciver;
    public TMP_InputField msgSender;

    private BackgroundWorker bw;
    private string recivedText;

    void Start()
    {
        bw = new BackgroundWorker();
        bw.DoWork += new DoWorkEventHandler(bw_DoWork);
        bw.RunWorkerAsync();
    }

    void Update()
    {
        msgReciver.text = recivedText;
    }

    public void SendClicked()
    {
        byte[] buffer = Encoding.ASCII.GetBytes($"{msgSender.text} \r\n");
        UdpClient udpClient = new UdpClient();

        udpClient.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, 4001));
        udpClient.Close();
    }

    private void bw_DoWork(object sender, DoWorkEventArgs ea)
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
}