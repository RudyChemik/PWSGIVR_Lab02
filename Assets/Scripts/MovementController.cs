using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private GameObject player;
    private int id;

    private float lastSendTime = 0f;
    private float sendInterval = 2f;

    public CommunicationController _communicationController;

    void Start()
    {
        player = this.gameObject;
        id = 1;
    }


    async void Update()
    {
        Vector3 movement = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        player.transform.Translate(movement * 5f * Time.deltaTime);

        if (Time.time - lastSendTime >= sendInterval)
        {
            lastSendTime = Time.time;
            _ = _communicationController.SendPlayerData(GetPlayerData());
        }
    }


    public PlayerData GetPlayerData()
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
