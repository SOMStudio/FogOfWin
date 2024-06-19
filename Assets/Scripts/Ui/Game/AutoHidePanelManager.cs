using Components.UI;
using UnityEngine;

namespace Ui.Game
{
    public abstract class AutoHidePanelManager : CanvasGroupComponent
    {
        [Header("Base")]
        [SerializeField] private bool useHideAfterTime = true;
        [SerializeField] private float hideAfterTime = 1f;
    
        public override void Show()
        {
            base.Show();
            
            if (useHideAfterTime) Invoke(nameof(Hide), hideAfterTime);
        }
    }
}