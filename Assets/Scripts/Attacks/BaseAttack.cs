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
        [SerializeField] protected float p_prepareTime = 0f;
        [SerializeField] protected float p_cooldown = 1f;
        [SerializeField] protected string p_animationTrigger = "Attack";

        protected Animator p_animator;
        protected Coroutine p_cooldownCoroutine;

        public bool CanAttack { get; protected set; } = true;
        public AttackTypes AttackType => p_attackType;
        public float CurrentAnimationTime => p_animator.GetCurrentAnimatorStateInfo(0).length;
        public float Cooldown => p_cooldown;
        public float PrepareTime => p_prepareTime;

        public event Action OnAttackInterrupted;
        public event Action<bool> OnAttackStarted;
        public event Action<bool> OnAttacked;
        public event Action<bool> OnAttackFinished;

        protected virtual void OnEnable() => p_animator = GetComponentInParent<Animator>();

        /// <summary>
        /// Starts the attack.
        /// </summary>
        [Server]
        public virtual void StartAttack()
        {
            if (!CanAttack)
                return;

            CanAttack = false;
            OnAttackStarted?.Invoke(true);
            OnAttackFinished?.Invoke(false);
            StartCoroutine(AttackPreparation());
        }

        /// <summary>
        /// Stops the attack and resets the attack state.
        /// </summary>
        public virtual void StopAttack()
        {
            StopAllCoroutines();
            CanAttack = true;
            OnAttackInterrupted?.Invoke();
            OnAttackStarted?.Invoke(false);
            OnAttacked?.Invoke(false);
            OnAttackFinished?.Invoke(false);
        }

        protected virtual void PrepareAttack()
        {
            p_animator.SetTrigger(p_animationTrigger);
            BroadCastAnimation(p_animationTrigger);
        }

        protected IEnumerator AttackPreparation()
        {
            PrepareAttack();
            yield return new WaitForSeconds(p_prepareTime);
            yield return new WaitForSeconds(CurrentAnimationTime);
            Attack();
            OnAttacked?.Invoke(true);

            if (!isActiveAndEnabled)
                yield break;
            p_cooldownCoroutine = StartCoroutine(AttackCooldown());
        }

        protected abstract void Attack();

        protected IEnumerator AttackCooldown()
        {
            yield return new WaitForSeconds(p_cooldown);
            CanAttack = true;
            BroadCastCanAttack(true);
            OnAttackStarted?.Invoke(false);
            OnAttacked?.Invoke(false);
            OnAttackFinished?.Invoke(true);
        }

        protected void IncreaseCooldown(float amount) => p_cooldown += amount;

        [ObserversRpc]
        private void BroadCastCanAttack(bool canAttack) => CanAttack = canAttack;

        [ObserversRpc]
        private void BroadCastAnimation(string trigger) => p_animator.SetTrigger(trigger);
    }
}
