using System;
using Components.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Ui.Game
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private CanvasGroupComponent canvasGroup;
    
        [Header("Tutorial")]
        [SerializeField] private TutorialItem[] tutorialSteps;

        [Header("Events")]
        public UnityEvent startTutorialEvent;
        public UnityEvent endTutorialEvent;
        
        private int activeTutorial;
    
        public void Show()
        {
            startTutorialEvent?.Invoke();
            
            activeTutorial = 0;
            ShowTutorial(activeTutorial);
        
            canvasGroup.Show();
        }

        private void Hide()
        {
            canvasGroup.Hide();
            
            endTutorialEvent?.Invoke();
        }

        private void ShowTutorial(int activateTutorial)
        {
            if (activateTutorial > 0)
            {
                foreach (var activeCanvas in tutorialSteps[activateTutorial - 1].canvas)
                {
                    activeCanvas.Hide();
                }
            }
            else
            {
                foreach (var activeCanvas in tutorialSteps[tutorialSteps.Length - 1].canvas)
                {
                    activeCanvas.Hide();
                }
            }
        
            tutorialSteps[activateTutorial].beforeShowItemEvent?.Invoke();
        
            foreach (var activeCanvas in tutorialSteps[activateTutorial].canvas)
            {
                activeCanvas.Show();
            }
        }

        public void ShowNext()
        {
            activeTutorial++;
        
            if (activeTutorial < tutorialSteps.Length) ShowTutorial(activeTutorial);
            else
            {
                Hide();

                activeTutorial = 0;
            
                ShowTutorial(activeTutorial);
            }
        }
    }

    [Serializable]
    public class TutorialItem
    {
        [Header("Invoke Event")]
        public UnityEvent beforeShowItemEvent;
        [Header("Show Canvas Group")]
        public CanvasGroupComponent[] canvas;
    }
}