using Core;
using UnityEngine;

public class LevelSound : MonoBehaviour
{
    [SerializeField] private AudioClip music = default; 
    
    private void Awake()
    {
        SoundSystem.PlayMusic(music);
    }
}
