using Components.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui.Game
{
    public class ScorePanelManager : CanvasGroupComponent
    {
        [Header("Score panel")]
        [SerializeField] private Text moneyAmountText;
        [SerializeField] private Button menuButton;

        public UnityEvent MenuButtonClickEvent => menuButton.onClick;

        public void ChangeMoneyAmountListener(int oldAmount, int newAmount, bool afterRotate)
        {
            if (!afterRotate) moneyAmountText.text = newAmount + "$";
        }
        
        public void ChangeMoneyAmountAnimatedListener(int oldAmount, int newAmount)
        {
            moneyAmountText.text = newAmount + "$";
        }
    }
}
