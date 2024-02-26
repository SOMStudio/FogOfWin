using UnityEngine;

public class WheelCell : MonoBehaviour
{
    [SerializeField] private int numberSprite;
    [SerializeField] private SpriteRenderer sprite;

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
    }
    
    public void HideSprite()
    {
        var color = sprite.color;
        color = new Color(color.r, color.g, color.b, 0);
        sprite.color = color;
    }

    public void SetColor(Color setColor)
    {
        sprite.color = new Color(setColor.r, setColor.g, setColor.b, sprite.color.a);
    }
}
