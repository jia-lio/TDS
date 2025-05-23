using System;
using Game.Monster;
using UnityEngine;

namespace Game.Hero.Bullet
{
    public class BulletController : MonoBehaviour
    {
        public Action OnReturn;
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Monster"))
            {
                var other = collision.gameObject.GetComponent<MonsterController>();
                if (other != null)
                {
                    other.Hit();
                    OnReturn?.Invoke();
                }
            }
        }
    }
}