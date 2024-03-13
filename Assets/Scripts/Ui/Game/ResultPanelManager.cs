using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class ResultPanelManager : BasePanelManager
    {
        [Header("Main")]
        [SerializeField] private Text informationText;

        public void SetText(string setText)
        {
            informationText.text = setText;
        }
    }
}