﻿using Boo.Lang;
using UnityEngine;
using UnityEngine.AI;

public class NetworkManager : MonoBehaviour
{
    const string VERSION = "v0.0.1";
    public string roomName = "Dungeon";

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.sendRate = 30;
        PhotonNetwork.sendRateOnSerialize = 30;
    }

    private void Awake()
    {
        //PhotonNetwork.automaticallySyncScene = true;
    }

    // Update is called once per frame
    void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    void OnCreatedRoom()
    {
        //PhotonNetwork.LoadLevel("Projekt");

        //SpawnSkeleton();
        //SpawnCube();
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined room");

        // All available players with spawn order
        List<string> availPlayers = new List<string>();
        availPlayers.Add("Warrior");
        availPlayers.Add("Wizard");
        availPlayers.Add("Archer");


        foreach (PhotonPlayer otherPlayer in PhotonNetwork.otherPlayers)
        {
            availPlayers = availPlayers.Remove(otherPlayer.NickName);
            Debug.Log("Found other player: " + otherPlayer.NickName);
        }
        if (availPlayers.Count > 0)
        {
            PhotonNetwork.player.NickName = availPlayers[0];
        }
        else
        {
            Debug.Log("No more players left");
            return;
        }
        Debug.Log("Let begin as " + PhotonNetwork.player.NickName);

        // Scan all player spawn points
        var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (var spawnPoint in spawnPoints)
        {
            var spawnParams = spawnPoint.GetComponent<SpawnPointParameters>();

            // Spawn next player if available
            if (spawnParams.prefab.name == PhotonNetwork.player.NickName)
            {
                // Instantiate player
                var currentPlayer = (GameObject)PhotonNetwork.InstantiateGameObject(spawnParams.prefab.name, spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
                currentPlayer.GetComponent<WSAD>().enabled = true;
                currentPlayer.GetComponent<CameraMove>().enabled = true;
                currentPlayer.transform.FindChild("Camera").gameObject.SetActive(true);
                currentPlayer.GetComponent<CharacterHealth>().enabled = true;
                currentPlayer.GetComponent<PlayerHealthBar>().enabled = true;
                currentPlayer.GetComponent<PhotonView>().RequestOwnership();

                // Enable character-specific scripts
                if (spawnParams.prefab.name == "Wizard")
                {
                    currentPlayer.GetComponent<WizardHolding>().enabled = true;
                }
                if (spawnParams.prefab.name == "Warrior")
                {
                    currentPlayer.GetComponent<Attack>().enabled = true;
                }
                if (spawnParams.prefab.name == "Archer")
                {
                    currentPlayer.GetComponent<BowAttack>().enabled = true;
                }
            }
        }
    }

    private void Update()
    {
    }
}
