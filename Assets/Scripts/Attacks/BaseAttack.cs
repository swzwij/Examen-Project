using System;
using System.Collections;
using Examen.Attacks;
using FishNet.Object;
using UnityEngine;

namespace Exame.Attacks
{
    public abstract class BaseAttack : NetworkBehaviour
    {
        [Header("Base Attack Settings")]
        [SerializeField] protected AttackTypes p_attackType;
        [SerializeField] protected float p_damage = 10f;
        [SerializeField] protected float p_cooldown = 1f;

        protected Animator p_animator;
        protected Coroutine p_cooldownCoroutine;
        private bool _hasAnimator;

        protected float CurrentAnimationLength => p_animator.GetCurrentAnimatorClipInfo(0).Length;
        public bool CanAttack { get; protected set; } = true;
        public AttackTypes AttackType => p_attackType;
        public float Cooldown => p_cooldown;

        public event Action OnAttack;
        public event Action OnAttacked;

        protected virtual void OnEnable()
        {
            if (TryGetComponent(out p_animator))
                _hasAnimator = true;

            OnAttack += ActivateAttack;
        }

        public virtual void ActivateAttack()
        {
            if (!CanAttack)
                return;

            CanAttack = false;
            Attack();
            OnAttacked?.Invoke();
            p_cooldownCoroutine = StartCoroutine(AttackCooldown());
        }

        protected abstract void Attack();

        protected IEnumerator AttackCooldown()
        {
            if (_hasAnimator)
                yield return new WaitForSeconds(CurrentAnimationLength);
            
            yield return new WaitForSeconds(p_cooldown);
            CanAttack = true;
            BroadCastCanAttack(true);
        }

        protected void IncreaseCooldown(float amount) => p_cooldown += amount;

        [ObserversRpc]
        private void BroadCastCanAttack(bool canAttack)
        {
            CanAttack = canAttack;
        }

        protected virtual void OnDisable() => OnAttack -= ActivateAttack;
    }
}