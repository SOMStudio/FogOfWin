using System;
using Data;
using Save;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

                box.rewardText.text = GetLuckyBoxText(box);
                
                box.closeBoxImage.gameObject.SetActive(true);
                box.openBoxImage.gameObject.SetActive(false);
                box.rewardText.gameObject.SetActive(false);
            }
        }

        public static string GetLuckyBoxText(LuckyBoxItem luckyBox)
        {
            string resultString = "";
            if (luckyBox.typeReward == TypeReward.Money)
            {
                resultString += luckyBox.amount + " $";
            }
            else
            {
                switch (luckyBox.typeBuster)
                {
                    case TypeBuster.Cell:
                        resultString += luckyBox.amount + " Cell Busters";
                        break;
                    case TypeBuster.LineHorizontal:
                        resultString += luckyBox.amount + " Horizontal line Cell Busters";
                        break;
                    case TypeBuster.LineVertical:
                        resultString += luckyBox.amount + " Vertical line Cell Busters";
                        break;
                }
            }

            return resultString;
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

            luckyBox.closeBoxImage.gameObject.SetActive(false);
            luckyBox.openBoxImage.gameObject.SetActive(true);
            luckyBox.rewardText.gameObject.SetActive(true);
            
            selectRewardEvent?.Invoke(luckyBox);
        }

        public void ShowOtherLuckyBox(LuckyBoxItem luckyBox)
        {
            foreach (var box in boxes)
            {
                if (box == luckyBox) continue;
                
                box.closeBoxImage.gameObject.SetActive(false);
                box.openBoxImage.gameObject.SetActive(true);
                box.rewardText.gameObject.SetActive(true);
            }
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
        [Header("Logic")]
        public TypeReward typeReward;
        public TypeBuster typeBuster;
        public int amount;

        [Header("Visual")]
        public Image closeBoxImage;
        public Image openBoxImage;
        public Text rewardText;
    }
    
    public enum TypeReward
    {
        Money,
        Buster
    }
}
