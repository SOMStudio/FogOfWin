using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    [CreateAssetMenu(fileName = "GameLogic", menuName = "ScriptableObjects/GameLogic", order = 1)]
    public class GameLogic : ScriptableObject
    {
        public const string MoneyAmountKey = "MoneyAmount";
        public const string RateAmountKey = "RateAmount";
        public const string StepAmountKey = "StepAmount";
        public const string CountCellBusterKey = "CountCellBuster";
        public const string CountLineVerticalBusterKey = "CountLineVerticalBusterKey";
        public const string CountLineHorizontalBusterKey = "CountLineHorizontalBuster";
    
        [Header("Main")]
        public WheelCell slotCellPrefab;
        public Sprite[] cellSprites;
    
        [Header("Money")]
        public int moneyAmountDefault = 10000;
        public int rateAmountDefault = 100;
        public int stepAmountDefault = 25;
    
        [Header("Busters")]
        public int countCellBusterDefault = 20;
        public int countLineVerticalBusterDefault = 10;
        public int countLineHorizontalBusterDefault = 10;

        [Header("Limits")]
        public float minSpeedWheelRotate = 0.07f;
        public float maxSpeedWheelRotate = 0.17f;
        public float minTimeRotate = 0.3f;
        public float maxTimeRotate = 0.5f;

        [Header("Reward")]
        public float coefficientFirstForCountNumberType = 0.1f;
        public float coefficientOtherForCountNumberType = 0.5f;
        public float coefficientFirstForNearNumberType = 0.1f;
        public float coefficientOtherForNearNumberType = 0.7f;
        public float coefficientFirstForLineNumberType = 0.1f;
        public float coefficientOtherForLineNumberType = 0.9f;

        [Header("Result")]
        public float startDelayBeforeResult = 0.5f;
        public float timeShowResult = 2f;
        public float finalDelayAfterStepResult = 0.5f;
    
        [Header("Cover")]
        public Color colorCoverCell;
        public Color colorSelectCoverCell;

        #region Init
        public void InitGameState(Slot positions, Slot cells)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                for (int j = 0; j < positions[0].Length; j++)
                {
                    InitRandomSprite(cells[i, j]);
                    cells[i, j].transform.position = positions[i, j].transform.position;
                }
            }
        }

        private void InitRandomSprite(WheelCell cell)
        {
            var randomSpriteNumber = GetRandomSpriteNumber();

            cell.SetSprite(randomSpriteNumber, GetSpriteWithNumber(randomSpriteNumber));
        }

        private int GetRandomSpriteNumber()
        {
            return Random.Range(0, cellSprites.Length);
        }

        private Sprite GetSpriteWithNumber(int numberSprite)
        {
            return cellSprites[numberSprite];
        }

        public float GetRandomRange(float nimValue, float maxValue)
        {
            return Random.Range(nimValue, maxValue);
        }
    
        public void InitRandomSpeedRotateWheel(float[] wheelSpeeds)
        {
            for (int i = 0; i < wheelSpeeds.Length; i++)
            {
                wheelSpeeds[i] = GetRandomRange(minSpeedWheelRotate, maxSpeedWheelRotate);
            }
        }
        #endregion
    
        #region Spin
        public void MoveWheelCells(Slot points, Slot cells, float[] timeRotate, float[] currentTime, bool stopRotate, Action stopRotateAction, float deltaTime)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (stopRotate && currentTime[i] == 0f) continue;
            
                currentTime[i] += deltaTime;
            
                for (int j = 0; j < points[0].Length - 1; j++)
                {
                    var spriteMove = cells[i, j];

                    spriteMove.transform.position = Vector3.Lerp(points[i, j].transform.position,
                        points[i, j + 1].transform.position, currentTime[i] / timeRotate[i]);
                }

                if (currentTime[i] / timeRotate[i] >= 1)
                {
                    currentTime[i] = 0f;
                
                    ShiftWheelCells(i, cells);
                
                    if (stopRotate && currentTime.All(el => el == 0f)) stopRotateAction?.Invoke();
                }
            }
        }

        private void ShiftWheelCells(int numWheel, Slot cells)
        {
            var lastSprite = cells[numWheel, cells[0].Length - 1];

            for (int i = cells[0].Length - 1; i > 0; i--)
            {
                cells[numWheel, i] = cells[numWheel, i - 1];
            }

            cells[numWheel, 0] = lastSprite;
            cells[numWheel, 0].transform.position = cells[numWheel, 0].transform.position;
        
            InitRandomSprite(cells[numWheel, 0]);
        }
        #endregion

        #region Reward
        public Dictionary<int, int> CountRewardResult(int price, Slot cells, TypeGame type, out Dictionary<int, Result> resultCells)
        {
            Dictionary<int, int> resultRewards;
        
            switch (type)
            {
                case TypeGame.Line:
                    resultRewards = CalculateResultLineNumbers(price, cells, out resultCells);
                    break;
                case TypeGame.Near:
                    resultRewards = CalculateResultNearNumbers(price, cells, out resultCells);
                    break;
                default:
                    resultRewards = CalculateResultCountNumbers(price, cells, out resultCells);
                    break;
            }
        
            return resultRewards;
        }

        private Dictionary<int, int> CalculateReward(int price, Dictionary<int, Result> resultCells, float coefficientForFirstWheel, float coefficientForOtherWheels)
        {
            var resultRewards = new Dictionary<int, int>();
        
            for (int i = 0; i < resultCells.Keys.Count; i++)
            {
                var key = resultCells.Keys.ToList()[i];
            
                if (resultCells[key].OtherWheels.Count == 0) continue;

                var reward = price * (resultCells[key].OtherWheels.Count * coefficientForOtherWheels +
                                      (resultCells[key].FirstWheel.Count - 1) * coefficientForFirstWheel);
            
                resultRewards.Add(key, (int)reward);
            }

            return resultRewards;
        }
    
        private Dictionary<int, Result> FillFirstWheelNumbers(Slot cells)
        {
            var result = new Dictionary<int, Result>();
            
            var firstWheel = cells[0];
        
            for (int i = 1; i < firstWheel.Length - 1; i++)
            {
                if (!result.Keys.Contains(firstWheel[i].Number))
                {
                    var newResult = new Result()
                    {
                        NumberPicture = firstWheel[i].Number,
                        FirstWheel = new List<SlotPosition>() {new SlotPosition() {Wheel = 0, Cell = i}},
                        OtherWheels = new List<SlotPosition>()
                    };
                    result.Add(firstWheel[i].Number, newResult);
                }
                else
                {
                    result[firstWheel[i].Number].FirstWheel.Add(new SlotPosition(){Wheel = 0, Cell = i});
                }
            }

            return result;
        }
    
        private Dictionary<int, int> CalculateResultCountNumbers(int price, Slot cells, out Dictionary<int, Result> resultCells)
        {
            resultCells = FillFirstWheelNumbers(cells);

            for (int i = 1; i < cells.Length; i++)
            {
                for (int j = 1; j < cells[i].Length - 1; j++)
                {
                    if (resultCells.Keys.Contains(cells[i, j].Number))
                    {
                        resultCells[cells[i, j].Number].OtherWheels.Add(new SlotPosition(){Wheel = i, Cell = j});
                    }
                }
            }

            return CalculateReward(price, resultCells, coefficientFirstForCountNumberType, coefficientOtherForCountNumberType);
        }

        private Dictionary<int, int> CalculateResultNearNumbers(int price, Slot cells, out Dictionary<int, Result> resultCells)
        {
            resultCells = FillFirstWheelNumbers(cells);
        
            foreach (var checkNumber in resultCells.Keys)
            {
                var checkField = new int[cells.Length, cells[0].Length - 2];
            
                for (int i = 0; i < cells.Length; i++)
                {
                    for (int j = 1; j < cells[i].Length - 1; j++)
                    {
                        checkField[i, j - 1] = cells[i, j].Number == resultCells[checkNumber].NumberPicture ? 0 : -1;
                    }
                }

                for (int i = 0; i < resultCells[checkNumber].FirstWheel.Count; i++)
                {
                    var firstPoint = resultCells[checkNumber].FirstWheel[i];
                    var convertInCheckField = new SlotPosition() { Wheel = firstPoint.Wheel, Cell = firstPoint.Cell - 1 };

                    if (checkField[convertInCheckField.Wheel, convertInCheckField.Cell] == 0)
                        MarkRoundStep(checkField, convertInCheckField,  new List<SlotPosition> {convertInCheckField});
                }
            
                var slotPositionList = GetSlotPositionList(checkField);

                for (int i = 0; i < slotPositionList.Count; i++)
                {
                    resultCells[checkNumber].OtherWheels.Add(slotPositionList[i]);
                }
            }
        
            return CalculateReward(price, resultCells, coefficientFirstForNearNumberType, coefficientOtherForNearNumberType);
        }

        private void MarkRoundStep(int[,] checkField, SlotPosition position, List<SlotPosition> ignore = null)
        {
            var numberPosition = checkField[position.Wheel, position.Cell];

            int[] shiftPosition = new[] { -1, 0, 1 };

            foreach (var shiftWheel in shiftPosition)
            {
                foreach (var shiftCell in shiftPosition)
                {
                    var checkPosition = position + new int[] { shiftWheel, shiftCell };

                    if (ignore != null && ignore.Contains(checkPosition)) continue;
                
                    if (checkPosition != position && checkPosition.Wheel >= 0 &&
                        checkPosition.Wheel <= checkField.GetLength(0) - 1 && checkPosition.Cell >= 0 &&
                        checkPosition.Cell <= checkField.GetLength(1) - 1)
                    {
                        if (checkField[checkPosition.Wheel, checkPosition.Cell] == 0)
                            checkField[checkPosition.Wheel, checkPosition.Cell] = numberPosition + 1;
                    }
                }
            }

            foreach (var shiftWheel in shiftPosition)
            {
                foreach (var shiftCell in shiftPosition)
                {
                    var checkPosition = position + new int[] { shiftWheel, shiftCell };

                    if (checkPosition != position && checkPosition.Wheel >= 0 &&
                        checkPosition.Wheel <= checkField.GetLength(0) - 1 && checkPosition.Cell >= 0 &&
                        checkPosition.Cell <= checkField.GetLength(1) - 1)
                    {
                        if (checkField[checkPosition.Wheel, checkPosition.Cell] == numberPosition + 1)
                            MarkRoundStep(checkField, checkPosition, ignore);
                    }
                }
            }
        }

        private List<SlotPosition> GetSlotPositionList(int[,] checkField)
        {
            var result = new List<SlotPosition>();

            for (int i = 0; i < checkField.GetLength(0); i++)
            {
                for (int j = 0; j < checkField.GetLength(1); j++)
                {
                    if (checkField[i, j] > 0) result.Add(new SlotPosition(){Wheel = i, Cell = j + 1});
                }
            }
        
            return result;
        }

        private Dictionary<int, int> CalculateResultLineNumbers(int price, Slot cells, out Dictionary<int, Result> resultCells)
        {
            resultCells = FillFirstWheelNumbers(cells);

            foreach (var numberPicture in resultCells.Keys)
            {
                foreach (var slotPosition in resultCells[numberPicture].FirstWheel)
                {
                    var checkPosition = new SlotPosition() { Wheel = slotPosition.Wheel, Cell = slotPosition.Cell };
                    var shiftPosition = new SlotPosition() { Wheel = 1, Cell = 0};
                    checkPosition += shiftPosition;
                
                    while (checkPosition.Wheel < cells.Length && cells[checkPosition.Wheel, checkPosition.Cell].Number == numberPicture)
                    {
                        resultCells[numberPicture].OtherWheels.Add(checkPosition);
                        checkPosition += shiftPosition;
                    }
                }
            }

            return CalculateReward(price, resultCells, coefficientFirstForLineNumberType, coefficientOtherForLineNumberType);;
        }
        #endregion

        #region Result
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
        #endregion
    
        #region Select
        public List<WheelCell> SelectSlotCellList(Slot slot, TypeBuster typeBusterSet, GameLogic.SlotPosition selectSlotPosition)
        {
            switch (typeBusterSet)
            {
                case TypeBuster.LineHorizontal:
                    return slot.wheels.Select(slotWheel => slotWheel[selectSlotPosition.Cell]).ToList();
                case TypeBuster.LineVertical:
                    return slot[selectSlotPosition.Wheel].places.ToList();
                default:
                    return new List<WheelCell> {slot[selectSlotPosition.Wheel, selectSlotPosition.Cell]};
            }
        }
        #endregion
    
        public struct Result
        {
            public int NumberPicture;
            public List<SlotPosition> FirstWheel;
            public List<SlotPosition> OtherWheels;
        }
    
        public struct SlotPosition
        {
            public int Wheel;
            public int Cell;

            public static bool operator ==(SlotPosition check1, SlotPosition check2)
            {
                return check1.Wheel == check2.Wheel && check1.Cell == check2.Cell;
            }

            public static bool operator !=(SlotPosition check1, SlotPosition check2)
            {
                return !(check1 == check2);
            }
        
            public static SlotPosition operator +(SlotPosition addTo, SlotPosition addPosition)
            {
                return new SlotPosition() { Wheel = addTo.Wheel + addPosition.Wheel, Cell = addTo.Cell + addPosition.Cell };
            }

            public static SlotPosition operator +(SlotPosition addTo, int[] addPosition)
            {
                return new SlotPosition() { Wheel = addTo.Wheel + addPosition[0], Cell = addTo.Cell + addPosition[1] };
            }
        }
    }

    [Serializable]
    public class Slot
    {
        public SlotWheel[] wheels;
    
        public Slot(int countWheel, int countPoint)
        {
            wheels = new SlotWheel[countWheel];

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i] = new SlotWheel(countPoint);
            }
        }
    
        public int Length => wheels.Length;
    
        public SlotWheel this[int w]
        {
            get => wheels[w];
            set => wheels[w] = value;
        }
    
        public WheelCell this[int w, int p]
        {
            get => wheels[w][p];
            set => wheels[w][p] = value;
        }
    }

    [Serializable]
    public class SlotWheel
    {
        public WheelCell[] places;
    
        public SlotWheel(int countPlaces)
        {
            places = new WheelCell[countPlaces];
        }

        public int Length => places.Length;

        public WheelCell this[int p]
        {
            get => places[p];
            set => places[p] = value;
        }
    }

    public enum TypeGame
    {
        Count,
        Near,
        Line
    }

    public enum TypeBuster
    {
        Cell,
        LineHorizontal,
        LineVertical
    }
}