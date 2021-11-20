using System;
using UnityEngine;

namespace Core
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Animator _anim = null;
        private static readonly int Fade = Animator.StringToHash("Fade");
        
        private const string StartFade = "StartFade";
        private const string StopFade= "StopFade";

        private void OnEnable()
        {
            EventSystem.Subscribe(StartFade, OnFade);
            EventSystem.Subscribe(StopFade, OnFade);
        }

        private void OnDisable()
        {
            EventSystem.Unsubscribe(StartFade, OnFade);
            EventSystem.Unsubscribe(StopFade, OnFade);
        }

        private void OnFade()
        {
            _anim.SetTrigger(Fade);
        }
    }
}
