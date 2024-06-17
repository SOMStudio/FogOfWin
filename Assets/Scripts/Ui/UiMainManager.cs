using System;
using System.Collections;
using Base;
using Components.UI;
using Data;
using Save;
using Sound;
using Ui.Game;
using Ui.Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class UiMainManager : MonoSingleton<UiMainManager>, IUiMainManager, IConsoleManager
    {
        [Header("Main")]
        [SerializeField] private CanvasGroupComponent windowCanvasGroup;
        
        [Header("Windows")]
        [SerializeField] private CanvasGroupComponent mainCanvasGroup;
        [SerializeField] private LoadPanelManager loadPanelManager;

        [Header("Settings")]
        [SerializeField] private CanvasGroupComponent settingsCanvasGroup;
        [SerializeField] private SettingsPanelManager settingsPanelManager;
        
        [Header("Store")]
        [SerializeField] private CanvasGroupComponent storeCanvasGroup;
        [SerializeField] private StorePanelManager storePanelManager;

        [Header("Lucky Box")]
        [SerializeField] private CanvasGroupComponent luckyBoxCanvasGrope;
        [SerializeField] private LuckyBoxPanelManager luckyBoxPanelManager;
        
        [Header("Result")]
        [SerializeField] private CanvasGroupComponent resulCanvasGrope;
        [SerializeField] private ResultPanelManager resultPanelManager;

        [Header("Console")]
        [SerializeField] private CanvasGroupComponent consolePanel;
        [SerializeField] private Text consoleText;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button luckyBoxButton;

        private GameLogic gameLogic;
        private ISaveManager saveManager;
        private ISoundManager soundManager;

        private int countClickFogOfWin;
        
        public UnityEvent ButtonPlayEvent => playButton.onClick;
        public UnityEvent<float> SliderSoundEvent => settingsPanelManager.soundSlider.onValueChanged; 
        public UnityEvent<float> SliderMusicEvent => settingsPanelManager.musicSlider.onValueChanged;
        
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        public void Init(GameLogic gameLogicSet, ISaveManager saveManagerSet, ISoundManager soundManagerSet)
        {
            gameLogic = gameLogicSet;
            saveManager = saveManagerSet;
            soundManager = soundManagerSet;
            
            settingsPanelManager.SoundVolume(saveManager.GetValueFloat(GameLogic.SoundVolumeKey));
            settingsPanelManager.MusicVolume(saveManager.GetValueFloat(GameLogic.MusicVolumeKey));

            storePanelManager.Init(saveManager, soundManager);
            
            saveManager.ChangeValueEvent += storePanelManager.UpdateStoreValues;
            
            luckyBoxPanelManager.Init(gameLogic, saveManager);

            if (gameLogic.NeedShowLuckyBoxWithCountShow(saveManager)) luckyBoxButton.gameObject.SetActive(true);
            else if (gameLogic.NeedShowLuckyBoxWithAmount(saveManager)) luckyBoxButton.gameObject.SetActive(true);
        }

        public void ShowMainPanels()
        {
            mainCanvasGroup.Show();

            if (gameLogic.NeedShowLuckyBoxWithAmount(saveManager))
            {
                luckyBoxPanelManager.Generate();
                luckyBoxButton.gameObject.SetActive(true);
            }
        }

        public void HideMainPanels()
        {
            mainCanvasGroup.Hide();
            
            countClickFogOfWin = 0;
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

        public void ShowLuckyBox(LuckyBoxItem luckyBox)
        {
            StartCoroutine(ShowLuckyBoxCoroutine(luckyBox));
        }

        private IEnumerator ShowLuckyBoxCoroutine(LuckyBoxItem luckyBox)
        {
            luckyBoxCanvasGrope.Interactive(false);
            
            yield return new WaitForSeconds(1f);
            
            luckyBoxPanelManager.ShowOtherLuckyBox(luckyBox);
            PlayOpenPanelSound();
            
            yield return new WaitForSeconds(2f);
            
            luckyBoxCanvasGrope.Hide();
            
            yield return new WaitForSeconds(0.5f);
            
            var message = LuckyBoxPanelManager.GetLuckyBoxText(luckyBox);
            resultPanelManager.SetText(message);
            resultPanelManager.Show();
            PlaySound(4);
        }

        public void AddMessage(string message, ConsoleTextType consoleTextTypeText = ConsoleTextType.Message)
        {
            var textResult = "";
            
            switch (consoleTextTypeText)
            {
                case ConsoleTextType.Message:
                    textResult = "<color='blue'>" + message + "</color>";
                    break;
                case ConsoleTextType.Error:
                    textResult = "<color='red'>" + message + "</color>";
                    break;
            }

            consoleText.text += Environment.NewLine + DateTime.Now.ToString("[HH:mm:ss] ") + textResult;
        }

        public void Clear()
        {
            consoleText.text = "Console:";
        }

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

        public void OpenConsolePanel()
        {
            countClickFogOfWin++;
            if (countClickFogOfWin == gameLogic.countClickForOpenConsolePanel) consolePanel.Show();
        }

        public void CloseConsolePanel()
        {
            consolePanel.Hide();
            countClickFogOfWin = 0;
        }
        #endregion
    }

    public enum ConsoleTextType
    {
        Message,
        Error
    }

    public interface IUiMainManager
    {
        UnityEvent ButtonPlayEvent { get; }
        UnityEvent<float> SliderSoundEvent { get; }
        UnityEvent<float> SliderMusicEvent { get; }

        void Init(GameLogic gameLogic, ISaveManager saveManager, ISoundManager soundManager);
        
        void ShowMainPanels();
        void HideMainPanels();

        void LoadWindowShow();
        void LoadWindowProgress(float progress);
        void LoadWindowHide();
    }
    
    public interface IConsoleManager
    {
        void AddMessage(string message, ConsoleTextType consoleTextTypeText = ConsoleTextType.Message);
        void Clear();
    }
}