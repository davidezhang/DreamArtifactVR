using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;

public class TCPClientNew : MonoBehaviour
{

    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[8142];

    private void StartClient()
    {
        try
        {
            _clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 8000));
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);

            // Try to reconnect ??  TODO
        }
        Debug.Log("connected");

        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        //Check how much bytes are recieved and call EndRecieve to finalize handshake
        int recieved = _clientSocket.EndReceive(AR);

        if (recieved <= 0)
            return;

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);


        //Processing received data
        Debug.Log("Rx");
        Debug.Log(System.Text.Encoding.ASCII.GetString(recData));



        //Start receiving again
        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    private void SendData()
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes("HELLO");
        Debug.Log(BitConverter.ToString(data));


        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        _clientSocket.SendAsync(socketAsyncData);
        Debug.Log("checkpoint");
    }

    void Start()
    {
        StartClient();
        

        Invoke("SendData", 1);
        //SendData(msg);
    }
}