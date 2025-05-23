using Cysharp.Threading.Tasks;
using Game.Hero.Bullet;
using Game.Monster;
using UnityEngine;

namespace Game.Pool
{
    public class PoolContainer : MonoBehaviour
    {
        public AddressablePool<MonsterController> Zombie { get; private set; }
        public AddressablePool<BulletController> Bullet { get; private set; }

        public async UniTask LoadAsync()
        {
            Zombie = CreatePool<MonsterController>("Assets/3.Prefabs/Monster/ZombieMelee.prefab", 50);
            Bullet = CreatePool<BulletController>("Assets/3.Prefabs/Bullet/Bullet.prefab", 10);
        }

        private AddressablePool<T> CreatePool<T>(string key, int capacity = 0) where T : Component
        {
            var obj = new GameObject(key) { transform = { parent = transform } };
            var pool = new AddressablePool<T>(key, obj.transform);
            
            pool.Init(capacity);

            return pool;
        }
    }
}