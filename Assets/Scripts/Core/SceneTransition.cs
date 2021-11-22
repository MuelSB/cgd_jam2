using System;
using UnityEngine;

namespace Core
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Animator _anim = null;
        private static readonly int Fade = Animator.StringToHash("Fade");
        
        private void OnEnable() => EventSystem.Subscribe(Events.LoadTransition, OnFade);
        private void OnDisable() => EventSystem.Unsubscribe(Events.LoadTransition, OnFade);

        private void OnFade()
        {
            _anim.SetTrigger(Fade);
        }
    }
}
