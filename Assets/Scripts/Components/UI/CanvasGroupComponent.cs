using UnityEngine;
using UnityEngine.Events;

namespace Components.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupComponent : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float showTime = 0.5f;

        [Header("Events")]
        [SerializeField] private UnityEvent showEvent;
        [SerializeField] private UnityEvent hideEvent;
        
        protected void Start()
        {
            InitState();
        }

        private void InitState()
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            InitState();
            
            if (canvasGroup.alpha > 0f) return;
            
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            showEvent?.Invoke();
        }

        public void Hide()
        {
            InitState();
            
            if (canvasGroup.alpha < 1f) return;
            
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            hideEvent?.Invoke();
        }
    }
}
