using Components.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    [RequireComponent(typeof(CanvasGropeComponent))]
    public class ResultPanelManager : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private Text informationText;
        [SerializeField] private bool useHideAfterTime = true;
        [SerializeField] private float hideAfterTime = 1f;

        [Header("Components")]
        [SerializeField] private CanvasGropeComponent canvasGrope;

        private void Start()
        {
            if (!canvasGrope) canvasGrope = GetComponent<CanvasGropeComponent>();
        }

        public void SetText(string setText)
        {
            informationText.text = setText;
        }
        
        public void Show()
        {
            canvasGrope.Show();
            
            if (useHideAfterTime) Invoke(nameof(Hide), hideAfterTime);
        }

        public void Hide()
        {
            canvasGrope.Hide();
        }
    }
}
