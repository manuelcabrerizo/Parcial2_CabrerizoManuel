using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Player : CustomControlable, IDamagable
{
    public static event Action<int, int> onLifeChange;
    public static event Action<float, float> onManaChange;

    public static event Action<Player> onPlayerWin;
    public static event Action<Player> onPlayerKill;

    [field: SerializeField] public SoundClipsSO Clips { get; private set; }
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
    [SerializeField] private Transform foot;
    [SerializeField] private AudioSource spellSound;

    public int Life => life;
    private int maxLife;

    public float Mana => mana;
    private float maxMana;
    private bool manaFull = true;

    private bool isHit = false;
    private float hitTime = 0;

    private float footStepTime = 0;

    private Controlable controlable;

    public ParticleSystem AimParticleSystem => aimParticleSystem;
    public ParticleSystem SpellParticleSystem => spellParticleSystem;
    public Material IdleMaterial => idleMaterial;
    public Material ControlMaterial => controlMaterial;
    public Material AttackMaterial => attackMaterial;
    public AudioSource SpellSound => spellSound;
    public ParticleSystemRenderer ParticleRenderer { get; private set; }
    public ParticleSystemRenderer SpellParticleRenderer { get; private set; }

    private void Awake()
    {
        Enemy.onEnemySpawn += OnEnemySpawn;
        Controlable.onControlableCreated += OnControlableCreated;
        Controlable.onControlableBreakFree += OnControlableBreakFree;

        ParticleRenderer = AimParticleSystem.GetComponent<ParticleSystemRenderer>();
        SpellParticleRenderer = SpellParticleSystem.GetComponent<ParticleSystemRenderer>();
        ParticleRenderer.material = IdleMaterial;
        controlable = GetComponent<Controlable>();
        // take this game object out of the player, so its position is relative to the world not the player
        spellParticleSystem.gameObject.transform.parent = gameObject.transform.parent;
    }

    protected override void OnStart()
    {
        maxLife = life;
        onLifeChange?.Invoke(life, maxLife);

        maxMana = mana;
        onManaChange?.Invoke(mana, maxMana);

        animator.SetBool("IsAlive", true);
        animator.SetBool("IsWinner", false);
    }

    private void OnDestroy()
    {
        Enemy.onEnemySpawn -= OnEnemySpawn;
        Controlable.onControlableCreated -= OnControlableCreated;
        Controlable.onControlableBreakFree -= OnControlableBreakFree;


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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.CheckCollisionLayer(other.gameObject, bookLayer))
        {
            animator.SetBool("IsWinner", true);
            other.gameObject.SetActive(false);
            onPlayerWin?.Invoke(this);
        }
    }

    private void Update()
    {
        if (isHit)
        {
            hitTime += Time.deltaTime;
            skinnedMeshRenderer.material.SetColor("_Tint", Color.Lerp(Color.black, Color.red, Mathf.Sin(hitTime * 40)));
        }

        ProcessSpellSound();
        UpdateMana();

        if (controlable)
        {
            PlayAudio();
        }
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

    private void ProcessSpellSound()
    {
        if (SpellSound.isPlaying && controlable.IsPause)
        {
            SpellSound.Stop();
        }
    }

    private void UpdateMana()
    {
        mana = Mathf.Clamp(mana + Time.deltaTime * 0.5f, 0.0f, maxMana);
        onManaChange?.Invoke(mana, maxMana);

        if (mana < maxMana)
        {
            manaFull = false;
        }

        if (manaFull == false && mana >= maxMana)
        {
            AudioManager.onPlayClip?.Invoke(Clips.manaFull, ClipType.SFX);
            manaFull = true;
        }
    }

    private void OnEnemySpawn(Enemy enemy)
    {
        enemy.SetTarget(transform);
    }

    public void TakeDamage(int amount)
    {
        AudioManager.onPlayClip3D?.Invoke(Clips.onHit, transform.position, 1, 4);
        life = Mathf.Max(life - amount, 0);
        onLifeChange?.Invoke(life, maxLife);
        StartCoroutine(HitAnimation(2));
        if (life == 0)
        {
            animator.SetBool("IsAlive", false);
            onPlayerKill?.Invoke(this);
        }
    }

    IEnumerator HitAnimation(float seconds)
    {
        hitTime = 0;
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

    private void PlayAudio()
    {
        ControlableData data = controlable.Data;
        Vector2 move = new Vector2(data.xInput, data.yInput);
        if (!controlable.IsPause && data.isGrounded && move.magnitude > 0.01f)
        {
            footStepTime += Time.deltaTime;
            if (footStepTime > 0.4f)
            {
                PlayFootSteps();
                footStepTime = 0.0f;
            }
        }
        else
        {
            footStepTime = 0.0f;
        }
    }

    private void PlayFootSteps()
    {
        AudioClip clip = Clips.footSteps[2];
        if (Terrain.activeTerrain)
        {

            Terrain terrain = Terrain.activeTerrain;
            Vector3 pos = GetMapPos();

            int mapX = Mathf.FloorToInt(pos.x * terrain.terrainData.alphamapWidth);
            int mapZ = Mathf.FloorToInt(pos.z * terrain.terrainData.alphamapHeight);

            float[,,] splatmapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            int maxTextures = terrain.terrainData.alphamapLayers;

            float maxValue = 0.0f;
            int index = 0;
            for (int i = 0; i < maxTextures; ++i)
            {
                if (splatmapData[0, 0, i] > maxValue)
                {
                    maxValue = splatmapData[0, 0, i];
                    index = i;
                }
            }
            switch (index)
            {
                case 0: clip = Clips.footSteps[0]; break;
                case 2: clip = Clips.footSteps[1]; break;
            }
            AudioManager.onPlayClip3D?.Invoke(clip, foot.position, 1, 4);
        }
        else
        {
            AudioManager.onPlayClip3D?.Invoke(clip, foot.position, 1, 4);
        }
    }

    private Vector3 GetMapPos()
    {
        ControlableData data = controlable.Data;
        Vector3 pos = data.body.position;
        Terrain terrain = Terrain.activeTerrain;
        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }

    private void OnControlableCreated(Controlable controlable)
    {
        if (controlable.TryGetComponent<Player>(out _))
        {
            this.controlable = controlable;
        }
    }

    private void OnControlableBreakFree(Controlable controlable)
    {
        if (controlable.TryGetComponent<Player>(out _))
        {
            this.controlable.Data.animator.SetBool("IsGrounded", true);
            this.controlable.Data.animator.SetBool("IsAiming", false);
            this.controlable.Data.animator.SetFloat("VelocityX", 0.0f);
            this.controlable.Data.animator.SetFloat("VelocityZ", 0.0f);
            this.controlable = null;
        }
    }
}
