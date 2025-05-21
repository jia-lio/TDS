using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

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
        private Transform _target;
        private EState _state = EState.None;

        public void Init(Transform target)
        {
            _target = target;
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
            float dirX = _target.position.x - transform.position.x;
            Vector3 dir = new Vector3(dirX, 0f, 0f).normalized;
            transform.position += dir * (2f * Time.fixedDeltaTime);
        }

        private void OnStateFixedUpdate()
        {
            switch (_state)
            {
                case EState.Run:
                    Run();
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