using UnityEngine;

namespace Services
{
    public class SoundService : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        
        [SerializeField] private AudioClip _music;
        [SerializeField] private AudioClip _cutropeClip;
        [SerializeField] private AudioClip _remove;

        private void Awake()
        {
            PlayMusic();
        }

        public void PlayCutRope()
        {
            PlaySound(_cutropeClip);
        }
        
        public void PlayRemove()
        {
            PlaySound(_remove);
        }

        private void PlaySound(AudioClip audioClip)
        {
            var newSound = new GameObject($"Sound_{audioClip.name}");
            newSound.transform.SetParent(transform);
            var audioSource = newSound.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void PlayMusic()
        {
            PlaySound(_music);
        }
    }
}