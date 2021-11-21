using Core;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private AudioClip music = default; 
    
    private void Awake()
    {
        SoundSystem.PlayMusic(music);
    }
}
