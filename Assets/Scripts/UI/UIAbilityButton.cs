using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAbilityButton : MonoBehaviour
    {
        [SerializeField] private Image image = null;
        [SerializeField] private Text text = null;

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}
