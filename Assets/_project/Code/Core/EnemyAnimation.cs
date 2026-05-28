using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator _animator;

    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void PlayWalkingAnim()
    {
        _animator.SetBool(IsWalk, true);
    }
    public void StopWalkingAnim()
    {
        _animator.SetBool(IsWalk, false);
    }

    public void DeadAnimation()
    {
        _animator.SetTrigger(IsDead);
    }
}
