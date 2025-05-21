using System;
using DG.Tweening;
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
        public CircleCollider2D collider;
        public SortingGroup groupLayer;
        
        private Transform _target;
        private EState _state = EState.None;

        private int _layer;
        private float _jumpCoolTime = 1f;
        private float _lastJumpTime = -1f;
        private float _jumpTargetY;

        private bool _isJump;
        private bool _isBack;
        
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
            if (_target == null) 
                return;

            var dirX = Mathf.Sign(_target.position.x - transform.position.x);
            rigidbody.velocity = new Vector2(dirX * 2f, rigidbody.velocity.y);
        }

        private void Attack()
        {
            
        }
        
        private void OnStateFixedUpdate()
        {
            if (_isJump)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 3f);
                _isJump = false;
                
                if (IsLineHitsMonster(Vector2.left * 0.6f, out MonsterController hitMonster))
                {
                    if (hitMonster != null)
                    {
                        var targetPos = hitMonster.transform.position.x + 1f;
                        hitMonster.transform.DOMoveX(targetPos, 0.75f);
                    }
                }
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

                    if (IsLineHitsMonster(Vector2.left * 0.5f, out _) && 
                        IsLineHitsMonster(Vector2.up * 0.75f, out _) == false && 
                        IsLineHitsMonster(new Vector2(-0.5f, 1), out _) == false)
                    {
                        _jumpTargetY = transform.position.y + 1.05f;
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var start = (Vector2)transform.position + collider.offset;
            var end = (Vector2)start + Vector2.left * 0.6f;
            
            Gizmos.DrawLine(start, end);
            
            
        }
    }
}