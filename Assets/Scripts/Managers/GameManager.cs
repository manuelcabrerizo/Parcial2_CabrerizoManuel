using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [field:SerializeField] public SoundClipsSO Clips { get; private set;}
    public static Action onResetCamera;
    public static Action<bool> onShowLoadingBar;

    private Player player = null;
    private Controlable controlable = null;
    private SceneReferences main = null;
    private string currentLoadedSceneName = null;
    private Transform targetTransform = null;

    private StateMachine fsm;
    private State<GameManager> playingState;
    private State<GameManager> pauseState;
    private State<GameManager> endState;
    private State<GameManager> gameOverState;
    private State<GameManager> winState;

    public bool IsWinner { get; private set; }

    protected override void OnAwaken()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;

        SceneReferences.onLoaded += OnSceneReferencesLoaded;
        Portal.onPortalToSceneEnter += OnPortalToSceneEnter;
        Portal.onPortalToMainEnter += OnPortalToMainEnter;
        Controlable.onControlableCreated += OnControlableCreated;
        UIPause.onResumeButtonClick += PauseGame;
        Player.onPlayerWin += OnPlayerWin;
        Player.onPlayerKill += OnPlayerKill;

        fsm = new StateMachine();
        playingState = new PlayingState(this);
        pauseState = new PauseState(this);
        endState = new EndState(this);
        winState = new WinState(this);
        gameOverState = new GameOverState(this);

    }

    private void Start()
    {
        fsm.PushState(playingState);
    }

    protected override void OnDestroyed()
    {
        fsm.Clear();
        SceneReferences.onLoaded -= OnSceneReferencesLoaded;
        Portal.onPortalToSceneEnter -= OnPortalToSceneEnter;
        Portal.onPortalToMainEnter -= OnPortalToMainEnter;
        Controlable.onControlableCreated -= OnControlableCreated;
        UIPause.onResumeButtonClick -= PauseGame;
        Player.onPlayerWin -= OnPlayerWin;
        Player.onPlayerKill -= OnPlayerKill;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            PauseGame();
        }

        fsm.Update();
    }

    private void FixedUpdate()
    {
        fsm.FixedUpdate();
    }

    public void SetPlayingState()
    {
        fsm.ChangeState(playingState);
    }

    public void SetGameOverState()
    {
        fsm.ChangeState(gameOverState);
    }

    public void SetWinState()
    {
        fsm.ChangeState(winState);
    }

    public void PauseGame()
    {
        if (fsm.PeekState() == playingState)
        {
            fsm.PushState(pauseState);
        }
        else if (fsm.PeekState() == pauseState)
        {
            fsm.PopState();
        }
    }

    private void OnSceneReferencesLoaded(SceneReferences scene)
    {
        if (main == null)
        {
            main = scene;
        }
    }

    private void OnPortalToSceneEnter(GameObject go, string sceneName, Transform targetTransform)
    {
        if (go == controlable.gameObject)
        {
            GameSceneManager.onLoadingCompleted += OnSceneLoadingComplete;
            GameSceneManager.Instance.ChangeSceneTo(sceneName, LoadSceneMode.Additive);
            currentLoadedSceneName = sceneName;
            this.targetTransform = targetTransform;
            onShowLoadingBar?.Invoke(true);
        }
    }

    private void OnPortalToMainEnter(GameObject go, Transform targetTransform)
    {
        if (go == controlable.gameObject)
        {
            controlable.transform.position = targetTransform.position;
            controlable.transform.rotation = targetTransform.rotation;
            onResetCamera?.Invoke();
            GameSceneManager.onLoadingCompleted += OnSceneUnloadingComplete;
            GameSceneManager.Instance.UnloadScene(currentLoadedSceneName);
            onShowLoadingBar?.Invoke(true);
        }
    }

    private void OnSceneLoadingComplete()
    {
        main.SetActiveGo(false);
        onShowLoadingBar?.Invoke(false);
        controlable.transform.position = targetTransform.position;
        controlable.transform.rotation = targetTransform.rotation;
        onResetCamera?.Invoke();
        GameSceneManager.onLoadingCompleted -= OnSceneLoadingComplete;
    }

    private void OnSceneUnloadingComplete()
    {
        main.SetActiveGo(true);
        onShowLoadingBar?.Invoke(false);
        GameSceneManager.onLoadingCompleted -= OnSceneUnloadingComplete;
    }

    private void OnPlayerCreated(Player player)
    {
        this.player = player;
    }

    private void OnControlableCreated(Controlable controlable)
    {
        this.controlable = controlable;
    }

    private void OnPlayerWin(Player player)
    {
        IsWinner = true;
        fsm.ChangeState(endState);
    }

    private void OnPlayerKill(Player player)
    {
        IsWinner = false;
        fsm.ChangeState(endState);
    }
}
