using UnityEngine;
using UnityEngine.Events;

public class WheelCell : MonoBehaviour
{
    [SerializeField] private int numberSprite;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Events")]
    [SerializeField] private UnityEvent showSpriteEvent;
    [SerializeField] private UnityEvent hideSpriteEvent;
    
    public int Number => numberSprite;
    
    public void SetSprite(int numberInData, Sprite setSprite)
    {
        numberSprite = numberInData;
        sprite.sprite = setSprite;
    }

    public void ShowSprite()
    {
        var color = sprite.color;
        color = new Color(color.r, color.g, color.b, 1);
        sprite.color = color;
        
        showSpriteEvent?.Invoke();
    }
    
    public void HideSprite()
    {
        var color = sprite.color;
        color = new Color(color.r, color.g, color.b, 0);
        sprite.color = color;
        
        hideSpriteEvent?.Invoke();
    }

    public void SetColor(Color setColor)
    {
        sprite.color = new Color(setColor.r, setColor.g, setColor.b, sprite.color.a);
    }
}
