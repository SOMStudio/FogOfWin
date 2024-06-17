using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public partial class GameLogic
    {
        public IEnumerator ShowResult(Slot points, int amount, int rateAmount, Dictionary<int, int> result,
            Dictionary<int, Result> resultCells, Action startShowResultAction,
            Action<int, int> changeMoneyAmountResultAction,
            Action<int, int> changeRateAmountResultAction, Action finishShowResultAction)
        {
            startShowResultAction?.Invoke();

            var currentAmount = amount;

            yield return new WaitForSeconds(startDelayBeforeResult);

            foreach (var itemResult in result)
            {
                int numberPicture = itemResult.Key;
                int winAmount = itemResult.Value;

                var resultWithNumber = resultCells[numberPicture];

                ShowSpritesWithResult(points, ref resultWithNumber);

                yield return new WaitForSeconds(timeShowResult);

                HideSpritesWithResult(points, ref resultWithNumber);

                changeMoneyAmountResultAction?.Invoke(currentAmount, currentAmount + winAmount);

                currentAmount += winAmount;

                yield return new WaitForSeconds(finalDelayAfterStepResult);
            }

            changeMoneyAmountResultAction?.Invoke(currentAmount, currentAmount - rateAmount);

            currentAmount -= rateAmount;

            yield return new WaitForSeconds(finalDelayAfterStepResult);

            changeRateAmountResultAction?.Invoke(0, rateAmount);

            finishShowResultAction?.Invoke();
        }

        private void ShowSpritesWithResult(Slot slot, ref Result result)
        {
            foreach (var slotPosition in result.FirstWheel)
            {
                slot[slotPosition.Wheel, slotPosition.Cell].ShowSprite();
            }

            foreach (var slotPosition in result.OtherWheels)
            {
                slot[slotPosition.Wheel, slotPosition.Cell].ShowSprite();
            }
        }

        private void HideSpritesWithResult(Slot slot, ref Result result)
        {
            foreach (var slotPosition in result.FirstWheel)
            {
                slot[slotPosition.Wheel, slotPosition.Cell].HideSprite();
            }

            foreach (var slotPosition in result.OtherWheels)
            {
                slot[slotPosition.Wheel, slotPosition.Cell].HideSprite();
            }
        }
    }
}