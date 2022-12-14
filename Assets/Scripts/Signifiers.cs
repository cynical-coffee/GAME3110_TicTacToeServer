using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Signifiers
{
    public const int RegisterAccountSignifier = 0;
    public const int LoginAccountSignifier = 1;
    public const int LoggedInSignifier = 2;
    public const int CreateRoomSignifier = 3;
    public const int CreatedRoomSignifier = 4;
    public const int LeaveRoomSignifier = 5;
    public const int StartGameSignifier = 6;
    public const int GamePlaySignifier = 7;
    public const int Player1Signifier = 8;
    public const int Player2Signifier = 9;
}



//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;

///*
// * Keyword: out 
//	The out is a keyword that allows the argument of a method to be passed by reference. 
//It must be a variable to do this as any operation on the parameter is made on the argument, 
//however it does not need to be initialized before being passed. 
// */

//public class NetworkedClient : MonoBehaviour
//{
//    private TicTacToeManager ticTacToeManager;

//    private int connectionID;
//    private int maxConnections = 1000;
//    private int reliableChannelID;
//    private int unreliableChannelID;
//    private int hostID;
//    private int socketPort = 5491;
//    private byte error;
//    private bool isConnected = false;
//    private int ourClientID;

//    // Start is called before the first frame update
//    void Start()
//    {
//        ticTacToeManager = GameObject.Find("TicTacToeManager").GetComponent<TicTacToeManager>();
//        Connect();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //if (Input.GetKeyDown(KeyCode.S))
//        //SendMessageToHost("Hello from client");

//        UpdateNetworkConnection();
//    }

//    private void UpdateNetworkConnection()
//    {
//        if (isConnected)
//        {
//            int recHostID;
//            int recConnectionID;
//            int recChannelID;
//            byte[] recBuffer = new byte[1024];
//            int bufferSize = 1024;
//            int dataSize;
//            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

//            switch (recNetworkEvent)
//            {
//                case NetworkEventType.ConnectEvent:
//                    Debug.Log("connected.  " + recConnectionID);
//                    ourClientID = recConnectionID;
//                    break;
//                case NetworkEventType.DataEvent:
//                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
//                    ProcessReceivedMsg(msg, recConnectionID);
//                    break;
//                case NetworkEventType.DisconnectEvent:
//                    isConnected = false;
//                    Debug.Log("disconnected.  " + recConnectionID);
//                    break;
//            }
//        }
//    }

//    private void Connect()
//    {

//        if (!isConnected)
//        {
//            Debug.Log("Attempting to create connection");

//            NetworkTransport.Init();

//            ConnectionConfig config = new ConnectionConfig();
//            reliableChannelID = config.AddChannel(QosType.Reliable);
//            unreliableChannelID = config.AddChannel(QosType.Unreliable);
//            HostTopology topology = new HostTopology(config, maxConnections);
//            hostID = NetworkTransport.AddHost(topology, 0);
//            Debug.Log("Socket open.  Host ID = " + hostID);

//            connectionID = NetworkTransport.Connect(hostID, "192.168.2.188", socketPort, 0, out error); // server is local on network

//            if (error == 0)
//            {
//                isConnected = true;
//                Debug.Log("Connected, id = " + connectionID);
//            }
//        }
//    }

//    public void Disconnect()
//    {
//        NetworkTransport.Disconnect(hostID, connectionID, out error);
//    }

//    public void SendMessageToHost(string msg)
//    {
//        byte[] buffer = Encoding.Unicode.GetBytes(msg);
//        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
//    }

//    private void ProcessReceivedMsg(string msg, int id)
//    {
//        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

//        if (msg.StartsWith(Signifiers.LoggedInSignifier.ToString()))
//        {
//            StateManager.Instance.state = StateManager.GameState.CREATEGAMEROOM;
//        }

//        if (msg.StartsWith(Signifiers.CreatedRoomSignifier.ToString()))
//        {
//            StateManager.Instance.state = StateManager.GameState.GAMEROOM;
//        }

//        if (msg.StartsWith(Signifiers.StartGameSignifier.ToString()))
//        {
//            Debug.Log("Player start");
//            //ticTacToeManager.GetPlayerLetter(msg);
//            ticTacToeManager.state = TicTacToeManager.GameState.PLAYERTURN;
//        }

//        if (msg.StartsWith(Signifiers.GamePlaySignifier.ToString()))
//        {
//            //ticTacToeManager.GetPlayerLetter(msg);
//            Debug.Log("processing Opponent Move");
//            ticTacToeManager.ProcessOpponentMove(msg);
//        }
//    }

//    public bool IsConnected()
//    {
//        return isConnected;
//    }

//    public int ConnectionID()
//    {
//        return connectionID;
//    }
//}