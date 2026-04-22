using UnityEngine;

namespace JCC.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        public void PlaySFX() 
        {
            _audioSource.Play();
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
}