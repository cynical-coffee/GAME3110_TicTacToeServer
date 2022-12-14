using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class TicTacToeManager : MonoBehaviour
{
    private NetworkedServer server;
   // private GameRoomManager gameRoom;
    public List<int> players = new List<int>(2);
    private int player1;
    private int player2;

    private void Start()
    {
        server = GameObject.Find("NetworkServer").GetComponent<NetworkedServer>();
       // gameRoom = GameObject.Find("GameRoomManager").GetComponent<GameRoomManager>();
    }

    private int CoinFlip()
    {
        int coinFlip = Random.Range(0, 2);
        return coinFlip;
    }

    public void CheckForPlayers()
    {
        if (players.Count == 2)
        {
            player1 = CoinFlip();
            if (player1 == 0)
            {
                player2 = 1;
            }
            else
            {
                player2 = 0;
            }
            server.SendMessageToClient(Signifiers.StartGameSignifier.ToString() + "X", players[player1]);
        }
    }

    public void ProcessPlayerMove(string receivedMessage, int connectionID)
    {
        string[] playerMove;
        playerMove = receivedMessage.Split(",");

        if (connectionID == players[player1])
        {
            server.SendMessageToClient(Signifiers.GamePlaySignifier.ToString() + "," + "O" + "," + playerMove[2] , players[player2]);
            Debug.Log($"player 2 Presses {playerMove[2]} and plays O");
        }
        else
        {
            server.SendMessageToClient(Signifiers.GamePlaySignifier.ToString() + "," + "X" + "," + playerMove[2], players[player1]);
            Debug.Log($"player 1 Presses {playerMove[2]} and plays X");
        }
    }
}
