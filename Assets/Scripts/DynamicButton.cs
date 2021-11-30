using System;
using UnityEngine;
using UnityEngine.UI;

public struct ButtonData
{
    public Ability Ability;
    public Action Callback;
}

public class DynamicButton : MonoBehaviour
{
    [SerializeField] private Image _image = default;
    [SerializeField] private Button _button = default;

    public void SetUpButton(ButtonData data)
    {
        // set the image
        _image.sprite = data.Ability.image;
        
        // register action to the button
        _button.onClick.AddListener( data.Callback.Invoke );
    }
}