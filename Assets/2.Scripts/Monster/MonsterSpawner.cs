using System;
using Cysharp.Threading.Tasks;
using Game.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    public Transform target;
    public GameObject[] spawnPosition;      // 0 : 가장 위 /  1 : 중간 / 2 : 가장 밑

    private PoolContainer _poolContainer;

    private const int MONSTER_MAX_COUNT = 10;
    
    public async void Init(PoolContainer poolContainer)
    {
        _poolContainer = poolContainer;

        await CreateMonster();
    }

    private async UniTask CreateMonster()
    {
        try
        {
            for (int i = 0; i < MONSTER_MAX_COUNT; i++)
            {
                var delay = Random.Range(200, 2000);
                await UniTask.Delay(delay);
                var randomPos = Random.Range(0, spawnPosition.Length);
                var controller = await _poolContainer.Zombie.Rent(spawnPosition[randomPos].transform);
                controller.Init(target);
            }
        }
        catch
        {
            // ignored
        }
    }

}
