using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SlotManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private int moneyAmount = 10000;
    [SerializeField] private TypeGame typeGame = TypeGame.Count;
    [SerializeField] private int rateAmount;
    [SerializeField] private TypeBuster typeBuster = TypeBuster.Cell;
    
    [Header("Slot")]
    [SerializeField] private Slot slotPoints;
    [SerializeField] private Slot slotCells;
    [SerializeField] private Slot slotCovers;

    [Header("Cover")]
    [SerializeField] private Color colorCoverCell;
    [SerializeField] private Color colorSelectCoverCell;

    [Header("Busters")]
    [SerializeField] private int countCellBuster = 5;
    [SerializeField] private int countLineVerticalBuster = 5;
    [SerializeField] private int countLineHorizontalBuster = 5;

    private float[] timeRotate;
    private float[] currentTime;
    private bool stopRotate = true;

    private GameLogic.SlotPosition selectSlotCover = new(){Wheel = -1, Cell = -1};
    
    private GameManager gameManager;

    [Header("Events")]
    public UnityEvent startSlotRotateEvent;
    public UnityEvent stopSlotRotateEvent;
    public UnityEvent<TypeGame> changeTypeGameEvent;
    public UnityEvent<TypeBuster> changeTypeBusterEvent;
    public UnityEvent<int> changeRateAmountEvent;
    public UnityEvent<int, int, bool> changeMoneyAmountEvent;
    public UnityEvent<Dictionary<int, int>, Dictionary<int, GameLogic.Result>> winRateDetailAmountEvent;
    public UnityEvent<TypeBuster, int> changeBusterCountEvent;

    [Header("Events for Animation")]
    public UnityEvent startShowResultEvent;
    public UnityEvent<int, int> changeRateAmountResultEvent;
    public UnityEvent<int, int> changeMoneyAmountResultEvent;
    public UnityEvent finishShowResultEvent;

    private void Start()
    {
        gameManager = (GameManager)GameManager.instance;

        rateAmount = gameManager.GameLogic.rateAmountDefault;
        
        timeRotate = new float[slotPoints.Length];
        currentTime = new float[slotPoints.Length];
            
        gameManager.GameLogic.InitGameState(slotPoints, slotCells);
        
        startSlotRotateEvent.AddListener(HideCellCover);
        
        stopSlotRotateEvent.AddListener(ShowCellCover);
        
        changeMoneyAmountEvent?.Invoke(moneyAmount, moneyAmount, false);
    }

    private void Update()
    {
        gameManager.GameLogic.MoveWheelCells(slotPoints, slotCells, timeRotate, currentTime, stopRotate,
            () => stopSlotRotateEvent?.Invoke(), Time.deltaTime);
    }

    private void CheckResult()
    {
        var resultCells = new Dictionary<int, GameLogic.Result>();
        
        var result = gameManager.GameLogic.CountRewardResult(rateAmount, slotCells, typeGame, out resultCells);

        StartCoroutine(ShowResult(moneyAmount, result, resultCells));
        
        winRateDetailAmountEvent?.Invoke(result, resultCells);

        int sumResult = result.Sum(el => el.Value);

        changeMoneyAmountEvent?.Invoke(moneyAmount, moneyAmount - rateAmount + sumResult, true);
        
        moneyAmount += sumResult;
        moneyAmount -= rateAmount;
    }

    private IEnumerator ShowResult(int amount, Dictionary<int, int> result, Dictionary<int, GameLogic.Result> resultCells)
    {
        startShowResultEvent?.Invoke();
        
        var currentAmount = amount;
        
        yield return new WaitForSeconds(gameManager.GameLogic.startDelayBeforeResult);
        
        foreach (var itemResult in result)
        {
            int numberPicture = itemResult.Key;
            int winAmount = itemResult.Value;
            
            foreach (var slotPosition in resultCells[numberPicture].FirstWheel)
            {
                slotPoints[slotPosition.Wheel, slotPosition.Cell].ShowSprite();
            }

            foreach (var slotPosition in resultCells[numberPicture].OtherWheels)
            {
                slotPoints[slotPosition.Wheel, slotPosition.Cell].ShowSprite();
            }

            yield return new WaitForSeconds(gameManager.GameLogic.timeShowResult);
            
            foreach (var slotPosition in resultCells[numberPicture].FirstWheel)
            {
                slotPoints[slotPosition.Wheel, slotPosition.Cell].HideSprite();
            }

            foreach (var slotPosition in resultCells[numberPicture].OtherWheels)
            {
                slotPoints[slotPosition.Wheel, slotPosition.Cell].HideSprite();
            }
            
            changeMoneyAmountResultEvent?.Invoke(currentAmount, currentAmount + winAmount);

            currentAmount += winAmount;
            
            yield return new WaitForSeconds(gameManager.GameLogic.finalDelayAfterStepResult);
        }
        
        changeMoneyAmountResultEvent?.Invoke(currentAmount, currentAmount - rateAmount);
            
        currentAmount -= rateAmount;
        
        changeRateAmountResultEvent?.Invoke(0, rateAmount);
        
        finishShowResultEvent?.Invoke();
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
            gameManager.GameLogic.InitRandomSpeedRotateWheel(timeRotate);
            
            startSlotRotateEvent?.Invoke();
            
            changeRateAmountResultEvent?.Invoke(rateAmount, 0);
        }
        
        stopRotate = !stopRotate;
    }

    private void ChangeRateAmount(int changeAmount)
    {
        rateAmount += changeAmount;
        
        changeRateAmountEvent?.Invoke(rateAmount);
    }

    private void ChangeMoneyAmount(int changeAmount)
    {
        changeMoneyAmountEvent?.Invoke(moneyAmount, moneyAmount + changeAmount, false);
            
        moneyAmount += changeAmount;
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
    }

    private void SelectSlotCover(GameLogic.SlotPosition slotPosition)
    {
        selectSlotCover = slotPosition;

        var slotCellList = gameManager.GameLogic.SelectSlotCellList(slotCovers, typeBuster, selectSlotCover);
        
        foreach (var wheelCell in slotCellList)
        {
            wheelCell.SetColor(colorSelectCoverCell);
        }
    }

    private void UnselectSlotCover(GameLogic.SlotPosition slotPosition)
    {
        var slotCellList = gameManager.GameLogic.SelectSlotCellList(slotCovers, typeBuster, selectSlotCover);
        
        foreach (var wheelCell in slotCellList)
        {
            wheelCell.SetColor(colorCoverCell);
        }
        
        selectSlotCover = new GameLogic.SlotPosition { Wheel = -1, Cell = -1 };
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
            
            var slotCellList = gameManager.GameLogic.SelectSlotCellList(slotCovers, typeBuster, selectSlotCover);
        
            foreach (var wheelCell in slotCellList)
            {
                wheelCell.HideSprite();
            }
            
            AddBusterCount(typeBuster, -1);
        }
    }

    private void AddBusterCount(TypeBuster typeBusterCheck, int addCount)
    {
        switch (typeBusterCheck)
        {
            case TypeBuster.LineHorizontal:
                countLineHorizontalBuster += addCount;
                
                changeBusterCountEvent?.Invoke(typeBusterCheck, countLineHorizontalBuster);
                break;
            case TypeBuster.LineVertical:
                countLineVerticalBuster += addCount;
                
                changeBusterCountEvent?.Invoke(typeBusterCheck, countLineVerticalBuster);
                break;
            default:
                countCellBuster += addCount;
                
                changeBusterCountEvent?.Invoke(typeBusterCheck, countCellBuster);
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

    #region Listener
    public void OnMouseEnterCellCoverListener(string wheelCell)
    {
        var wheel = Int32.Parse(wheelCell[0].ToString());
        var cell = Int32.Parse(wheelCell[1].ToString());

        SelectSlotCover(new GameLogic.SlotPosition { Wheel = wheel, Cell = cell });
    }
    
    public void OnMouseExitCellCoverListener(string wheelCell)
    {
        var wheel = Int32.Parse(wheelCell[0].ToString());
        var cell = Int32.Parse(wheelCell[1].ToString());

        UnselectSlotCover(new GameLogic.SlotPosition { Wheel = wheel, Cell = cell });
    }

    public void OnMouseUpCellCoverListener(string wheelCell)
    {
        var wheel = Int32.Parse(wheelCell[0].ToString());
        var cell = Int32.Parse(wheelCell[1].ToString());

        ClickSlotCover(new GameLogic.SlotPosition { Wheel = wheel, Cell = cell });
    }
    #endregion

    #region UI
    public void RotateSlotButton()
    {
        if (!stopRotate) return;
            
        StartStopSlotRotate();
        
        var randomTimeRotate = gameManager.GameLogic.GetRandomRange(gameManager.GameLogic.minTimeRotate,
            gameManager.GameLogic.maxTimeRotate);
        
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
        ChangeRateAmount(gameManager.GameLogic.stepAmountDefault);
        ChangeMoneyAmount(-gameManager.GameLogic.stepAmountDefault);
    }
    
    public void DecreaseRateAmountButton()
    {
        ChangeRateAmount(-gameManager.GameLogic.stepAmountDefault);
        ChangeMoneyAmount(gameManager.GameLogic.stepAmountDefault);
    }
    #endregion
}
