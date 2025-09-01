using UnityEngine;
using Photon.Pun;

public class ServerManager : MonoBehaviourPunCallbacks
{
    [Header("Prefabs y Spawn")]
    public GameObject player;
    public Transform spawnPoint;

    private void Start()
    {
        Debug.Log($"Connecting as '{PhotonNetwork.NickName}'...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
        Debug.Log("Joining/Creating room...");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("We're connected and in a room!");

        if (player == null)
        {
            Debug.LogError("Player prefab no asignado.");
            return;
        }
        if (player.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("El Player prefab necesita un PhotonView.");
            return;
        }

        PhotonNetwork.Instantiate(
            player.name,
            spawnPoint ? spawnPoint.position : Vector3.zero,
            Quaternion.identity
        );
    }
}
