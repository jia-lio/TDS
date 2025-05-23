using System;
using Cysharp.Threading.Tasks;
using Game.Monster;
using UnityEngine;

namespace Game.Hero.Bullet
{
    public class BulletController : MonoBehaviour
    {
        public Action OnReturn;

        private bool _isFiring;
        private Vector2 _direction;

        private float _speed = 20f;
        private int _damage = 2;
        
        public void Fire(Vector2 direction)
        {
            _direction = (direction - (Vector2)transform.position).normalized;
            _isFiring = true;
            
            AutoReturnAsync().Forget();
        }

        public void Update()
        {
            if (_isFiring)
            {
                transform.position += (Vector3)(_direction * _speed * Time.deltaTime);
            }
        }

        private async UniTaskVoid AutoReturnAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            OnReturn?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Monster"))
            {
                var target = other.gameObject.GetComponent<MonsterController>();
                if (target != null)
                {
                    target.Hit(_damage);
                    OnReturn?.Invoke();
                }
            }
        }
    }
}