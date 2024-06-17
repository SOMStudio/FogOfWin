using System;
using System.Linq;
using UnityEngine;

namespace Data
{
    public partial class GameLogic
    {
        public void MoveWheelCells(Slot points, Slot cells, float[] timeRotate, float[] currentTime,
            bool[] stopWheelState, bool stopRotate, float deltaTime, Action stopRotateAction)
        {
            var countWheels = points.Length;
            var countCells = points[0].Length;

            for (int i = 0; i < countWheels; i++)
            {
                if (stopRotate && stopWheelState[i]) continue;

                currentTime[i] += deltaTime;
                var siftWheel = currentTime[i] / timeRotate[i];

                for (int j = 0; j < countCells - 1; j++)
                {
                    var spriteMove = cells[i, j];

                    spriteMove.transform.position = Vector3.Lerp(points[i, j].transform.position,
                        points[i, j + 1].transform.position, siftWheel);
                }

                if (siftWheel >= 1f)
                {
                    currentTime[i] = .0f;
                    if (stopRotate) stopWheelState[i] = true;

                    ShiftWheelCells(i, cells);

                    if (stopRotate && stopWheelState.All(el => el)) stopRotateAction?.Invoke();
                }
            }
        }

        private void ShiftWheelCells(int numWheel, Slot cells)
        {
            var lastNumber = cells[0].Length - 1;
            var lastSprite = cells[numWheel, lastNumber];

            for (int i = lastNumber; i > 0; i--)
            {
                cells[numWheel, i] = cells[numWheel, i - 1];
            }

            cells[numWheel, 0] = lastSprite;

            InitRandomSprite(cells[numWheel, 0]);
        }
    }
}