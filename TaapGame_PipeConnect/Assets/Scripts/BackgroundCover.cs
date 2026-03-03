using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundCover : MonoBehaviour
{
    void Start()
    {
        Image img = GetComponent<Image>();
        RectTransform rt = img.rectTransform;

        float screenRatio = (float)Screen.width / Screen.height;
        float imageRatio = img.sprite.rect.width / img.sprite.rect.height;

        if (screenRatio > imageRatio)
        {
            float scale = screenRatio / imageRatio;
            rt.localScale = new Vector3(scale, scale, 1);
        }
        else
        {
            float scale = imageRatio / screenRatio;
            rt.localScale = new Vector3(scale, scale, 1);
        }
    }
}
