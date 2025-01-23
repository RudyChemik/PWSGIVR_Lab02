using System.Collections.Generic;
using UnityEngine;

public class GameMenager : MonoBehaviour
{
    public List<PlayerData> playersData;
    public GameObject enemy;
    private Dictionary<int, GameObject> enemies = new Dictionary<int, GameObject>();
    void Start()
    {
        playersData = new List<PlayerData>();
    }

    void Update()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        foreach(var data in playersData)
        {
            if (enemies.ContainsKey(data.Id))
            {
                enemies[data.Id].transform.position = new Vector3(data.X, data.Y, data.Z);
            }
        }
    }

    public void SpawnPlayer(PlayerData player)
    {
        Vector3 position = new Vector3(player.X, player.Y, player.Z);
        GameObject newEnemy =  Instantiate(enemy, position, Quaternion.identity);
        enemies[player.Id] = newEnemy;
    }
}
