using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Game.Monster
{
    public enum EState
    {
        None,
        Attack,
        Die,
        Run,
        Stop,
    }
    
    public class MonsterController : MonoBehaviour
    {
        public Animator ani;
        public Rigidbody2D rigidbody;
        public CircleCollider2D collider;
        public SortingGroup groupLayer;

        public Action OnReturn;
        
        private Transform _target;
        private EState _state = EState.None;

        private int _layer;
        private int _monsterSpeed = 2;
        private int _hp = 2;
        private float _jumpCoolTime;
        private float _lastJumpTime = -1f;

        private bool _isJump;
        private bool _isWait;
        private bool _isAttackWait;
        
        public void Init(Transform target, int layer)
        {
            _target = target;
            _layer = layer;

            _jumpCoolTime = Random.Range(3f, 10f);
            
            SetLayer();
        }

        private void SetLayer()
        {
            gameObject.layer = _layer + 6;
            groupLayer.sortingOrder = _layer;
        }

        private void SetState()
        {
            if(_state == EState.Stop)
                return;
            
            var distanceX = Mathf.Abs(_target.position.x - transform.position.x);
            if (distanceX < 1.6f)
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
            if (_target == null) 
                return;

            _isAttackWait = false;

            // 공중
            if (IsLineHitGround(Vector2.down * 0.75f, _layer) == false &&
                IsLineHitsMonster(Vector2.down * 0.75f, out _) == false)
                return;
            
            SetAnimation("IsAttacking", false);
            rigidbody.velocity = new Vector2( -_monsterSpeed, rigidbody.velocity.y);
        }

        private void Attack()
        {
            if(_isAttackWait || _state == EState.Stop)
                return;
            
            SetAnimation("IsAttacking");
            rigidbody.velocity = new Vector2( -_monsterSpeed, rigidbody.velocity.y);
        }
        
        private void OnStateFixedUpdate()
        {
            _isWait = true;
            
            if (_isJump)
            {
                rigidbody.velocity = new Vector2(-1f, 7f);
                _isJump = false;
                HitMonsterBackMoving().Forget();
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

            _isWait = false;
        }

        public void OnAttack()
        {
            //Debug.Log("데미지");
        }

        private async UniTask HitMonsterBackMoving()
        {
            await UniTask.Delay(600);
                
            if (IsLineHitsMonster(Vector2.down * 0.5f, out var hitMonster))
            {
                if (hitMonster != null)
                {
                    hitMonster._state = EState.Stop;
                    hitMonster.rigidbody.AddForce(Vector2.right * 7f, ForceMode2D.Impulse);
                        
                    await UniTask.Delay(300);
                    hitMonster._state = EState.Run;
                }
            }
        }

        private void FixedUpdate()
        {
            if(_isWait)
                return;

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
                    
                    if (IsLineHitsMonster(Vector2.left * 0.5f, out _) && 
                        IsLineHitsMonster(Vector2.up, out _) == false && 
                        IsLineHitsMonster(new Vector2(-0.5f, 0.75f), out _) == false)
                    {
                        _isJump = true;
                        _lastJumpTime = Time.time;
                    }
                }
            }
        }

        private bool IsLineHitsMonster(Vector2 direction, out MonsterController hitMonster)
        {
            var origin = (Vector2)transform.position + collider.offset;;
            var hits = Physics2D.LinecastAll(origin, origin + direction);

            hitMonster = null;

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    hitMonster = hit.collider.GetComponent<MonsterController>();
                    if (hitMonster != null && hitMonster.gameObject.layer == gameObject.layer)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        private bool IsLineHitGround(Vector2 direction, int layer)
        {
            var origin = (Vector2)transform.position + collider.offset;;
            var hits = Physics2D.LinecastAll(origin, origin + direction);
            var groundLayer = layer + 9;
            
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    if (hit.transform.gameObject.layer == groundLayer)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        private void SetAnimation(string aniName, bool isValue = true)
        {
            ani.SetBool(aniName, isValue);
        }

        public void Hit(int damage)
        {
            _hp -= damage;

            if (IsDie())
            {
                Die().Forget();
            }
        }

        private async UniTask Die()
        {
            SetAnimation("IsDead");
            
            await UniTask.Delay(1000);
            OnReturn?.Invoke();
        }

        private bool IsDie()
        {
            if (_hp <= 0)
                return true;
            
            return false;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var start = (Vector2)transform.position + collider.offset;
            
            Gizmos.DrawLine(start, start + Vector2.left * 0.5f);
            Gizmos.DrawLine(start, start + Vector2.up);
            Gizmos.DrawLine(start, start + new Vector2(-0.5f, 0.75f));
            Gizmos.DrawLine(start, start + Vector2.down * 0.5f);
        }
    }
}