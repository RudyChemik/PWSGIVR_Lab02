using System.Collections.Generic;
using UnityEngine;

public class GameMenager : MonoBehaviour
{
    public List<PlayerData> playersData;
    public GameObject enemy;
    void Start()
    {
        playersData = new List<PlayerData>();
    }

    void Update()
    {
        GeneratePlayers();
    }

    private void GeneratePlayers()
    {
        foreach (PlayerData player in playersData)
        {
            Vector3 position = new Vector3(player.X, player.Y, player.Z);
            Instantiate(enemy, position, Quaternion.identity);
        }
    }
}
