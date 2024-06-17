using Components.UI;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class BusterTypePanelManager : CanvasGroupComponent
    {
        [Header("Buster type panel")]
        [SerializeField] private Image busterType1Panel;
        [SerializeField] private Image busterType2Panel;
        [SerializeField] private Image busterType3Panel;
        [SerializeField] private Button buster1Button;
        [SerializeField] private Button buster2Button;
        [SerializeField] private Button buster3Button;
        [SerializeField] private Text buster1CountText;
        [SerializeField] private Text buster2CountText;
        [SerializeField] private Text buster3CountText;
        
        public void ActivateBusterType(BusterType busterType)
        {
            var defaultColor = busterType1Panel.color;

            switch (busterType)
            {
                case BusterType.Cell:
                    busterType1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    busterType2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    busterType3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case BusterType.LineHorizontal:
                    busterType1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    busterType2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    busterType3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case BusterType.LineVertical:
                    busterType1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    busterType2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    busterType3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    break;
            }
        }
        
        public void UpdateBusterTypeCount(BusterType busterType, int countSet)
        {
            switch (busterType)
            {
                case BusterType.LineHorizontal:
                    buster2CountText.text = countSet.ToString();
                    break;
                case BusterType.LineVertical:
                    buster3CountText.text = countSet.ToString();
                    break;
                case BusterType.Cell:
                    buster1CountText.text = countSet.ToString();
                    break;
            }
        }
    }
}
