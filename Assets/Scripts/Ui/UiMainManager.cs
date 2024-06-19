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
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ui
{
    public class UiMainManager : MonoSingleton<UiMainManager>, IUiMainManager, IConsoleManager
    {
        [Header("Main")]
        [SerializeField] private CanvasGroupComponent windowCanvasGroup;
        
        [Header("Panels")]
        [SerializeField] private CanvasGroupComponent mainPanelCanvasGroup;
        [SerializeField] private LoadPanelManager loadPanelManager;
        [SerializeField] private SettingsPanelManager settingsPanelManager;
        [SerializeField] private StorePanelManager storePanelManager;
        [SerializeField] private LuckyBoxPanelManager luckyBoxPanelManager;
        [SerializeField] private ResultPanelManager resultPanelManager;
        [SerializeField] private ConsolePanelManager consolePanelManager;

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
            mainPanelCanvasGroup.Show();

            if (gameLogic.NeedShowLuckyBoxWithAmount(saveManager))
            {
                luckyBoxPanelManager.Generate();
                luckyBoxButton.gameObject.SetActive(true);
            }
        }

        public void HideMainPanels()
        {
            mainPanelCanvasGroup.Hide();
            
            countClickFogOfWin = 0;
        }

        public void ShowSettingsPanel()
        {
            settingsPanelManager.Show();
        }
        
        public void HideSettingsPanel()
        {
            settingsPanelManager.Hide();
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
            luckyBoxPanelManager.Interactive(false);
            
            yield return new WaitForSeconds(1f);
            
            luckyBoxPanelManager.ShowOtherLuckyBox(luckyBox);
            PlayOpenPanelSound();
            
            yield return new WaitForSeconds(2f);
            
            luckyBoxPanelManager.Hide();
            
            yield return new WaitForSeconds(0.5f);
            
            var message = LuckyBoxPanelManager.GetLuckyBoxText(luckyBox);
            resultPanelManager.SetText(message);
            resultPanelManager.Show();
            PlaySound(4);
        }

        public void AddMessage(string message, ConsoleTextType consoleTextTypeText = ConsoleTextType.Message)
        {
            consolePanelManager.AddMessage(message, consoleTextTypeText);
        }

        public void Clear()
        {
            consolePanelManager.Clear();
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
            if (countClickFogOfWin == gameLogic.countClickForOpenConsolePanel) consolePanelManager.Show();
        }

        public void CloseConsolePanel()
        {
            consolePanelManager.Hide();
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