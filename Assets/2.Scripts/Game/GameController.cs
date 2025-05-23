using Game.Hero;
using Game.Pool;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] 
        private PoolContainer poolContainer;
        [SerializeField] 
        public MonsterSpawner monsterSpawner;
        [SerializeField]
        public HeroController heroController;

        private async void Start()
        {
            await poolContainer.LoadAsync();

            monsterSpawner.Init(poolContainer);
            heroController.Init(poolContainer);
        }
    }
}