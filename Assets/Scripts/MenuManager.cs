using UnityEngine;
using Core;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic = null;
    
    private void Awake()
    {
        SoundSystem.PlayMusic(menuMusic);
    }

    public void LoadGame()
    {
        EventSystem.Invoke(Events.LoadGame);
    }
}
