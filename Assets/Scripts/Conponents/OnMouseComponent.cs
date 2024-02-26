using System;
using UnityEngine;
using UnityEngine.Events;

public class OnMouseComponent : MonoBehaviour
{
    public UnityEvent onMouseEnterEvent;
    public UnityEvent onMouseExitEvent;
    public UnityEvent onMouseDownEvent;
    public UnityEvent onMouseUpEvent;

    private void Start()
    {
        Physics2D.queriesHitTriggers = true;
    }

    private void OnMouseEnter()
    {
        onMouseEnterEvent?.Invoke();
    }

    private void OnMouseExit()
    {
        onMouseExitEvent?.Invoke();
    }

    private void OnMouseDown()
    {
        onMouseDownEvent?.Invoke();
    }

    private void OnMouseUp()
    {
        onMouseUpEvent?.Invoke();
    }
}
