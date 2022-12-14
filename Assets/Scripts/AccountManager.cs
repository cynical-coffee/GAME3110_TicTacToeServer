using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    private NetworkedServer server;

    // Signifiers
    private const string usernameSignifier = "0";
    private const string passwordSignifier = "1";

    private void Start()
    {
        server = GameObject.Find("NetworkServer").GetComponent<NetworkedServer>();
    }

    private bool CheckForExistingAccount(string username)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Accounts");
        string[] userFileNames = Directory.GetFiles(path, "*.txt");

        for (int i = 0; i < userFileNames.Length; i++)
        {
            userFileNames[i] = Path.GetFileNameWithoutExtension(userFileNames[i]);
        }

        foreach (string fileName in userFileNames)
        {
            if (fileName == username)
            {
                Debug.Log("Account name in use!");
                return true;
            }
        }
        Debug.Log("Account name NOT in use!");
        return false;
    }

    public void CreateNewAccountCredentials(string receivedMessage, int connectionID)
    {
        string[] sAccountUserPass;
        sAccountUserPass = receivedMessage.Split(",");
        if (!CheckForExistingAccount(sAccountUserPass[1]))
        {
            using (System.IO.StreamWriter mStreamWriter = new StreamWriter($@"Accounts\{sAccountUserPass[1]}.txt"))
            {
                mStreamWriter.WriteLine(usernameSignifier.ToString() + "," + sAccountUserPass[1]);
                mStreamWriter.WriteLine(passwordSignifier.ToString() + "," + sAccountUserPass[2]);
            }
        }
        else
        {
            server.SendMessageToClient("Account already exists!", connectionID);
        }
    }

    public void CheckLoginCredentials(string receivedMessage, int connectionID)
    {
        string[] sAccountUserPass;
        sAccountUserPass = receivedMessage.Split(",");
        if (CheckForExistingAccount(sAccountUserPass[1]))
        {
            using (System.IO.StreamReader mStreamReader = new StreamReader($@"Accounts\{sAccountUserPass[1]}.txt"))
            {
                string mCurrentLine = "";
                string[] sPassword;
                while ((mCurrentLine = mStreamReader.ReadLine()) != null)
                {
                    if (mCurrentLine.StartsWith(passwordSignifier))
                    {
                        sPassword = mCurrentLine.Split(",");
                        if (sPassword[1] == sAccountUserPass[2])
                        {
                            Debug.Log("User Logged In!");
                            server.SendMessageToClient(Signifiers.LoggedInSignifier.ToString(), connectionID);
                        }
                    }
                }
            }
        }
    }

}
