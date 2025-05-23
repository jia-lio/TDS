using Cysharp.Threading.Tasks;
using Game.Pool;
using UnityEngine;

namespace Game.Hero
{
    public class HeroController : MonoBehaviour
    {
        private PoolContainer _poolContainer;

        public void Init(PoolContainer poolContainer)
        {
            _poolContainer = poolContainer;
        }

        private async UniTask CreatBullet()
        {
            try
            {
                var controller = await _poolContainer.Bullet.Rent(transform);
                controller.OnReturn = () =>
                {
                    _poolContainer.Bullet.Return(controller);
                };
            }
            catch
            {
                // ignored
            }
        }
    }
}