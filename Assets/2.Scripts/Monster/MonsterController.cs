using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Monster
{
    public enum EState
    {
        None,
        Attack,
        Die,
        Run
    }
    
    public class MonsterController : MonoBehaviour
    {
        public Rigidbody2D rigidbody;
        public SortingGroup groupLayer;
        
        private Transform _target;
        private EState _state = EState.None;

        private int _layer;
        
        public void Init(Transform target, int layer)
        {
            _target = target;
            _layer = layer;
            
            SetLayer();
        }

        private void SetLayer()
        {
            gameObject.layer = _layer + 6;
            groupLayer.sortingOrder = _layer;
        }

        private void SetState()
        {
            var distanceX = Mathf.Abs(_target.position.x - transform.position.x);
            if (distanceX < 1.5f)
            {
                _state = EState.Attack;
            }
            else
            {
                _state = EState.Run;
            }
        }

        private void Run()
        {
            if (_target == null) return;

            float dirX = Mathf.Sign(_target.position.x - transform.position.x);
            rigidbody.velocity = new Vector2(dirX * 2f, rigidbody.velocity.y);
        }

        private void Attack()
        {
            
        }

        private void OnStateFixedUpdate()
        {
            switch (_state)
            {
                case EState.Run:
                    Run();
                    break;
                case EState.Attack:
                    Attack();
                    break;
            }
        }
        
        private void FixedUpdate()
        {
            SetState();
            OnStateFixedUpdate();
        }
    }
}