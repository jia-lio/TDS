using System;
using Game.Monster;
using UnityEngine;

namespace Game.Hero.Bullet
{
    public class BulletController : MonoBehaviour
    {
        public Action OnReturn;

        private bool _isFiring;
        private Vector2 _direction;

        private float _speed = 15f;
        
        public void Fire(Vector2 direction)
        {
            _direction = (direction - (Vector2)transform.position).normalized;
            _isFiring = true;
        }

        public void Update()
        {
            if (_isFiring)
            {
                transform.position += (Vector3)(_direction * _speed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Monster"))
            {
                var target = other.gameObject.GetComponent<MonsterController>();
                if (target != null)
                {
                    target.Hit();
                    OnReturn?.Invoke();
                }
            }
        }
    }
}