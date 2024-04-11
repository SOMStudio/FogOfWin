using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Data;
using Save;
using Ui;
using UnityEngine;
using UnityEngine.Events;

public class SlotManager : MonoSingleton<SlotManager>, ISlotManager
{
    [Header("Main")]
    [SerializeField] private int moneyAmount = 10000;
    [SerializeField] private int rateAmount = 200;
    [SerializeField] private int stepAmount = 50;
    [SerializeField] private TypeGame typeGame = TypeGame.Count;
    [SerializeField] private TypeBuster typeBuster = TypeBuster.Cell;

    [Header("Slot")]
    [SerializeField] private GameObject slotBase;
    [SerializeField] private Slot slotPoints;
    [SerializeField] private Slot slotCells;
    [SerializeField] private Slot slotCovers;

    [Header("Busters")]
    [SerializeField] private int countCellBuster = 5;
    [SerializeField] private int countLineVerticalBuster = 5;
    [SerializeField] private int countLineHorizontalBuster = 5;

    private float[] timeRotate;
    private float[] currentTime;
    private bool[] stopWheelState;
    
    private bool stopRotate = true;

    private GameLogic.SlotPosition selectSlotCover = new(){Wheel = -1, Cell = -1};
    private bool initTutorial;

    private ISaveManager saveManager;
    private IConsoleManager consoleManager;
    
    [Header("Managers")]
    [SerializeField] private GameLogic gameLogic;

    [Header("Events")]
    [SerializeField] private UnityEvent startSlotRotateEvent;
    [SerializeField] private UnityEvent stopSlotRotateEvent;
    [SerializeField] private UnityEvent<TypeGame> changeTypeGameEvent;
    [SerializeField] private UnityEvent<TypeBuster> changeTypeBusterEvent;
    [SerializeField] private UnityEvent<int, int, bool> changeMoneyAmountEvent;
    [SerializeField] private UnityEvent<int> changeRateAmountEvent;
    [SerializeField] private UnityEvent<Dictionary<int, int>, Dictionary<int, GameLogic.Result>> winRateDetailAmountEvent;

    [Header("Events for Animation")]
    [SerializeField] private UnityEvent startShowResultEvent;
    [SerializeField] private UnityEvent<int, int> changeRateAmountResultEvent;
    [SerializeField] private UnityEvent<int, int> changeMoneyAmountResultEvent;
    [SerializeField] private UnityEvent finishShowResultEvent;

    public UnityEvent<int, int, bool> ChangeMoneyAmountEvent => changeMoneyAmountEvent;
    public UnityEvent<int> ChangeRateAmountEvent => changeRateAmountEvent;

    private void Start()
    {
        timeRotate = new float[slotPoints.Length];
        currentTime = new float[slotPoints.Length];
        stopWheelState = new bool[slotPoints.Length];
        
        for (var i = 0; i < stopWheelState.Length; i++) stopWheelState[i] = true;
        
        startSlotRotateEvent.AddListener(HideCellCover);
        stopSlotRotateEvent.AddListener(ShowCellCover);
    }

    private void Update()
    {
        if (!gameLogic) return;

        gameLogic.MoveWheelCells(slotPoints, slotCells, timeRotate, currentTime, stopWheelState, stopRotate,
            Time.deltaTime, () => stopSlotRotateEvent?.Invoke());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        saveManager.ChangeValueEvent -= UpdateSaveManagerListener;
    }

