using System;
using Base;
using Data;
using Save;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Data")]
    [SerializeField] private GameLogic gameLogic;

    [Header("Managers")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private SlotManager slotManager;

    private ISaveManager iSaveManager;
    private ISlotManager iSlotManager;

    protected override void Awake()
    {
        base.Awake();

        CheckManagers();
    }

    private void Start()
    {
        if (gameLogic && slotManager && saveManager)
        {
            InitSlotManager();
        }
    }

    private void CheckManagers()
    {
        if (!saveManager)
        {
            saveManager = SaveManager.instance;
            iSaveManager = saveManager;
        }
        if (!slotManager)
        {
            slotManager = SlotManager.instance;
            iSlotManager = slotManager;
        }
    }
    
    private void InitSlotManager()
    {
        int moneyAmount = iSaveManager.GetValue(GameLogic.MoneyAmountKey, gameLogic.moneyAmountDefault);
        int rateAmount = iSaveManager.GetValue(GameLogic.RateAmountKey, gameLogic.rateAmountDefault);
        int stepAmount = iSaveManager.GetValue(GameLogic.StepAmountKey, gameLogic.stepAmountDefault);
        int countCellBuster = iSaveManager.GetValue(GameLogic.CountCellBusterKey, gameLogic.countCellBusterDefault);
        int countLineVerticalBuster = iSaveManager.GetValue(GameLogic.CountLineVerticalBusterKey, gameLogic.countLineVerticalBusterDefault);
        int countLineHorizontalBuster = iSaveManager.GetValue(GameLogic.CountLineHorizontalBusterKey, gameLogic.countLineHorizontalBusterDefault);

        iSlotManager.Init(gameLogic, moneyAmount, rateAmount, stepAmount, countCellBuster, countLineVerticalBuster, countLineHorizontalBuster);
        
        iSlotManager.ChangeMoneyAmountEvent.AddListener(ChangeMoneyAmountListener);
        iSlotManager.ChangeRateAmountEvent.AddListener(ChangeRateAmountListener);
        iSlotManager.ChangeBusterCountEvent.AddListener(ChangeBusterCountListener);
    }

    private void ChangeMoneyAmountListener(int oldAmount, int newAmount, bool afterRotate)
    {
        iSaveManager.SetValue(GameLogic.MoneyAmountKey, newAmount);
    }

    private void ChangeRateAmountListener(int newAmount)
    {
        iSaveManager.SetValue(GameLogic.RateAmountKey, newAmount);
    }
    
    private void ChangeBusterCountListener(TypeBuster typeBuster, int count)
    {
        switch (typeBuster)
        {
            case TypeBuster.Cell:
                iSaveManager.SetValue(GameLogic.CountCellBusterKey, count);
                break;
            case TypeBuster.LineVertical:
                iSaveManager.SetValue(GameLogic.CountLineVerticalBusterKey, count);
                break;
            case TypeBuster.LineHorizontal:
                iSaveManager.SetValue(GameLogic.CountLineHorizontalBusterKey, count);
                break;
        }
    }
}
