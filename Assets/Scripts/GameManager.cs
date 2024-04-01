using System.Collections;
using Base;
using Data;
using Music;
using Save;
using Sound;
using Ui;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Main")]
    [SerializeField] private GameState gameState = GameState.MainMenu;
    
    [Header("Data")]
    [SerializeField] private GameLogic gameLogic;

    [Header("Managers")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private UiMainManager uiMainManager;
    [SerializeField] private UiGameManager uiGameManager;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private SoundManager soundManager;

    private bool needInitSaveSystem;
    private bool needUpdateGameState;
    private bool needInitUiMain;
    private bool needInitUiGame;
    private bool needInitSlot;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
        
        CheckManagers();
    }

    private void Start()
    {
        InitManagers();
    }

    private void CheckManagers()
    {
        if (!saveManager)
        {
            saveManager = SaveManager.instance;
            if (saveManager) needInitSaveSystem = true;
        }
        if (!musicManager) musicManager = MusicManager.instance;
        if (!soundManager) soundManager = SoundManager.instance;
        if (!slotManager)
        {
            slotManager = SlotManager.instance;
            if (slotManager) needInitSlot = true;
        }
        if (!uiMainManager)
        {
            uiMainManager = UiMainManager.instance;
            if (uiMainManager) needInitUiMain = true;
        }
        if (!uiGameManager)
        {
            uiGameManager = UiGameManager.instance;
            if (uiGameManager) needInitUiGame = true;
        }
    }

    private void InitManagers()
    {
        if (needInitSaveSystem)
        {
            needInitSaveSystem = false;
            InitSaveManager();
        }
        if (needInitUiMain)
        {
            needInitUiMain = false;
            InitUiMainManager(uiMainManager, gameLogic, saveManager);
        }
        if (needInitUiGame)
        {
            needInitUiGame = false;
            InitUiGameManager(uiGameManager, saveManager);
        }
        if (slotManager && saveManager && needInitSlot)
        {
            needInitSlot = false;
            InitSlotManager(slotManager, saveManager);
        }
        if (needUpdateGameState)
        {
            needUpdateGameState = false;
            ChangeGameState(slotManager, uiMainManager, uiGameManager);
        }
    }

    private void InitSaveManager()
    {
        if (!saveManager.HasValue(GameLogic.MoneyAmountKey))
            saveManager.SetValue(GameLogic.MoneyAmountKey, gameLogic.moneyAmountDefault);
        if (!saveManager.HasValue(GameLogic.RateAmountKey))
            saveManager.SetValue(GameLogic.RateAmountKey, gameLogic.rateAmountDefault);
        if (!saveManager.HasValue(GameLogic.StepAmountKey))
            saveManager.SetValue(GameLogic.StepAmountKey, gameLogic.stepAmountDefault);

        if (!saveManager.HasValue(GameLogic.CountCellBusterKey))
            saveManager.SetValue(GameLogic.CountCellBusterKey, gameLogic.countCellBusterDefault);
        if (!saveManager.HasValue(GameLogic.CountLineVerticalBusterKey))
            saveManager.SetValue(GameLogic.CountLineVerticalBusterKey, gameLogic.countLineVerticalBusterDefault);
        if (!saveManager.HasValue(GameLogic.CountLineHorizontalBusterKey))
            saveManager.SetValue(GameLogic.CountLineHorizontalBusterKey, gameLogic.countLineHorizontalBusterDefault);

        if (!saveManager.HasValue(GameLogic.CountEnterInGameKey))
            saveManager.SetValue(GameLogic.CountEnterInGameKey, 1);
        else
            saveManager.SetValue(GameLogic.CountEnterInGameKey, saveManager.GetValueInt(GameLogic.CountEnterInGameKey) + 1);
        
        if (!saveManager.HasValue(GameLogic.SoundVolumeKey))
            saveManager.SetValue(GameLogic.SoundVolumeKey, gameLogic.soundVolumeDefault);
        if (!saveManager.HasValue(GameLogic.MusicVolumeKey))
            saveManager.SetValue(GameLogic.MusicVolumeKey, gameLogic.musicVolumeDefault);
    }

    private void ChangeGameState(ISlotManager slotManagerGame, IUiMainManager uiMainManagerActive, IUiGameManager uiGameManagerActive)
    {
        switch (gameState)
        {
            case GameState.MainMenu:
                uiMainManagerActive?.ShowMainPanels();
                uiGameManagerActive?.HideGamePanels();
                slotManagerGame?.HideGame();
                break;
            case GameState.Game:
                uiMainManagerActive?.HideMainPanels();
                uiGameManagerActive?.ShowGamePanels();
                slotManagerGame?.ShowGame();
                break;
        }
    }
    
    private void InitUiMainManager(IUiMainManager uiMainManagerInit, GameLogic gameLogicInit , ISaveManager saveManagerInit)
    {
        uiMainManagerInit.Init(gameLogicInit, saveManagerInit);
        
        uiMainManagerInit.ButtonPlayEvent.AddListener(StartGame);
        
        uiMainManagerInit.SliderSoundEvent.AddListener(ChangeSoundVolume);
        uiMainManagerInit.SliderMusicEvent.AddListener(ChangeMusicVolume);
    }

    private void ChangeSoundVolume(float newAmount)
    {
        saveManager.SetValue(GameLogic.SoundVolumeKey, newAmount);
        soundManager.UpdateVolume();
    }
    
    private void ChangeMusicVolume(float newAmount)
    {
        saveManager.SetValue(GameLogic.MusicVolumeKey, newAmount);
        musicManager.UpdateVolume();
    }

    private void InitUiGameManager(IUiGameManager uiGameManagerInit, ISaveManager saveManagerInit)
    {
        uiGameManagerInit.Init(saveManagerInit);
        
        uiGameManagerInit.ButtonMainMenuEvent.AddListener(StartMainMenu);
    }

    private void InitSlotManager(ISlotManager slotManagerInit, ISaveManager saveManagerInit)
    {
        slotManagerInit.Init(gameLogic, saveManagerInit);
    }

    private IEnumerator StartGameAsync(IUiMainManager mainMenu)
    {
        var loadScene = SceneManager.LoadSceneAsync("Game");

        mainMenu.LoadWindowShow();
        while (!loadScene.isDone)
        {
            mainMenu.LoadWindowProgress(loadScene.progress);
            
            yield return null;
        }
        mainMenu.LoadWindowHide();

        gameState = GameState.Game;
        needUpdateGameState = true;
        
        CheckManagers();

        yield return null;
        
        InitManagers();

        yield return null;
    }

    private IEnumerator StartMainAsync()
    {
        var loadScene = SceneManager.LoadSceneAsync("Main");

        while (!loadScene.isDone)
        {
            yield return null;
        }

        gameState = GameState.MainMenu;
        needUpdateGameState = true;
        
        CheckManagers();

        yield return null;
        
        InitManagers();

        yield return null;
    }
    
    private void StartGame()
    {
        if (!uiGameManager)
        {
            StartCoroutine(StartGameAsync(uiMainManager));
        }
        else
        {
            gameState = GameState.Game;
            needUpdateGameState = true;
            
            InitManagers();
        }
    }

    private void StartMainMenu()
    {
        if (!uiMainManager)
        {
            StartCoroutine(StartMainAsync());
        }
        else
        {
            gameState = GameState.MainMenu;
            needUpdateGameState = true;
            
            InitManagers();
        }
    }
}

public enum GameState
{
    MainMenu,
    Game
}
