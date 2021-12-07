using System;
using UnityEngine;
using UnityEngine.UI;
public struct UIData
{
    public Sprite image_src;
}
public class DynamicImage : MonoBehaviour
{
    [SerializeField] private Image _image = default;
    public void SetUpImage(UIData data)
    {
        // set the image
        _image.sprite = data.image_src;
    }
}
