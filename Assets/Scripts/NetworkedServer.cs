using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.UI;

public class NetworkedServer : MonoBehaviour
{
    private AccountManager accountManager;
    private GameRoomManager gameRoomManager;
    private TicTacToeManager ticTacToeManager;

    private int maxConnections = 1000;
    private int reliableChannelID;
    private int unreliableChannelID;
    private int hostID;
    private int socketPort = 5491;
    private string msg;
    private int recConnectionID;
   
    void Start()
    {
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
        gameRoomManager = GameObject.Find("GameRoomManager").GetComponent<GameRoomManager>();
        ticTacToeManager = GameObject.Find("TicTacToeManager").GetComponent<TicTacToeManager>();
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null);
    }

    void Update()
    {
        int recHostID;
       
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error = 0;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection, " + recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                ProcessReceivedMsg(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnection, " + recConnectionID);
                gameRoomManager.LeaveRoom(recConnectionID);
                break;
        }
    }

    public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessReceivedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        if (msg.StartsWith(Signifiers.RegisterAccountSignifier.ToString()))
        {
            accountManager.CreateNewAccountCredentials(msg, id);
        }
        
        if (msg.StartsWith(Signifiers.LoginAccountSignifier.ToString()))
        {
            accountManager.CheckLoginCredentials(msg, id);
        }
        
        if (msg.StartsWith(Signifiers.CreateRoomSignifier.ToString()))
        {
            gameRoomManager.CreateNewRoom(msg, id);
        }
        
        if (msg.StartsWith(Signifiers.LeaveRoomSignifier.ToString()))
        {
            gameRoomManager.LeaveRoom(id);
        }
        
        if (msg.StartsWith(Signifiers.GamePlaySignifier.ToString()))
        {
            ticTacToeManager.ProcessPlayerMove(msg, id);
        }
    }
}