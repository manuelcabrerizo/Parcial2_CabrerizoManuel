using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private ParticleSystem aimParticleSystem;
    [SerializeField] private ParticleSystem spellParticleSystem;
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material controlMaterial;
    [SerializeField] private Material attackMaterial;
    public ParticleSystem AimParticleSystem => aimParticleSystem;
    public ParticleSystem SpellParticleSystem => spellParticleSystem;
    public Material IdleMaterial => idleMaterial;
    public Material ControlMaterial => controlMaterial;
    public Material AttackMaterial => attackMaterial;
}
