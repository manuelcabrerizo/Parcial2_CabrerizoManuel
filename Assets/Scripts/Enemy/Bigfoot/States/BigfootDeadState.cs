using System;
using UnityEngine;

public class BigfootDeadState : State<Bigfoot>
{
    private float time = 0;

    public BigfootDeadState(Bigfoot owner, Func<bool> enterCondition) 
        : base(owner, enterCondition) { }

    public override void OnEnter()
    {
        owner.Animator.SetBool("IsDead", true);
        AudioManager.onPlayClip3D?.Invoke(owner.Clips.monsterDead, owner.transform.position, 1, 20);
    }

    public override void OnUpdate()
    {
        if (!owner.IsDeadAnimationEnd)
        {
            owner.SkinnedMeshRenderer.material.SetColor("_Tint", Color.Lerp(Color.black, Color.red, Mathf.Sin(time * 40)));
            time += Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        owner.Animator.SetBool("IsDead", false);
    }
}
