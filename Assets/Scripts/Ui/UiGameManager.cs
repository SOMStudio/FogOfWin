using System.Collections.Generic;
using System.Linq;
using Base;
using Components.UI;
using Data;
using Save;
using Sound;
using Ui.Game;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class UiGameManager : MonoSingleton<UiGameManager>, IUiGameManager
    {
        [Header("Main")]
        [SerializeField] private CanvasGroupComponent mainCanvasGrope;
    
        [Header("Score panel")]
        [SerializeField] private CanvasGroupComponent moneyCanvasGrope;
        [SerializeField] private Text moneyAmountText;
        [SerializeField] private Button menuButton;

        [Header("Type panel")]
        [SerializeField] private CanvasGroupComponent typeCanvasGrope;
        [SerializeField] private Image typeGame1Panel;
        [SerializeField] private Image typeGame2Panel;
        [SerializeField] private Image typeGame3Panel;
        [SerializeField] private Button typeGame1Button;
        [SerializeField] private Button typeGame2Button;
        [SerializeField] private Button typeGame3Button;

        [Header("Buster panel")]
        [SerializeField] private CanvasGroupComponent busterCanvasGrope;
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
        [SerializeField] private CanvasGroupComponent spinCanvasGrope;
        [SerializeField] private Text rateAmountText;
        [SerializeField] private Button reduceRateButton;
        [SerializeField] private Button increaseRateButton;
        [SerializeField] private GameObject spinPanel;
        [SerializeField] private GameObject resultPanel;

        [Header("Result panel")]
        [SerializeField] private ResultPanelManager resultWindow;

        private ISaveManager saveManager;
        private int amountWin = 0;

        public UnityEvent ButtonMainMenuEvent => menuButton.onClick;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            saveManager.ChangeValueEvent -= SaveSystemListener;
        }

        public void Init(ISaveManager saveManagerSet)
        {
            saveManager = saveManagerSet;

            UpdateTypeBusterCount(TypeBuster.Cell, saveManager.GetValueInt(GameLogic.CountCellBusterKey));
            UpdateTypeBusterCount(TypeBuster.LineHorizontal, saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey));
            UpdateTypeBusterCount(TypeBuster.LineVertical, saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey));
            
            saveManager.ChangeValueEvent += SaveSystemListener;
        }
        
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

        private void SaveSystemListener(string keySaveItem)
        {
            switch (keySaveItem)
            {
                case GameLogic.CountCellBusterKey:
                    UpdateTypeBusterCount(TypeBuster.Cell, saveManager.GetValueInt(GameLogic.CountCellBusterKey));
                    break;
                case GameLogic.CountLineHorizontalBusterKey:
                    UpdateTypeBusterCount(TypeBuster.LineHorizontal, saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey));
                    break;
                case GameLogic.CountLineVerticalBusterKey:
                    UpdateTypeBusterCount(TypeBuster.LineVertical, saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey));
                    break;
            }
        }
        
        private void UpdateTypeBusterCount(TypeBuster typeBuster, int countSet)
        {
            switch (typeBuster)
            {
                case TypeBuster.LineHorizontal:
                    buster2CountText.text = countSet.ToString();
                    break;
                case TypeBuster.LineVertical:
                    buster3CountText.text = countSet.ToString();
                    break;
                case TypeBuster.Cell:
                    buster1CountText.text = countSet.ToString();
                    break;
            }
        }
    
        #region Actions
        public void HideGamePanels()
        {
            ShowMoneyPanel(false);
            ShowTypeGamePanel(false);
            ShowBusterPanel(false);
            ShowSpinGamePanel(false);
        }

        public void ShowGamePanels()
        {
            ShowMoneyPanel(true);
            ShowTypeGamePanel(true);
            ShowBusterPanel(true);
            ShowSpinGamePanel(true);
        }

        private void ShowMoneyPanel(bool setState)
        {
            if (setState) moneyCanvasGrope.Show();
            else moneyCanvasGrope.Hide();
        }

        private void ShowTypeGamePanel(bool setState)
        {
            if (setState) typeCanvasGrope.Show();
            else typeCanvasGrope.Hide();
        }

        private void ShowBusterPanel(bool setState)
        {
            if (setState) busterCanvasGrope.Show();
            else busterCanvasGrope.Hide();
        }

        private void ShowSpinGamePanel(bool setState)
        {
            if (setState) spinCanvasGrope.Show();
            else spinCanvasGrope.Hide();
        }

        public void ActivateAllCanvas(bool setState)
        {
            ActivateMoneyPanel(setState);
            ActivateTypeGamePanel(setState);
            ActivateBusterPanel(setState);
            ActivateSpinGamePanel(setState);
        }

        private void ActivateMoneyPanel(bool setState)
        {
            moneyCanvasGrope.Interactive(setState);
        }

        private void ActivateTypeGamePanel(bool setState)
        {
            typeCanvasGrope.Interactive(setState);
        }

        private void ActivateBusterPanel(bool setState)
        {
            busterCanvasGrope.Interactive(setState);
        }

        private void ActivateSpinGamePanel(bool setState)
        {
            spinCanvasGrope.Interactive(setState);
        }

        public void ShowSpinButton(bool setState)
        {
            spinPanel.SetActive(setState);
        }

        public void ShowResultButton(bool setState)
        {
            resultPanel.SetActive(setState);
        }

        public void SetResultWindow(Dictionary<int, int> result, Dictionary<int, GameLogic.Result> _)
        {
            amountWin = result.Sum(el => el.Value);
        
            if (amountWin != 0) resultWindow.SetText(amountWin + " $");
        }

        public void ShowResultWindow()
        {
            if (amountWin > 0) resultWindow.Show();
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
        #endregion
        
        #region Buttons
        public void PlayButtonSound()
        {
            SoundManager.instance?.PlaySoundByIndex(0);
        }
        
        public void PlayOpenPanelSound()
        {
            SoundManager.instance?.PlaySoundByIndex(1);
        }
        #endregion
    }

    public interface IUiGameManager
    {
        UnityEvent ButtonMainMenuEvent { get; }
        void Init(ISaveManager saveManager);
        void HideGamePanels();
        void ShowGamePanels();
        
    }
}