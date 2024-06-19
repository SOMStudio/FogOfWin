using Components.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Menu
{
    public class SettingsPanelManager : CanvasGroupComponent
    {
        [Header("Settings")]
        public Slider soundSlider;
        public Slider musicSlider;

        public void SoundVolume(float value)
        {
            soundSlider.value = value;
        }

        public void MusicVolume(float value)
        {
            musicSlider.value = value;
        }
    }
}
