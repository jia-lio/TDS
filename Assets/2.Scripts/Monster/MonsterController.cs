using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Monster
{
    public enum EState
    {
        None,
        Attack,
        Die,
        Run,
    }
    
    public class MonsterController : MonoBehaviour
    {
        public Rigidbody2D rigidbody;
        public BoxCollider2D collider;
        public SortingGroup groupLayer;
        
        private Transform _target;
        private EState _state = EState.None;

        private int _layer;
        private float _jumpCoolTime = 1f;
        private float _lastJumpTime = -1f;
        private float _jumpTargetY;

        private bool _isJump;
        
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
            if (_isJump)
            {
                var currentPos = transform.position;
                var targetPos = new Vector3(currentPos.x, _jumpTargetY, currentPos.z);

                transform.position = Vector3.MoveTowards(currentPos, targetPos, 2 * Time.deltaTime);

                if (Mathf.Abs(transform.position.y - _jumpTargetY) < 0.01f)
                {
                    _isJump = false;
                }
                
                return;
            }

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

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (Time.time - _lastJumpTime < _jumpCoolTime)
                return;
            
            if (collision.gameObject.CompareTag("Monster"))
            {
                var other = collision.gameObject.GetComponent<MonsterController>();
                if (other != null && other._layer == _layer)
                {
                    var myX = transform.position.x;
                    var otherX = other.transform.position.x;
                    var isCover = myX > otherX;

                    if (isCover == false)
                        return;

                    var targetY = _target.position.y - 2f;
                    if (transform.position.y + 1.05f > targetY)
                        return;

                    if (IsLineHitsMonster(Vector2.left * 0.5f) && 
                        IsLineHitsMonster(Vector2.up * 0.75f) == false && 
                        IsLineHitsMonster(new Vector2(-0.5f, 1)) == false)
                    {
                        _jumpTargetY = transform.position.y + 1.05f;
                        _isJump = true;
                        _lastJumpTime = Time.time;
                    }
                }
            }
        }

        private bool IsLineHitsMonster(Vector2 direction)
        {
            var origin = (Vector2)collider.bounds.center;
            var hits = Physics2D.LinecastAll(origin, origin + direction);

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    var hitMonster = hit.collider.GetComponent<MonsterController>();
                    if (hitMonster != null && hitMonster.gameObject.layer == gameObject.layer)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}