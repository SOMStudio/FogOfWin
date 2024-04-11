using System;
using System.Collections;
using System.ComponentModel;
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

        private int countClickFogOfWin;
        
        public UnityEvent ButtonPlayEvent => playButton.onClick;
        public UnityEvent<float> SliderSoundEvent => settingsPanelManager.soundSlider.onValueChanged; 
        public UnityEvent<float> SliderMusicEvent => settingsPanelManager.musicSlider.onValueChanged;
        
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        public void Init(GameLogic gameLogicSet, ISaveManager saveManagerSet)
        {
            gameLogic = gameLogicSet;
            saveManager = saveManagerSet;
            
            settingsPanelManager.SoundVolume(saveManager.GetValueFloat(GameLogic.SoundVolumeKey));
            settingsPanelManager.MusicVolume(saveManager.GetValueFloat(GameLogic.MusicVolumeKey));

            storePanelManager.Init(saveManager);
            
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

        public void ShowLuckyPox(LuckyBoxItem luckyBox)
        {
            string resultString = "";
            if (luckyBox.typeReward == TypeReward.Money)
            {
                resultString += luckyBox.amount + " $";
            }
            else
            {
                switch (luckyBox.typeBuster)
                {
                    case TypeBuster.Cell:
                        resultString += luckyBox.amount + " Cell Busters";
                        break;
                    case TypeBuster.LineHorizontal:
                        resultString += luckyBox.amount + " Horizontal line Cell Busters";
                        break;
                    case TypeBuster.LineVertical:
                        resultString += luckyBox.amount + " Vertical line Cell Busters";
                        break;
                }
            }

            StartCoroutine(ShowLuckyBoxCoroutine(resultString));
        }

        private IEnumerator ShowLuckyBoxCoroutine(string message)
        {
            luckyBoxCanvasGrope.Interactive(false);
            
            yield return new WaitForSeconds(3f);
            
            luckyBoxCanvasGrope.Hide();

            resultPanelManager.SetText(message);
            resulCanvasGrope.Show();
        }

        public void AddMessage(string message, TypeConsoleText typeText = TypeConsoleText.Message)
        {
            var textResult = "";
            
            switch (typeText)
            {
                case TypeConsoleText.Message:
                    textResult = "<color='blue'>" + message + "</color>";
                    break;
                case TypeConsoleText.Error:
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
            SoundManager.instance?.PlaySoundByIndex(0);
        }
        
        public void PlayOpenPanelSound()
        {
            SoundManager.instance?.PlaySoundByIndex(1);
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

    public enum TypeConsoleText
    {
        Message,
        Error
    }

    public interface IUiMainManager
    {
        UnityEvent ButtonPlayEvent { get; }
        UnityEvent<float> SliderSoundEvent { get; }
        UnityEvent<float> SliderMusicEvent { get; }

        void Init(GameLogic gameLogic, ISaveManager saveManager);
        
        void ShowMainPanels();
        void HideMainPanels();

        void LoadWindowShow();
        void LoadWindowProgress(float progress);
        void LoadWindowHide();
    }
    
    public interface IConsoleManager
    {
        void AddMessage(string message, TypeConsoleText typeText = TypeConsoleText.Message);
        void Clear();
    }
}