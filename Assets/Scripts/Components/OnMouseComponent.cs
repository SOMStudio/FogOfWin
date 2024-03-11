using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class OnMouseComponent : MonoBehaviour
    {
        [Header("Main")]
        public bool activeState = true;
        
        [Header("Events")]
        public UnityEvent onMouseEnterEvent;
        public UnityEvent onMouseExitEvent;
        public UnityEvent onMouseDownEvent;
        public UnityEvent onMouseUpEvent;

        public void Activate()
        {
            if (!activeState) activeState = !activeState;
        }

        public void Deactivate()
        {
            if (activeState) activeState = !activeState;
        }
        
        private void OnMouseEnter()
        {
            if (activeState) onMouseEnterEvent?.Invoke();
        }

        private void OnMouseExit()
        {
            if (activeState) onMouseExitEvent?.Invoke();
        }

        private void OnMouseDown()
        {
            if (activeState) onMouseDownEvent?.Invoke();
        }

        private void OnMouseUp()
        {
            if (activeState) onMouseUpEvent?.Invoke();
        }
    }
}
