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
        public float Cooldown => p_cooldown;
        public float PrepareTime => p_prepareTime;

        public event Action OnAttackStarted;
        public event Action<bool> OnAttacked;
        public event Action<bool> OnAttackFinished;

        protected virtual void OnEnable()
        {
            OnAttackStarted += StartAttack;
            p_animator = GetComponentInParent<Animator>();
        }

        public virtual void StartAttack()
        {
            if (!CanAttack)
                return;

            CanAttack = false;
            StartCoroutine(AttackPreparation());
        }

        protected virtual void PrepareAttack()
        {
            p_animator.SetTrigger(p_animationTrigger);
            BroadCastAnimation(p_animationTrigger);
        }

        protected IEnumerator AttackPreparation()
        {
            PrepareAttack();
            float animationTime = p_animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(p_prepareTime + animationTime);
            Attack();
            OnAttacked?.Invoke(true);
            OnAttackFinished?.Invoke(false);
            p_cooldownCoroutine = StartCoroutine(AttackCooldown());
        }

        protected abstract void Attack();

        protected IEnumerator AttackCooldown()
        {
            yield return new WaitForSeconds(p_cooldown);
            CanAttack = true;
            BroadCastCanAttack(true);
            OnAttacked?.Invoke(false);
            OnAttackFinished?.Invoke(true);
        }

        protected void IncreaseCooldown(float amount) => p_cooldown += amount;

        [ObserversRpc]
        private void BroadCastCanAttack(bool canAttack) => CanAttack = canAttack;

        [ObserversRpc]
        private void BroadCastAnimation(string trigger) => p_animator.SetTrigger(trigger);

        protected virtual void OnDisable() => OnAttackStarted -= StartAttack;
    }
}
