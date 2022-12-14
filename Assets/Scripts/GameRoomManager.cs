using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class GameRoom
{
    public LinkedList<int> playerConnectionIDs;
    public string name;

    public GameRoom()
    {
        playerConnectionIDs = new LinkedList<int>();
    }
}

public class GameRoomManager : MonoBehaviour
{
    private NetworkedServer server;
    private LinkedList<GameRoom> gameRooms;
    private TicTacToeManager ticTacToeManager;

    private void Start()
    {
        server = GameObject.Find("NetworkServer").GetComponent<NetworkedServer>();
        ticTacToeManager = GameObject.Find("TicTacToeManager").GetComponent<TicTacToeManager>();
        gameRooms = new LinkedList<GameRoom>();
    }

    private bool CheckForExistingRoom(string roomName)
    {
        foreach (GameRoom room in gameRooms)
        {
            if (room.name == roomName )
            {
                Debug.Log("Room already exists");
                return true;
            }
        }
        Debug.Log("Creating room");
        return false;
    }

    public void CreateNewRoom(string receivedMessage, int connectionID)
    {
        string[] roomName;
        roomName = receivedMessage.Split(",");
        if (!CheckForExistingRoom(roomName[1]))
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.name = roomName[1];
            gameRoom.playerConnectionIDs.AddLast(connectionID);
            ticTacToeManager.players.Add(connectionID);
            Debug.Log("Room created!");
            Debug.Log($"Number of players in {gameRoom.name}: {gameRoom.playerConnectionIDs.Count}");
            gameRooms.AddLast(gameRoom);
            server.SendMessageToClient(Signifiers.CreatedRoomSignifier.ToString(), connectionID); ticTacToeManager.CheckForPlayers();
            Debug.Log($"Number of active GameRooms: {gameRooms.Count}.");
        }
        else
        {
            foreach (GameRoom room in gameRooms)
            {
                if (room.name == roomName[1])
                {
                    room.playerConnectionIDs.AddLast(connectionID);
                    ticTacToeManager.players.Add(connectionID);
                    server.SendMessageToClient(Signifiers.CreatedRoomSignifier.ToString(), connectionID);
                    ticTacToeManager.CheckForPlayers();
                    Debug.Log($"Player added to {room.name}");
                    Debug.Log($"Number of players in {room.name}: {room.playerConnectionIDs.Count}");
                    break;
                }
            }
        }
    }

    public void LeaveRoom(int connectionID)
    {
        GameRoom roomDisconnectedFrom = null;
        foreach (GameRoom room in gameRooms)
        {
            foreach (int player in room.playerConnectionIDs)  
            {
                if (connectionID == player)
                {
                    roomDisconnectedFrom = room;
                }
            }
        }

        if (roomDisconnectedFrom != null)
        {
            roomDisconnectedFrom.playerConnectionIDs.Remove(connectionID);
            Debug.Log($"Number of player remaining in room: {roomDisconnectedFrom.playerConnectionIDs.Count}");
            if (roomDisconnectedFrom.playerConnectionIDs.Count <= 0)
            {
                gameRooms.Remove(roomDisconnectedFrom);
                Debug.Log($"Number of rooms remaining: {gameRooms.Count}");
            }
        }
    }
}
