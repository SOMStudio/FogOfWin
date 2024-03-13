using Base;
using Components.UI;
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

        [Header("Buttons")]
        [SerializeField] private Button playButton;

        public UnityEvent ButtonPlayEvent => playButton.onClick;
        
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        public void ShowMainPanel()
        {
            mainCanvasGroup.Show();
        }

        public void HideMainPanel()
        {
            mainCanvasGroup.Hide();
        }
    }

    public interface IUiMainManager
    {
        UnityEvent ButtonPlayEvent { get; }
        
        void ShowMainPanel();
        void HideMainPanel();
        
    }
}
