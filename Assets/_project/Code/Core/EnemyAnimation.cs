using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator animator;

    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");

    public void PlayWalkingAnim()
    {
        animator.SetBool(IsWalk, true);
    }
    public void StopWalkingAnim()
    {
        animator.SetBool(IsWalk, false);
    }

    public void DeadAnimation()
    {
        animator.SetTrigger(IsDead);
    }
}
