using System;
using Data;
using Save;
using UnityEngine;
using UnityEngine.Events;

namespace Ui.Menu
{
    public class LuckyBoxPanelManager : MonoBehaviour
    {
        [Header("Main")]
        public LuckyBoxItem[] boxes;

        [Header("Events")]
        public UnityEvent<LuckyBoxItem> selectRewardEvent;

        private GameLogic gameLogic;
        private ISaveManager saveManager;
        
        public void Init(GameLogic gameLogicSet, ISaveManager saveManagerSet)
        {
            gameLogic = gameLogicSet;
            saveManager = saveManagerSet;

            Generate();
        }
        
        public void Generate()
        {
            foreach (var box in boxes)
            {
                box.typeReward = (TypeReward)UnityEngine.Random.Range(0, 2);
                box.typeBuster = TypeBuster.Cell;
                box.amount = 0;

                switch (box.typeReward)
                {
                    case TypeReward.Money:
                        box.amount = UnityEngine.Random.Range(gameLogic.minAmountMoneyLuckyBox,
                            gameLogic.maxAmountMoneyLuckyBox);
                        break;
                    case TypeReward.Buster:
                        box.typeBuster = (TypeBuster)UnityEngine.Random.Range(0, 3);
                        box.amount = UnityEngine.Random.Range(gameLogic.minCountBusterLuckyBox,
                            gameLogic.maxCountBusterLuckyBox);
                        break;
                }
            }
        }
        
        private void OpenLuckyBox(LuckyBoxItem luckyBox)
        {
            if (luckyBox.typeReward == TypeReward.Money)
            {
                var moneyAmount = saveManager.GetValueInt(GameLogic.MoneyAmountKey);
                saveManager.SetValue(GameLogic.MoneyAmountKey, moneyAmount + luckyBox.amount);
            }
            else
            {
                switch (luckyBox.typeBuster)
                {
                    case TypeBuster.Cell:
                    {
                        var countBuster = saveManager.GetValueInt(GameLogic.CountCellBusterKey);
                        saveManager.SetValue(GameLogic.CountCellBusterKey, countBuster + luckyBox.amount);
                        break;
                    }
                    case TypeBuster.LineHorizontal:
                    {
                        var countBuster = saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey);
                        saveManager.SetValue(GameLogic.CountLineHorizontalBusterKey, countBuster + luckyBox.amount);
                        break;
                    }
                    case TypeBuster.LineVertical:
                    {
                        var countBuster = saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey);
                        saveManager.SetValue(GameLogic.CountLineVerticalBusterKey, countBuster + luckyBox.amount);
                        break;
                    }
                }
            }
            
            selectRewardEvent?.Invoke(luckyBox);
        }
        
        public void OpenLuckyBox1()
        {
            OpenLuckyBox(boxes[0]);
        }
        
        public void OpenLuckyBox2()
        {
            OpenLuckyBox(boxes[1]);
        }
        
        public void OpenLuckyBox3()
        {
            OpenLuckyBox(boxes[2]);
        }
    }

    [Serializable]
    public class LuckyBoxItem
    {
        public TypeReward typeReward;
        public TypeBuster typeBuster;
        public int amount;
    }
    
    public enum TypeReward
    {
        Money,
        Buster
    }
}
