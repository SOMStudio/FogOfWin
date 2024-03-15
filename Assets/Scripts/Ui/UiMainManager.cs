using Base;
using Components.UI;
using Sound;
using Ui.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class UiMainManager : MonoSingleton<UiMainManager>, IUiMainManager
    {
        [Header("Main")]
        [SerializeField] private CanvasGroupComponent windowCanvasGroup;
        
        [Header("Windows")]
        [SerializeField] private CanvasGroupComponent mainCanvasGroup;
        [SerializeField] private CanvasGroupComponent settingsCanvasGroup;
        [SerializeField] private CanvasGroupComponent storeCanvasGroup;
        [SerializeField] private LoadPanelManager loadPanelManager;

        [Header("Buttons")]
        [SerializeField] private Button playButton;

        public UnityEvent ButtonPlayEvent => playButton.onClick;
        
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        public void ShowMainPanels()
        {
            mainCanvasGroup.Show();
        }

        public void HideMainPanels()
        {
            mainCanvasGroup.Hide();
        }

        public void LoadWindowShow()
        {
            loadPanelManager.Show();
        }

        public void LoadWindowProgress(float progress)
        {
            loadPanelManager.Progress(progress);
        }

        public void LoadWindowHide()
        {
            loadPanelManager.Hide();
        }

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

    public interface IUiMainManager
    {
        UnityEvent ButtonPlayEvent { get; }
        
        void ShowMainPanels();
        void HideMainPanels();

        void LoadWindowShow();
        void LoadWindowProgress(float progress);
        void LoadWindowHide();
    }
}
