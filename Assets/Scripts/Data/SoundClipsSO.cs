using UnityEngine;

[CreateAssetMenu(fileName = "SoundClipsSO", menuName = "SoundClipsSO/Data", order = 1)]

public class SoundClipsSO : ScriptableObject
{
    public AudioClip music;

    public AudioClip monsterIdle;
    public AudioClip monsterHit;
    public AudioClip monsterAttack;
    public AudioClip monsterDead;
    public AudioClip monsterStep;

    public AudioClip[] footSteps;

    public AudioClip spell;
    public AudioClip onHit;
    public AudioClip onPressurePlate;
    public AudioClip onRingCatch;
}
