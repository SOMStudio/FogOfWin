using UnityEngine;

namespace Data
{
    public partial class GameLogic
    {
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
    }
}