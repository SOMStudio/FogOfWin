using Components.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class SpinPanelManager : CanvasGroupComponent
    {
        [Header("Spin panel")]
        [SerializeField] private Text rateAmountText;
        [SerializeField] private Button reduceRateButton;
        [SerializeField] private Button increaseRateButton;
        [SerializeField] private GameObject spinPanel;
        [SerializeField] private GameObject resultPanel;
        
        public void ShowSpinButton(bool setState)
        {
            spinPanel.SetActive(setState);
        }

        public void ShowResultButton(bool setState)
        {
            resultPanel.SetActive(setState);
        }
        
        public void ChangeRateAmountListener(int amount)
        {
            rateAmountText.text = amount + "$";
        }
        
        public void ChangeRateAmountAnimatedListener(int oldAmount, int newAmount)
        {
            rateAmountText.text = newAmount + "$";
        }
    }
}
