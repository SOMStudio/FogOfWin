using Base;
using Components.UI;
using Sound;
using Ui.Game;
using Ui.Menu;
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
        [SerializeField] private CanvasGroupComponent storeCanvasGroup;
        [SerializeField] private LoadPanelManager loadPanelManager;

        [Header("Settings")]
        [SerializeField] private CanvasGroupComponent settingsCanvasGroup;
        [SerializeField] private SettingsPanelManager settingsPanelManager;

        [Header("Buttons")]
        [SerializeField] private Button playButton;

        public UnityEvent ButtonPlayEvent => playButton.onClick;
        public UnityEvent<float> SliderSoundEvent => settingsPanelManager.soundSlider.onValueChanged; 
        public UnityEvent<float> SliderMusicEvent => settingsPanelManager.musicSlider.onValueChanged;
        
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        public void Init(float soundAmount, float musicAmount)
        {
            settingsPanelManager.SoundVolume(soundAmount);
            settingsPanelManager.MusicVolume(musicAmount);
        }

        public void ShowMainPanels()
        {
            mainCanvasGroup.Show();
        }

        public void HideMainPanels()
        {
            mainCanvasGroup.Hide();
        }

        public void ShowSettingsPanel()
        {
            settingsCanvasGroup.Show();
        }
        
        public void HideSettingsPanel()
        {
            settingsCanvasGroup.Hide();
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
        UnityEvent<float> SliderSoundEvent { get; }
        UnityEvent<float> SliderMusicEvent { get; }

        void Init(float soundAmount, float musicAmount);
        
        void ShowMainPanels();
        void HideMainPanels();

        void LoadWindowShow();
        void LoadWindowProgress(float progress);
        void LoadWindowHide();
        
    }
}