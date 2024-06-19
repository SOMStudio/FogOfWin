using System;
using Components.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Menu
{
    public class ConsolePanelManager : CanvasGroupComponent
    {
        [Header("Console")]
        [SerializeField] private Text consoleText;
        
        public void AddMessage(string message, ConsoleTextType consoleTextTypeText = ConsoleTextType.Message)
        {
            var textResult = "";
            
            switch (consoleTextTypeText)
            {
                case ConsoleTextType.Message:
                    textResult = "<color='blue'>" + message + "</color>";
                    break;
                case ConsoleTextType.Error:
                    textResult = "<color='red'>" + message + "</color>";
                    break;
            }

            consoleText.text += Environment.NewLine + DateTime.Now.ToString("[HH:mm:ss] ") + textResult;
        }
        
        public void Clear()
        {
            consoleText.text = "Console:";
        }
    }
}
