using System;
using UnityEngine;

public class BigfootHitState : State<Bigfoot>
{
    private float time = 0;

    public BigfootHitState(Bigfoot owner, Func<bool> enterCondition) 
        : base(owner, enterCondition) { }

    public override void OnEnter()
    {
        Debug.Log("Hit OnEnter");
        time = 0;
        owner.Animator.SetTrigger("Hit");
        AudioManager.onPlayClip3D?.Invoke(owner.Clips.monsterHit, owner.transform.position, 100, 400);
    }

    public override void OnExit()
    {
        owner.SkinnedMeshRenderer.material.SetColor("_Tint", Color.black);
        Debug.Log("Hit OnExit");
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
