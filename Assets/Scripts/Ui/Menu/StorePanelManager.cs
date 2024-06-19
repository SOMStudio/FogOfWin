using System.Collections;
using Components.UI;
using Data;
using Save;
using Sound;
using UnityEngine;

namespace Ui.Menu
{
    public class StorePanelManager : CanvasGroupComponent
    {
        [Header("Store")]
        public StoreItemManager[] items;

        private ISaveManager saveManager;
        private ISoundManager soundManager;

        public void Init(ISaveManager saveManagerSet, ISoundManager soundManagerSet)
        {
            saveManager = saveManagerSet;
            soundManager = soundManagerSet;
            
            UpdateStoreValues(GameLogic.MoneyAmountKey);
            UpdateStoreValues(GameLogic.CountCellBusterKey);
            UpdateStoreValues(GameLogic.CountLineHorizontalBusterKey);
            UpdateStoreValues(GameLogic.CountLineVerticalBusterKey);
        }

        public void UpdateStoreValues(string keyUpdate)
        {
            switch (keyUpdate)
            {
                case GameLogic.MoneyAmountKey:
                    StartCoroutine(UpdateAmountCoroutine());
                    break;
                case GameLogic.CountCellBusterKey:
                    StartCoroutine(UpdateBusterCoroutine(BusterType.Cell,
                        saveManager.GetValueInt(GameLogic.CountCellBusterKey)));
                    break;
                case GameLogic.CountLineHorizontalBusterKey:
                    StartCoroutine(UpdateBusterCoroutine(BusterType.LineHorizontal,
                        saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey)));
                    break;
                case GameLogic.CountLineVerticalBusterKey:
                    StartCoroutine(UpdateBusterCoroutine(BusterType.LineVertical,
                        saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey)));
                    break;
            }
        }

        private IEnumerator UpdateAmountCoroutine()
        {
            int amount = saveManager.GetValueInt(GameLogic.MoneyAmountKey);
            
            foreach (var item in items)
            {
                item.UpdateAmount(amount);
                yield return null;
            }
        }
        
        private IEnumerator UpdateBusterCoroutine(BusterType busterType, int count)
        {
            foreach (var item in items)
            {
                if (item.busterType == busterType) item.UpdateCount(count);
                yield return null;
            }
        }

        public void ClickItem(StoreItemManager itemClick)
        {
            var amount = saveManager.GetValueInt(GameLogic.MoneyAmountKey);
            if (amount < itemClick.costBuster)
            {
                soundManager.PlaySoundByIndex(5);
                return;
            }
            soundManager.PlaySoundByIndex(4);
            
            saveManager.SetValue(GameLogic.MoneyAmountKey, amount - itemClick.costBuster);

            switch (itemClick.busterType)
            {
                case BusterType.Cell:
                {
                    var countBuster = saveManager.GetValueInt(GameLogic.CountCellBusterKey);
                    saveManager.SetValue(GameLogic.CountCellBusterKey, countBuster + itemClick.countBuster);
                    break;
                }
                case BusterType.LineHorizontal:
                {
                    var countBuster = saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey);
                    saveManager.SetValue(GameLogic.CountLineHorizontalBusterKey, countBuster + itemClick.countBuster);
                    break;
                }
                case BusterType.LineVertical:
                {
                    var countBuster = saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey);
                    saveManager.SetValue(GameLogic.CountLineVerticalBusterKey, countBuster + itemClick.countBuster);
                    break;
                }
            }
        }
    }
}
