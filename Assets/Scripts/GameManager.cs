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

    private bool needUpdateGameState = false;
    private bool needInitUiMain = false;
    private bool needInitUiGame = false;
    private bool needInitSlot = false;

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
        if (!saveManager) saveManager = SaveManager.instance;
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
        if (needInitUiMain)
        {
            needInitUiMain = false;
            InitUiMainManager(uiMainManager);
        }
        if (needInitUiGame)
        {
            needInitUiGame = false;
            InitUiGameManager(uiGameManager);
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
    
    private void InitUiMainManager(IUiMainManager uiMainManagerInit)
    {
        uiMainManagerInit.ButtonPlayEvent.AddListener(StartGame);
    }

    private void InitUiGameManager(IUiGameManager uiGameManagerInit)
    {
        uiGameManagerInit.ButtonMainMenuEvent.AddListener(StartMainMenu);
    }

    private void InitSlotManager(ISlotManager slotManagerInit, ISaveManager saveManagerInit)
    {
        int moneyAmount = saveManagerInit.GetValue(GameLogic.MoneyAmountKey, gameLogic.moneyAmountDefault);
        int rateAmount = saveManagerInit.GetValue(GameLogic.RateAmountKey, gameLogic.rateAmountDefault);
        int stepAmount = saveManagerInit.GetValue(GameLogic.StepAmountKey, gameLogic.stepAmountDefault);
        int countCellBuster = saveManagerInit.GetValue(GameLogic.CountCellBusterKey, gameLogic.countCellBusterDefault);
        int countLineVerticalBuster = saveManagerInit.GetValue(GameLogic.CountLineVerticalBusterKey, gameLogic.countLineVerticalBusterDefault);
        int countLineHorizontalBuster = saveManagerInit.GetValue(GameLogic.CountLineHorizontalBusterKey, gameLogic.countLineHorizontalBusterDefault);

        slotManagerInit.Init(gameLogic, moneyAmount, rateAmount, stepAmount, countCellBuster, countLineVerticalBuster, countLineHorizontalBuster);
        
        slotManagerInit.ChangeMoneyAmountEvent.AddListener(ChangeMoneyAmountListener);
        slotManagerInit.ChangeRateAmountEvent.AddListener(ChangeRateAmountListener);
        slotManagerInit.ChangeBusterCountEvent.AddListener(ChangeBusterCountListener);
    }

    private void ChangeMoneyAmountListener(int oldAmount, int newAmount, bool afterRotate)
    {
        saveManager.SetValue(GameLogic.MoneyAmountKey, newAmount);
    }

    private void ChangeRateAmountListener(int newAmount)
    {
        saveManager.SetValue(GameLogic.RateAmountKey, newAmount);
    }
    
    private void ChangeBusterCountListener(TypeBuster typeBuster, int count)
    {
        switch (typeBuster)
        {
            case TypeBuster.Cell:
                saveManager.SetValue(GameLogic.CountCellBusterKey, count);
                break;
            case TypeBuster.LineVertical:
                saveManager.SetValue(GameLogic.CountLineVerticalBusterKey, count);
                break;
            case TypeBuster.LineHorizontal:
                saveManager.SetValue(GameLogic.CountLineHorizontalBusterKey, count);
                break;
        }
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
