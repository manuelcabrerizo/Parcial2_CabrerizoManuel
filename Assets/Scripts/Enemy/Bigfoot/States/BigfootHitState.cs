using System;
using UnityEngine;

public class BigfootHitState : State<Bigfoot>
{
    private float time = 0;

    public BigfootHitState(Bigfoot owner, Func<bool> enterCondition) 
        : base(owner, enterCondition) { }

    public override void OnEnter()
    {
        time = 0;
        owner.Animator.SetTrigger("Hit");
        AudioManager.onPlayClip3D?.Invoke(owner.Clips.monsterHit, owner.transform.position, owner.data.minSoundRadio, owner.data.maxSoundRadio);
    }

    public override void OnExit()
    {
        owner.SkinnedMeshRenderer.material.SetColor("_Tint", Color.black);
    }

    public override void OnUpdate()
    {
        owner.SkinnedMeshRenderer.material.SetColor("_Tint", Color.Lerp(Color.black, Color.red, Mathf.Sin(time * 40)));
        if (time >= 4.0f)
        {
            owner.SetIsHit(false);
        }
        time += Time.deltaTime;
    }
}
