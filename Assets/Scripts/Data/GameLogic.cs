using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameLogic", menuName = "ScriptableObjects/GameLogic", order = 1)]
    public partial class GameLogic : ScriptableObject
    {
        public const string MoneyAmountKey = "MoneyAmount";
        public const string RateAmountKey = "RateAmount";
        public const string StepAmountKey = "StepAmount";
        public const string CountCellBusterKey = "CountCellBuster";
        public const string CountLineVerticalBusterKey = "CountLineVerticalBusterKey";
        public const string CountLineHorizontalBusterKey = "CountLineHorizontalBuster";
        public const string CountEnterInGameKey = "CountEnterInGameKey";
        public const string SoundVolumeKey = "FogOfWin_SFXVol";
        public const string MusicVolumeKey = "FogOfWin_MusicVol";

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

        [Header("Settings")]
        public float soundVolumeDefault = 0.5f;
        public float musicVolumeDefault;

        [Header("Lucky Box")]
        public int showIfAmountMoneyLower = 2000;
        public int showIfCountEnter = 10;
        public int minAmountMoneyLuckyBox = 3000;
        public int maxAmountMoneyLuckyBox = 9000;
        public int minCountBusterLuckyBox = 3;
        public int maxCountBusterLuckyBox = 8;

        [Header("Console Panel")]
        public int countClickForOpenConsolePanel = 10;
    }
}