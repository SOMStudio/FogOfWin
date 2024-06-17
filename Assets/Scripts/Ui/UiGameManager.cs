using System.Collections.Generic;
using System.Linq;
using Base;
using Components.UI;
using Data;
using Save;
using Sound;
using Ui.Game;
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
        private ISoundManager soundManager;
        
        private int amountWin;

        public UnityEvent ButtonMainMenuEvent => menuButton.onClick;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            saveManager.ChangeValueEvent -= SaveSystemListener;
        }

        public void Init(ISaveManager saveManagerSet, ISoundManager soundMangerSet)
        {
            saveManager = saveManagerSet;
            soundManager = soundMangerSet;

            UpdateTypeBusterCount(BusterType.Cell, saveManager.GetValueInt(GameLogic.CountCellBusterKey));
            UpdateTypeBusterCount(BusterType.LineHorizontal, saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey));
            UpdateTypeBusterCount(BusterType.LineVertical, saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey));
            
            saveManager.ChangeValueEvent += SaveSystemListener;
        }
        
        private void ActivateTypeGame(GameType gameType)
        {
            var defaultColor = typeGame1Panel.color;

            switch (gameType)
            {
                case GameType.Count:
                    typeGame1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    typeGame2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    typeGame3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case GameType.Near:
                    typeGame1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    typeGame2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    typeGame3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case GameType.Line:
                    typeGame1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    typeGame2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    typeGame3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    break;
            }
        }
    
        private void ActivateTypeBuster(BusterType busterType)
        {
            var defaultColor = typeBuster1Panel.color;

            switch (busterType)
            {
                case BusterType.Cell:
                    typeBuster1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    typeBuster2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    typeBuster3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case BusterType.LineHorizontal:
                    typeBuster1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    typeBuster2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    typeBuster3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case BusterType.LineVertical:
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
                    UpdateTypeBusterCount(BusterType.Cell, saveManager.GetValueInt(GameLogic.CountCellBusterKey));
                    break;
                case GameLogic.CountLineHorizontalBusterKey:
                    UpdateTypeBusterCount(BusterType.LineHorizontal, saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey));
                    break;
                case GameLogic.CountLineVerticalBusterKey:
                    UpdateTypeBusterCount(BusterType.LineVertical, saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey));
                    break;
            }
        }
        
        private void UpdateTypeBusterCount(BusterType busterType, int countSet)
        {
            switch (busterType)
            {
                case BusterType.LineHorizontal:
                    buster2CountText.text = countSet.ToString();
                    break;
                case BusterType.LineVertical:
                    buster3CountText.text = countSet.ToString();
                    break;
                case BusterType.Cell:
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
            if (amountWin > 0)
            {
                resultWindow.Show();
                PlaySound(3);
            }
            else
            {
                PlaySound(5);
            }
        }
        #endregion
    
        #region Listeners
        public void ChangeTypeGameListener(GameType gameType)
        {
            ActivateTypeGame(gameType);
        }

        public void ChangeTypeBusterListener(BusterType busterType)
        {
            ActivateTypeBuster(busterType);
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
            if (newAmount > oldAmount) PlaySound(2);
        }
        #endregion
        
        #region Buttons
        public void PlayButtonSound()
        {
            soundManager.PlaySoundByIndex(0);
        }
        
        public void PlayOpenPanelSound()
        {
            soundManager.PlaySoundByIndex(1);
        }
        
        public void PlaySound(int index)
        {
            soundManager.PlaySoundByIndex(index);
        }
        #endregion
    }

    public interface IUiGameManager
    {
        UnityEvent ButtonMainMenuEvent { get; }
        void Init(ISaveManager saveManager, ISoundManager soundManager);
        void HideGamePanels();
        void ShowGamePanels();
    }
}