    public void Init(GameLogic gameLogicSet, ISaveManager saveManagerSet, IConsoleManager consoleManagerSet)
    {
        gameLogic = gameLogicSet;
        saveManager = saveManagerSet;
        consoleManager = consoleManagerSet;
        
        moneyAmount = saveManager.GetValueInt(GameLogic.MoneyAmountKey);
        rateAmount = saveManager.GetValueInt(GameLogic.RateAmountKey);
        stepAmount = saveManager.GetValueInt(GameLogic.StepAmountKey);

        countCellBuster = saveManager.GetValueInt(GameLogic.CountCellBusterKey);
        countLineVerticalBuster = saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey);
        countLineHorizontalBuster = saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey);

        saveManager.ChangeValueEvent += UpdateSaveManagerListener;
        
        gameLogic.InitGameState(slotPoints, slotCells);
        
        finishShowResultEvent.AddListener(UpdateAmountListener);

        changeMoneyAmountEvent?.Invoke(moneyAmount, moneyAmount, false);
        changeRateAmountEvent?.Invoke(rateAmount);
    }

    private void CheckResult()
    {
        var result = gameLogic.CountRewardResult(rateAmount, slotCells, typeGame, out var resultCells);

        StartCoroutine(gameLogic.ShowResult(slotPoints, moneyAmount, rateAmount, result, resultCells,
            () => startShowResultEvent?.Invoke(),
            (a, b) => changeMoneyAmountResultEvent?.Invoke(a, b),
            (a, b) => changeRateAmountResultEvent?.Invoke(a, b),
            () => finishShowResultEvent?.Invoke()));
        
        winRateDetailAmountEvent?.Invoke(result, resultCells);

        int sumResult = result.Sum(el => el.Value);

        changeMoneyAmountEvent?.Invoke(moneyAmount, moneyAmount - rateAmount + sumResult, true);
        
        moneyAmount += sumResult;
        moneyAmount -= rateAmount;
    }

    private void SetTypeGame(TypeGame typeGameSet)
    {
        typeGame = typeGameSet;
        
        changeTypeGameEvent?.Invoke(typeGame);
    }

    private void SetTypeBuster(TypeBuster typeBusterSet)
    {
        typeBuster = typeBusterSet;
        
        changeTypeBusterEvent?.Invoke(typeBuster);
    }

    private void StartStopSlotRotate()
    {
        if (stopRotate)
        {
            gameLogic.InitRandomSpeedRotateWheel(timeRotate);
            
            startSlotRotateEvent?.Invoke();
            
            changeRateAmountResultEvent?.Invoke(rateAmount, 0);
        }
        
        stopRotate = !stopRotate;
        for (var i = 0; i < stopWheelState.Length; i++) stopWheelState[i] = false;
    }

    private void ChangeRateAmount(int changeAmount)
    {
        int newRateAmount = rateAmount + changeAmount;
        
        saveManager.SetValue(GameLogic.RateAmountKey, newRateAmount);
        
        changeRateAmountEvent?.Invoke(newRateAmount);
    }

    private void ChangeMoneyAmount(int changeAmount)
    {
        int newAmount = moneyAmount + changeAmount;
        
        changeMoneyAmountEvent?.Invoke(moneyAmount, newAmount, false);
        
        saveManager.SetValue(GameLogic.MoneyAmountKey, newAmount);
    }

    private void ShowCellCover()
    {
        foreach (var slotWheel in slotCovers.wheels)
        {
            foreach (var wheelCell in slotWheel.places)
            {
                wheelCell.ShowSprite();
            }
        }
    }

    private void HideCellCover()
    {
        foreach (var slotWheel in slotCovers.wheels)
        {
            foreach (var wheelCell in slotWheel.places)
            {
                wheelCell.HideSprite();
            }
        }

        UnselectSlotCoverAll();
    }

    private void SelectSlotCover(GameLogic.SlotPosition slotPosition)
    {
        selectSlotCover = slotPosition;

        var slotCellList = gameLogic.SelectSlotCellList(slotCovers, typeBuster, selectSlotCover);
        
        foreach (var wheelCell in slotCellList)
        {
            wheelCell.SetColor(gameLogic.colorSelectCoverCell);
        }
    }

    private void UnselectSlotCover(GameLogic.SlotPosition slotPosition)
    {
        if (selectSlotCover.Wheel == -1) return;
        
        var slotCellList = gameLogic.SelectSlotCellList(slotCovers, typeBuster, selectSlotCover);
        
        foreach (var wheelCell in slotCellList)
        {
            wheelCell.SetColor(gameLogic.colorCoverCell);
        }
        
        selectSlotCover = new GameLogic.SlotPosition { Wheel = -1, Cell = -1 };
    }
    
    private void UnselectSlotCoverAll()
    {
        foreach (var wheel in slotCovers.wheels)
        {
            foreach (var place in wheel.places)
            {
                place.SetColor(gameLogic.colorCoverCell);
            }
        }
    }

    private void ClickSlotCover(GameLogic.SlotPosition slotPosition)
    {
        if (selectSlotCover.Wheel < 0) SelectSlotCover(slotPosition);
        else if (selectSlotCover != slotPosition)
        {
            UnselectSlotCover(selectSlotCover);
            SelectSlotCover(slotPosition);
        }
        else
        {
            if (GetBusterCount(typeBuster) <= 0) return;
            
            var slotCellList = gameLogic.SelectSlotCellList(slotCovers, typeBuster, selectSlotCover);
        
            foreach (var wheelCell in slotCellList)
            {
                wheelCell.HideSprite();
            }
            
            AddBusterCount(typeBuster, -1);
            UnselectSlotCover(selectSlotCover);
        }
    }

    private void AddBusterCount(TypeBuster typeBusterCheck, int addCount)
    {
        switch (typeBusterCheck)
        {
            case TypeBuster.LineHorizontal:
                saveManager.SetValue(GameLogic.CountLineHorizontalBusterKey, countLineHorizontalBuster + addCount);
                break;
            case TypeBuster.LineVertical:
                saveManager.SetValue(GameLogic.CountLineVerticalBusterKey, countLineVerticalBuster + addCount);
                break;
            default:
                saveManager.SetValue(GameLogic.CountCellBusterKey, countCellBuster + addCount);
                break;
        }
    }

    private int GetBusterCount(TypeBuster typeBusterCheck)
    {
        switch (typeBusterCheck)
        {
            case TypeBuster.LineHorizontal:
                return countLineHorizontalBuster;
            case TypeBuster.LineVertical:
                return countLineVerticalBuster;
            default:
                return countCellBuster;
        }
    }

    public void ShowGame()
    {
        slotBase.SetActive(true);
    }

    public void HideGame()
    {
        slotBase.SetActive(false);
    }

    public void ShowTutorial()
    {
        initTutorial = true;
    }

    public void HideTutorial()
    {
        initTutorial = false;
    }

    #region Listener
    public void OnMouseEnterCellCoverListener(string wheelCell)
    {
        #if !UNITY_ANDROID
        var wheel = Int32.Parse(wheelCell[0].ToString());
        var cell = Int32.Parse(wheelCell[1].ToString());

        SelectSlotCover(new GameLogic.SlotPosition { Wheel = wheel, Cell = cell });
        #endif
    }
    
    public void OnMouseExitCellCoverListener(string wheelCell)
    {
        #if !UNITY_ANDROID
        var wheel = Int32.Parse(wheelCell[0].ToString());
        var cell = Int32.Parse(wheelCell[1].ToString());

        UnselectSlotCover(new GameLogic.SlotPosition { Wheel = wheel, Cell = cell });
        #endif
    }

    public void OnMouseUpCellCoverListener(string wheelCell)
    {
        if (initTutorial) return;
        
        var wheel = Int32.Parse(wheelCell[0].ToString());
        var cell = Int32.Parse(wheelCell[1].ToString());

        ClickSlotCover(new GameLogic.SlotPosition { Wheel = wheel, Cell = cell });
    }

    private void UpdateSaveManagerListener(string value)
    {
        switch (value)
        {
            case GameLogic.CountCellBusterKey:
                countCellBuster = saveManager.GetValueInt(GameLogic.CountCellBusterKey);
                break;
            case GameLogic.CountLineHorizontalBusterKey:
                countLineHorizontalBuster = saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey);
                break;
            case GameLogic.CountLineVerticalBusterKey:
                countLineVerticalBuster = saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey);
                break;
            case GameLogic.MoneyAmountKey:
                moneyAmount = saveManager.GetValueInt(GameLogic.MoneyAmountKey);
                changeMoneyAmountEvent?.Invoke(moneyAmount, moneyAmount, false);
                break;
            case GameLogic.RateAmountKey:
                rateAmount = saveManager.GetValueInt(GameLogic.RateAmountKey);
                break;
        }
    }

    private void UpdateAmountListener()
    {
        saveManager.SetValue(GameLogic.MoneyAmountKey, moneyAmount);
    }
    #endregion

    #region UI
    public void RotateSlotButton()
    {
        if (!stopRotate) return;
            
        StartStopSlotRotate();
        
        var randomTimeRotate = gameLogic.GetRandomRange(gameLogic.minTimeRotate,
            gameLogic.maxTimeRotate);
        
        Invoke(nameof(StartStopSlotRotate), randomTimeRotate);
    }

    public void ResultSlotButton()
    {
        if (!stopRotate) return;

        HideCellCover();
        CheckResult();
    }

    public void SetCountTypeGameButton()
    {
        SetTypeGame(TypeGame.Count);
    }
    
    public void SetNearTypeGameButton()
    {
        SetTypeGame(TypeGame.Near);
    }
    
    public void SetLineTypeGameButton()
    {
        SetTypeGame(TypeGame.Line);
    }
    
    public void SetCellTypeBusterButton()
    {
        SetTypeBuster(TypeBuster.Cell);
    }

    public void SetLineHorizontalTypeBusterButton()
    {
        SetTypeBuster(TypeBuster.LineHorizontal);
    }
    
    public void SetLineVerticalTypeBusterButton()
    {
        SetTypeBuster(TypeBuster.LineVertical);
    }
    
    public void IncreaseRateAmountButton()
    {
        ChangeRateAmount(stepAmount);
        ChangeMoneyAmount(-stepAmount);
    }
    
    public void DecreaseRateAmountButton()
    {
        ChangeRateAmount(-stepAmount);
        ChangeMoneyAmount(stepAmount);
    }
    #endregion
}

public interface ISlotManager
{
    UnityEvent<int, int, bool> ChangeMoneyAmountEvent { get; }
    UnityEvent<int> ChangeRateAmountEvent { get; }

    void Init(GameLogic gameLogicSet, ISaveManager saveManager, IConsoleManager consoleManager);

    void HideGame();
    void ShowGame();
}
