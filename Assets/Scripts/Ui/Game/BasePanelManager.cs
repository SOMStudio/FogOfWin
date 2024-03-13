using Components.UI;
using UnityEngine;

namespace Ui.Game
{
    [RequireComponent(typeof(CanvasGroupComponent))]
    public abstract class BasePanelManager : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private bool useHideAfterTime = true;
        [SerializeField] private float hideAfterTime = 1f;
    
        private CanvasGroupComponent canvasGroup;
    
        protected virtual void Start()
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroupComponent>();
        }
    
        public virtual void Show()
        {
            canvasGroup.Show();
            
            if (useHideAfterTime) Invoke(nameof(Hide), hideAfterTime);
        }

        public virtual void Hide()
        {
            canvasGroup.Hide();
        }
    }
}