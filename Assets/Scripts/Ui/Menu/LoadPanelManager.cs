using Ui.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Menu
{
    public class LoadPanelManager : BasePanelManager
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
