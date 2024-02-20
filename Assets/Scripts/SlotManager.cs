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
    
    [Header("Slot")]
    [SerializeField] private Slot slotPoints;
    [SerializeField] private Slot slotCells;
    [SerializeField] private Slot slotCovers;

    private float[] _timeRotate;
    private float[] _currentTime;
    private bool _stopRotate = true;
    
    private GameManager _gameManager;

    [Header("Events")]
    public UnityEvent startSlotRotateEvent;
    public UnityEvent stopSlotRotateEvent;
    public UnityEvent<TypeGame> changeTypeGameEvent;
    public UnityEvent<int> changeRateAmountEvent;
    public UnityEvent<int, int, bool> changeMoneyAmountEvent;
    public UnityEvent<Dictionary<int, int>, Dictionary<int, GameLogic.Result>> winRateDetailAmountEvent;

    [Header("Events for Animation")]
    public UnityEvent startShowResultEvent;
    public UnityEvent<int, int> changeRateAmountResultEvent;
    public UnityEvent<int, int> changeMoneyAmountResultEvent;
    public UnityEvent finishShowResultEvent;

    private void Start()
    {
        _gameManager = (GameManager)GameManager.instance;

        rateAmount = _gameManager.GameLogic.rateAmountDefault;
        
        _timeRotate = new float[slotPoints.Length];
        _currentTime = new float[slotPoints.Length];
            
        _gameManager.GameLogic.InitGameState(slotPoints, slotCells);
        
        stopSlotRotateEvent.AddListener(CheckResult);
        
        changeMoneyAmountEvent?.Invoke(moneyAmount, moneyAmount, false);
    }

    private void Update()
    {
        _gameManager.GameLogic.MoveWheelCells(slotPoints, slotCells, _timeRotate, _currentTime, _stopRotate,
            () => stopSlotRotateEvent?.Invoke(), Time.deltaTime);
    }

    private void CheckResult()
    {
        var resultCells = new Dictionary<int, GameLogic.Result>();
        
        var result = _gameManager.GameLogic.CountRewardResult(rateAmount, slotCells, typeGame, out resultCells);

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
        
        yield return new WaitForSeconds(_gameManager.GameLogic.startDelayBeforeResult);
        
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

            yield return new WaitForSeconds(_gameManager.GameLogic.timeShowResult);
            
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
            
            yield return new WaitForSeconds(_gameManager.GameLogic.finalDelayAfterStepResult);
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

    private void StartStopSlotRotate()
    {
        if (_stopRotate)
        {
            _gameManager.GameLogic.InitRandomSpeedRotateWheel(_timeRotate);
            
            startSlotRotateEvent?.Invoke();
            
            changeRateAmountResultEvent?.Invoke(rateAmount, 0);
        }
        
        _stopRotate = !_stopRotate;
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

    #region UI
    public void RotateSlotButton()
    {
        if (!_stopRotate) return;
            
        StartStopSlotRotate();
        
        var randomTimeRotate = _gameManager.GameLogic.GetRandomRange(_gameManager.GameLogic.minTimeRotate,
            _gameManager.GameLogic.maxTimeRotate);
        
        Invoke(nameof(StartStopSlotRotate), randomTimeRotate);
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

    public void IncreaseRateAmountButton()
    {
        ChangeRateAmount(_gameManager.GameLogic.stepAmountDefault);
        ChangeMoneyAmount(-_gameManager.GameLogic.stepAmountDefault);
    }
    
    public void DecreaseRateAmountButton()
    {
        ChangeRateAmount(-_gameManager.GameLogic.stepAmountDefault);
        ChangeMoneyAmount(_gameManager.GameLogic.stepAmountDefault);
    }
    #endregion
}
