using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public partial class GameLogic
    {
        public Dictionary<int, int> CountRewardResult(int price, Slot cells, GameType gameType,
            out Dictionary<int, Result> resultCells)
        {
            Dictionary<int, int> resultRewards;

            switch (gameType)
            {
                case GameType.Line:
                    resultRewards = CalculateResultLineNumbers(price, cells, out resultCells);
                    break;
                case GameType.Near:
                    resultRewards = CalculateResultNearNumbers(price, cells, out resultCells);
                    break;
                default:
                    resultRewards = CalculateResultCountNumbers(price, cells, out resultCells);
                    break;
            }

            return resultRewards;
        }

        private Dictionary<int, int> CalculateReward(int price, Dictionary<int, Result> resultCells,
            float coefficientForFirstWheel, float coefficientForOtherWheels)
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
                        FirstWheel = new List<SlotPosition>() { new SlotPosition() { Wheel = 0, Cell = i } },
                        OtherWheels = new List<SlotPosition>()
                    };
                    result.Add(firstWheel[i].Number, newResult);
                }
                else
                {
                    result[firstWheel[i].Number].FirstWheel.Add(new SlotPosition() { Wheel = 0, Cell = i });
                }
            }

            return result;
        }

        private Dictionary<int, int> CalculateResultCountNumbers(int price, Slot cells,
            out Dictionary<int, Result> resultCells)
        {
            resultCells = FillFirstWheelNumbers(cells);

            for (int i = 1; i < cells.Length; i++)
            {
                for (int j = 1; j < cells[i].Length - 1; j++)
                {
                    if (resultCells.Keys.Contains(cells[i, j].Number))
                    {
                        resultCells[cells[i, j].Number].OtherWheels.Add(new SlotPosition() { Wheel = i, Cell = j });
                    }
                }
            }

            return CalculateReward(price, resultCells, coefficientFirstForCountNumberType,
                coefficientOtherForCountNumberType);
        }

        private Dictionary<int, int> CalculateResultNearNumbers(int price, Slot cells,
            out Dictionary<int, Result> resultCells)
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

                foreach (var firstPoint in resultCells[checkNumber].FirstWheel)
                {
                    var convertInCheckField = new SlotPosition()
                        { Wheel = firstPoint.Wheel, Cell = firstPoint.Cell - 1 };

                    if (checkField[convertInCheckField.Wheel, convertInCheckField.Cell] == 0)
                        MarkRoundStep(checkField, convertInCheckField, new List<SlotPosition> { convertInCheckField });
                }

                var slotPositionList = GetSlotPositionList(checkField);

                foreach (var t in slotPositionList)
                {
                    resultCells[checkNumber].OtherWheels.Add(t);
                }
            }

            return CalculateReward(price, resultCells, coefficientFirstForNearNumberType,
                coefficientOtherForNearNumberType);
        }

        private void MarkRoundStep(int[,] checkField, SlotPosition position, List<SlotPosition> ignore = null)
        {
            var numberPosition = checkField[position.Wheel, position.Cell];

            int[] shiftPosition = new[] { -1, 0, 1 };

            foreach (var shiftWheel in shiftPosition)
            {
                foreach (var shiftCell in shiftPosition)
                {
                    var checkPosition = position + new[] { shiftWheel, shiftCell };

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
                    var checkPosition = position + new[] { shiftWheel, shiftCell };

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
                    if (checkField[i, j] > 0) result.Add(new SlotPosition() { Wheel = i, Cell = j + 1 });
                }
            }

            return result;
        }

        private Dictionary<int, int> CalculateResultLineNumbers(int price, Slot cells,
            out Dictionary<int, Result> resultCells)
        {
            resultCells = FillFirstWheelNumbers(cells);

            foreach (var numberPicture in resultCells.Keys)
            {
                foreach (var slotPosition in resultCells[numberPicture].FirstWheel)
                {
                    var checkPosition = new SlotPosition() { Wheel = slotPosition.Wheel, Cell = slotPosition.Cell };
                    var shiftPosition = new SlotPosition() { Wheel = 1, Cell = 0 };
                    checkPosition += shiftPosition;

                    while (checkPosition.Wheel < cells.Length &&
                           cells[checkPosition.Wheel, checkPosition.Cell].Number == numberPicture)
                    {
                        resultCells[numberPicture].OtherWheels.Add(checkPosition);
                        checkPosition += shiftPosition;
                    }
                }
            }

            return CalculateReward(price, resultCells, coefficientFirstForLineNumberType,
                coefficientOtherForLineNumberType);
        }
    }
}