using UnityEngine;

public class Card : MonoBehaviour
{
    public CardScriptableObject Template;

    public SpriteRenderer Sprite;

    public void init(CardScriptableObject newCard)
    {
        Template = newCard;
        Sprite.sprite = Template.Image;
        gameObject.AddComponent<BoxCollider2D>();
        transform.localPosition = new Vector3(-1000, -1000, 0);
    }
}