using Components.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Menu
{
    public class LoadPanelManager : CanvasGroupComponent
    {
        [Header("Main")]
        [SerializeField] private Slider slider;
        [SerializeField] private Text progressTExt;

        public void Progress(float progress)
        {
            slider.value = progress;
            progressTExt.text = (progress * 100) + "%";
        }
    }
}
