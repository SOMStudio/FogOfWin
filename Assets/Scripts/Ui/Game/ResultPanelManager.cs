using Components.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    [RequireComponent(typeof(CanvasGroupComponent))]
    public class ResultPanelManager : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private Text informationText;
        [SerializeField] private bool useHideAfterTime = true;
        [SerializeField] private float hideAfterTime = 1f;

        [Header("Components")]
        [SerializeField] private CanvasGroupComponent canvasGroup;

        private void Start()
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroupComponent>();
        }

        public void SetText(string setText)
        {
            informationText.text = setText;
        }
        
        public void Show()
        {
            canvasGroup.Show();
            
            if (useHideAfterTime) Invoke(nameof(Hide), hideAfterTime);
        }

        public void Hide()
        {
            canvasGroup.Hide();
        }
    }
}
