using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Monster;
using Game.Pool;
using UnityEngine;

namespace Game.Hero
{
    public class HeroController : MonoBehaviour
    {
        public SpriteRenderer gun;
        
        private PoolContainer _poolContainer;

        private MonsterController _targetMonster;

        private bool _isAttacking;

        public void Init(PoolContainer poolContainer)
        {
            _poolContainer = poolContainer;
        }

        private async UniTask CreatBullet()
        {
            try
            {
                var controller = await _poolContainer.Bullet.Rent(transform);
                controller.Fire(_targetMonster.transform.position);
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

        private async UniTaskVoid AttackLoop()
        {
            if(_targetMonster == null)
                return;
            
            _isAttacking = true;

            while (true)
            {
                var angle = Mathf.Atan2(_targetMonster.transform.position.y, _targetMonster.transform.position.x) * Mathf.Rad2Deg;
                gun.transform.DORotate(new Vector3(0, 0, angle), 0.1f);
                await CreatBullet();
                await UniTask.Delay(5000);
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            if (_targetMonster != null)
                return;

            var target = other.transform.gameObject.GetComponent<MonsterController>();
            if (target != null)
            {
                _targetMonster = target;

                if (_isAttacking == false)
                {
                    AttackLoop().Forget();
                }
            }
        }
    }
}