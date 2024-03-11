using Data;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private CanvasGroup mainCanvasGrope;
    
    [Header("Score panel")]
    [SerializeField] private CanvasGroup moneyCanvasGrope;
    [SerializeField] private Text moneyAmountText;
    [SerializeField] private Button menuButton;

    [Header("Type panel")]
    [SerializeField] private CanvasGroup typeCanvasGrope;
    [SerializeField] private Image typeGame1Panel;
    [SerializeField] private Image typeGame2Panel;
    [SerializeField] private Image typeGame3Panel;
    [SerializeField] private Button typeGame1Button;
    [SerializeField] private Button typeGame2Button;
    [SerializeField] private Button typeGame3Button;

    [Header("Buster panel")]
    [SerializeField] private CanvasGroup busterCanvasGrope;
    [SerializeField] private Image typeBuster1Panel;
    [SerializeField] private Image typeBuster2Panel;
    [SerializeField] private Image typeBuster3Panel;
    [SerializeField] private Button buster1Button;
    [SerializeField] private Button buster2Button;
    [SerializeField] private Button buster3Button;
    [SerializeField] private Text buster1CountText;
    [SerializeField] private Text buster2CountText;
    [SerializeField] private Text buster3CountText;

    [Header("Spin panel")]
    [SerializeField] private CanvasGroup spinCanvasGrope;
    [SerializeField] private Text rateAmountText;
    [SerializeField] private Button reduceRateButton;
    [SerializeField] private Button increaseRateButton;
    [SerializeField] private GameObject spinPanel;
    [SerializeField] private GameObject resultPanel;

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
    
    private void ActivateTypeBuster(TypeBuster typeBuster)
    {
        var defaultColor = typeBuster1Panel.color;

        switch (typeBuster)
        {
            case TypeBuster.Cell:
                typeBuster1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                typeBuster2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeBuster3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                break;
            case TypeBuster.LineHorizontal:
                typeBuster1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeBuster2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                typeBuster3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                break;
            case TypeBuster.LineVertical:
                typeBuster1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeBuster2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                typeBuster3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                break;
        }
    }

    private void UpdateTypeBusterCount(TypeBuster typeBuster, int countSet)
    {
        if (typeBuster == TypeBuster.LineHorizontal)
        {
            buster2CountText.text = countSet.ToString();
        } else if (typeBuster == TypeBuster.LineVertical)
        {
            buster3CountText.text = countSet.ToString();
        }
        else if (typeBuster == TypeBuster.Cell)
        {
            buster1CountText.text = countSet.ToString();
        }
    }
    
    #region Actions
    public void ActivateAllCanvas(bool setState)
    {
        ActivateMoneyPanel(setState);
        ActivateTypeGamePanel(setState);
        ActivateBusterPanel(setState);
        ActivateSpinGamePanel(setState);
    }

    public void ActivateMoneyPanel(bool setState)
    {
        moneyCanvasGrope.interactable = setState;
    }

    public void ActivateTypeGamePanel(bool setState)
    {
        typeCanvasGrope.interactable = setState;
    }

    public void ActivateBusterPanel(bool setState)
    {
        busterCanvasGrope.interactable = setState;
    }

    public void ActivateSpinGamePanel(bool setState)
    {
        spinCanvasGrope.interactable = setState;
    }

    public void ShowSpinButton(bool setState)
    {
        spinPanel.SetActive(setState);
    }

    public void ShowResultButton(bool setState)
    {
        resultPanel.SetActive(setState);
    }
    #endregion
    
    #region Listeners
    public void ChangeTypeGameListener(TypeGame typeGame)
    {
        ActivateTypeGame(typeGame);
    }

    public void ChangeTypeBusterListener(TypeBuster typeBuster)
    {
        ActivateTypeBuster(typeBuster);
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

    public void ChangeCountBusterListener(TypeBuster typeBuster, int count)
    {
        UpdateTypeBusterCount(typeBuster, count);
    }
    #endregion
}
