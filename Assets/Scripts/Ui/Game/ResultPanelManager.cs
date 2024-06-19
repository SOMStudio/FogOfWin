using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class ResultPanelManager : AutoHidePanelManager
    {
        [Header("Result")]
        [SerializeField] private Text informationText;

        public void SetText(string setText)
        {
            informationText.text = setText;
        }
    }
}