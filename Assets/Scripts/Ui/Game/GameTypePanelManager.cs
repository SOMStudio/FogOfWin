using Components.UI;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class GameTypePanelManager : CanvasGroupComponent
    {
        [Header("Game type panel")]
        [SerializeField] private Image gameType1Panel;
        [SerializeField] private Image gameType2Panel;
        [SerializeField] private Image gameType3Panel;
        [SerializeField] private Button gameType1Button;
        [SerializeField] private Button gameType2Button;
        [SerializeField] private Button gameType3Button;
    
        public void ActivateGameType(GameType gameType)
        {
            var defaultColor = gameType1Panel.color;

            switch (gameType)
            {
                case GameType.Count:
                    gameType1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    gameType2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    gameType3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case GameType.Near:
                    gameType1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    gameType2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    gameType3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    break;
                case GameType.Line:
                    gameType1Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    gameType2Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
                    gameType3Panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1);
                    break;
            }
        }
    }
}
