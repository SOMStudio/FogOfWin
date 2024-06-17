using Save;

namespace Data
{
    public partial class GameLogic
    {
        public bool NeedShowLuckyBoxWithCountShow(ISaveManager saveManager)
        {
            int countShowCheck = saveManager.GetValueInt(CountEnterInGameKey) % showIfCountEnter;
            return countShowCheck == 0;
        }

        public bool NeedShowLuckyBoxWithAmount(ISaveManager saveManager)
        {
            int amountCheck = saveManager.GetValueInt(MoneyAmountKey);
            return amountCheck < showIfAmountMoneyLower;
        }
    }
}