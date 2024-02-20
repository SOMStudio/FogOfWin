using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private CanvasGroup canvasGrope;
    
    [Header("HUD panel")]
    [SerializeField] private Text moneyAmountText;
    [SerializeField] private Button menuButton;

    [Header("Type panel")]
    [SerializeField] private Image typeGame1Panel;
    [SerializeField] private Image typeGame2Panel;
    [SerializeField] private Image typeGame3Panel;
    [SerializeField] private Button typeGame1Button;
    [SerializeField] private Button typeGame2Button;
    [SerializeField] private Button typeGame3Button;

    [Header("Buster panel")]
    [SerializeField] private Button buster1Button;
    [SerializeField] private Button buster2Button;
    [SerializeField] private Button buster3Button;

    [Header("Spin panel")]
    [SerializeField] private Text rateAmountText;
    [SerializeField] private Button reduceRateButton;
    [SerializeField] private Button increaseRateButton;
    [SerializeField] private Button spinButton;

    private void ActivateTypeGame(TypeGame typeGame)
    {
        var defaultColor = typeGame1Panel.color;

        switch (typeGame)
        {
            case TypeGame.Count:
                typeGame1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                typeGame2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeGame3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                break;
            case TypeGame.Near:
                typeGame1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeGame2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                typeGame3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                break;
            case TypeGame.Line:
                typeGame1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeGame2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeGame3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                break;
        }
    }
    
    #region Listeners
    public void ActivateSpinListener()
    {
        canvasGrope.interactable = false;
    }
    
    public void DeactivateSpinListener()
    {
        canvasGrope.interactable = true;
    }

    public void ChangeTypeGameListener(TypeGame typeGame)
    {
        ActivateTypeGame(typeGame);
    }

    public void ApplyBusterListener()
    {
        
    }

    public void ChangeRateAmountListener(int amount)
    {
        rateAmountText.text = amount + "$";
    }

    public void ChangeMoneyAmountListener(int oldAmount, int newAmount, bool afterRotate)
    {
        if (!afterRotate) moneyAmountText.text = newAmount + "$";
    }

    public void ChangeRateAmountAnimatedListener(int oldAmount, int newAmount)
    {
        rateAmountText.text = newAmount + "$";
    }
    
    public void ChangeMoneyAmountAnimatedListener(int oldAmount, int newAmount)
    {
        moneyAmountText.text = newAmount + "$";
    }
    #endregion
}
