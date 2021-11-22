using UnityEngine;

namespace Core
{
    public class SoundSystem : MonoBehaviour
    {
        private enum AudioMode { PlayAudio, DebugDisableAudio, DebugEffectsOnly }
        [SerializeField] private AudioMode _audioMode = AudioMode.PlayAudio;
        
        private static AudioSource _musicSource = default;
        private static AudioSource _effectSource = default;
        
        private void OnEnable()
        {
            // get the audio sources
            var components = GetComponentsInChildren<AudioSource>();
            
            // set Audio Mode
            SetDebugAudioMode();
            
            // set up music
            _musicSource = components[0];
            _musicSource.loop = true;

            // set up effects
            _effectSource = components[1];
        }

        public static void PlayMusic(AudioClip clip)
        {
            if (_musicSource.isPlaying)
            {
                _musicSource.Stop();
            }
            _musicSource.PlayOneShot(clip);
        }
        
        public static void PlayEffect(AudioClip clip)
        {
            _effectSource.PlayOneShot(clip);
        }

        private void SetDebugAudioMode()
        {
            if (_audioMode == AudioMode.PlayAudio || !Application.isEditor) return;
            
            _musicSource.mute = true;
            if (_audioMode != AudioMode.DebugEffectsOnly)
            {
                _effectSource.mute = true;
            }
        }
    }
}
