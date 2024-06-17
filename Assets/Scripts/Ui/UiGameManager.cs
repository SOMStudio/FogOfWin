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

namespace Ui
{
    public class UiGameManager : MonoSingleton<UiGameManager>, IUiGameManager
    {
        [Header("Main")]
        [SerializeField] private CanvasGroupComponent mainCanvasGrope;
        
        [Header("Panels")]
        [SerializeField] private ScorePanelManager scorePanelManager;
        [SerializeField] private GameTypePanelManager gameTypePanelManager;
        [SerializeField] private BusterTypePanelManager busterTypePanelManager;
        [SerializeField] private SpinPanelManager spinPanelManager;

        [Header("Windows")]
        [SerializeField] private ResultPanelManager resultWindow;

        private ISaveManager saveManager;
        private ISoundManager soundManager;
        
        private int amountWin;

        public UnityEvent ButtonMainMenuEvent => scorePanelManager.MenuButtonClickEvent;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            saveManager.ChangeValueEvent -= SaveSystemListener;
        }

        public void Init(ISaveManager saveManagerSet, ISoundManager soundMangerSet)
        {
            saveManager = saveManagerSet;
            soundManager = soundMangerSet;

            busterTypePanelManager.UpdateBusterTypeCount(BusterType.Cell,
                saveManager.GetValueInt(GameLogic.CountCellBusterKey));
            busterTypePanelManager.UpdateBusterTypeCount(BusterType.LineHorizontal,
                saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey));
            busterTypePanelManager.UpdateBusterTypeCount(BusterType.LineVertical,
                saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey));

            saveManager.ChangeValueEvent += SaveSystemListener;
        }

        private void SaveSystemListener(string keySaveItem)
        {
            switch (keySaveItem)
            {
                case GameLogic.CountCellBusterKey:
                    busterTypePanelManager.UpdateBusterTypeCount(BusterType.Cell,
                        saveManager.GetValueInt(GameLogic.CountCellBusterKey));
                    break;
                case GameLogic.CountLineHorizontalBusterKey:
                    busterTypePanelManager.UpdateBusterTypeCount(BusterType.LineHorizontal,
                        saveManager.GetValueInt(GameLogic.CountLineHorizontalBusterKey));
                    break;
                case GameLogic.CountLineVerticalBusterKey:
                    busterTypePanelManager.UpdateBusterTypeCount(BusterType.LineVertical,
                        saveManager.GetValueInt(GameLogic.CountLineVerticalBusterKey));
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
            if (setState) scorePanelManager.Show();
            else scorePanelManager.Hide();
        }

        private void ShowTypeGamePanel(bool setState)
        {
            if (setState) gameTypePanelManager.Show();
            else gameTypePanelManager.Hide();
        }

        private void ShowBusterPanel(bool setState)
        {
            if (setState) busterTypePanelManager.Show();
            else busterTypePanelManager.Hide();
        }

        private void ShowSpinGamePanel(bool setState)
        {
            if (setState) spinPanelManager.Show();
            else spinPanelManager.Hide();
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
            scorePanelManager.Interactive(setState);
        }

        private void ActivateTypeGamePanel(bool setState)
        {
            gameTypePanelManager.Interactive(setState);
        }

        private void ActivateBusterPanel(bool setState)
        {
            busterTypePanelManager.Interactive(setState);
        }

        private void ActivateSpinGamePanel(bool setState)
        {
            spinPanelManager.Interactive(setState);
        }

        public void ShowSpinButton(bool setState)
        {
            spinPanelManager.ShowSpinButton(setState);
        }

        public void ShowResultButton(bool setState)
        {
            spinPanelManager.ShowResultButton(setState);
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
            gameTypePanelManager.ActivateGameType(gameType);
        }

        public void ChangeTypeBusterListener(BusterType busterType)
        {
            busterTypePanelManager.ActivateBusterType(busterType);
        }

        public void ChangeRateAmountListener(int amount)
        {
            spinPanelManager.ChangeRateAmountListener(amount);
        }

        public void ChangeMoneyAmountListener(int oldAmount, int newAmount, bool afterRotate)
        {
            scorePanelManager.ChangeMoneyAmountListener(oldAmount, newAmount, afterRotate);
        }

        public void ChangeRateAmountAnimatedListener(int oldAmount, int newAmount)
        {
            spinPanelManager.ChangeRateAmountAnimatedListener(oldAmount, newAmount);
        }
    
        public void ChangeMoneyAmountAnimatedListener(int oldAmount, int newAmount)
        {
            scorePanelManager.ChangeMoneyAmountAnimatedListener(oldAmount, newAmount);
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