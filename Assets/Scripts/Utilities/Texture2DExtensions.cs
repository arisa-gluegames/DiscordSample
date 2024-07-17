using UnityEngine;
public static class Texture2DExtensions
{
    public static Sprite ToSprite(this Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("Texture is null, cannot create a Sprite.");
            return null;
        }

        // Create a Sprite from the Texture2D
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }
}