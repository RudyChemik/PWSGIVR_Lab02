using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private GameObject player;
    private int id;

    public CommunicationController _communicationController;

    void Start()
    {
        player = this.gameObject;
        id = 1;
    }

    void Update()
    {
        Vector3 movement = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        player.transform.Translate(movement * 5f * Time.deltaTime);
        _communicationController.SendPlayerData(9191, GetPlayerData());
    }

    private PlayerData GetPlayerData()
    {
        PlayerData playerData = new PlayerData()
        {
            Id = id,
            X = player.transform.position.x,
            Y = player.transform.position.y,
            Z = player.transform.position.z,
        };

        return playerData;
    }
}
