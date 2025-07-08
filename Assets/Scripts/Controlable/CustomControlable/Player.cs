using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CustomControlable, IDamagable
{
    public static event Action<int, int> onLifeChange;
    public static event Action<float, float> onManaChange;

    public static event Action<Player> onPlayerWin;
    public static event Action<Player> onPlayerKill;

    [SerializeField] private int life = 10;
    [SerializeField] private float mana = 1;
    [SerializeField] private LayerMask damagableMask;
    [SerializeField] private LayerMask bookLayer;
    [SerializeField] private ParticleSystem aimParticleSystem;
    [SerializeField] private ParticleSystem spellParticleSystem;
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material controlMaterial;
    [SerializeField] private Material attackMaterial;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    public int Life => life;
    private int maxLife;

    public float Mana => mana;
    private float maxMana;

    private bool isHit = false;
    private float time = 0;
    public ParticleSystem AimParticleSystem => aimParticleSystem;
    public ParticleSystem SpellParticleSystem => spellParticleSystem;
    public Material IdleMaterial => idleMaterial;
    public Material ControlMaterial => controlMaterial;
    public Material AttackMaterial => attackMaterial;
    public ParticleSystemRenderer ParticleRenderer { get; private set; }
    public ParticleSystemRenderer SpellParticleRenderer { get; private set; }

    private void Awake()
    {
        Enemy.onEnemySpawn += OnEnemySpawn;

        ParticleRenderer = AimParticleSystem.GetComponent<ParticleSystemRenderer>();
        SpellParticleRenderer = SpellParticleSystem.GetComponent<ParticleSystemRenderer>();
        ParticleRenderer.material = IdleMaterial;
        // take this game object out of the player, so its position is relative to the world not the player
        spellParticleSystem.gameObject.transform.parent = gameObject.transform.parent;
    }

    private void Start()
    {
        maxLife = life;
        onLifeChange?.Invoke(life, maxLife);

        maxMana = mana;
        onManaChange?.Invoke(mana, maxMana);
    }

    private void OnDestroy()
    {
        Enemy.onEnemySpawn -= OnEnemySpawn;
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHit)
        {
            return;
        }

        if (Utils.CheckCollisionLayer(collision.gameObject, damagableMask))
        {
            CrateProjectile crate = collision.gameObject.GetComponent<CrateProjectile>();
            if (crate.CanMakeDamage())
            {
                TakeDamage(1);
            }
        }

        if (Utils.CheckCollisionLayer(collision.gameObject, bookLayer))
        {
            onPlayerWin?.Invoke(this);
        }
    }

    private void Update()
    {
        if (isHit)
        {
            time += Time.deltaTime;
            skinnedMeshRenderer.material.SetColor("_Tint", Color.Lerp(Color.black, Color.red, Mathf.Sin(time * 40)));
        }

        mana = Mathf.Clamp(mana + Time.deltaTime*0.5f, 0.0f, maxMana);
        onManaChange?.Invoke(mana, maxMana);
    }

    public override void Initialize(Controlable controlable)
    {
        ControlableData data = controlable.Data;
        // Basic states
        State<Controlable> idleState = new PlayerIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        State<Controlable> walkState = new ControlableWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        State<Controlable> jumpState = new ControlableJumpState(controlable, () => { return (data.currentJumpDone < data.jumpCount) && Input.GetKeyDown(KeyCode.Space); });
        State<Controlable> fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });
        // Additive states
        State<Controlable> spellCastState = new ControlableSpellCastState(controlable, () => { return Input.GetMouseButton(0); });

        StateGraph<Controlable> stateGraph = controlable.StateGraph;
        stateGraph.AddStateTransitions(idleState, new List<State<Controlable>> { walkState, fallState, jumpState, spellCastState });
        stateGraph.AddStateTransitions(walkState, new List<State<Controlable>> { idleState, fallState, jumpState, spellCastState });
        stateGraph.AddStateTransitions(fallState, new List<State<Controlable>> { idleState, walkState, spellCastState, jumpState });
        stateGraph.AddStateTransitions(jumpState, new List<State<Controlable>> { fallState, spellCastState });
        stateGraph.AddStateTransitions(spellCastState, new List<State<Controlable>> { idleState, walkState, jumpState, fallState });

        List<State<Controlable>> basicStates = new List<State<Controlable>> { idleState, walkState, jumpState, fallState };
        List<State<Controlable>> additiveStates = new List<State<Controlable>> { spellCastState };

        stateGraph.AddBasicStates(basicStates);
        stateGraph.AddAdditiveStates(additiveStates);
        stateGraph.SetInitialState(idleState);
    }

    private void OnEnemySpawn(Enemy enemy)
    {
        enemy.SetTarget(transform);
    }

    public void TakeDamage(int amount)
    {
        life = Mathf.Max(life - amount, 0);
        onLifeChange?.Invoke(life, maxLife);
        StartCoroutine(HitAnimation(2));
        if (life == 0)
        {
            onPlayerKill?.Invoke(this);
        }
    }

    IEnumerator HitAnimation(float seconds)
    {
        time = 0;
        isHit = true;
        yield return new WaitForSeconds(seconds);
        isHit = false;
        skinnedMeshRenderer.material.SetColor("_Tint", Color.black);
    }

    public void CastSpell()
    {
        mana -= 1.0f;
        onManaChange?.Invoke(mana, maxMana);
    }
}
