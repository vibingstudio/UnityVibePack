using System.Collections;
using VibePack.Utility;
using UnityEngine;

namespace VibePack.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioObject : MonoBehaviour
    {
        public float volume = 1f;
        public bool loop = true;
        public bool playOnAwake = true;
        public bool behaveAsMusic = true;
        public Optional<float> easingTime;

        bool isEasing = false;

        public AudioSource audioSource { get; set; }

        protected virtual void Awake()
        {
            if (behaveAsMusic)
                AudioSystemController.AddMusicObject(this);
            else
                AudioSystemController.AddSoundObject(this);

            audioSource = GetComponent<AudioSource>();
            
            audioSource.volume = 0;
            audioSource.loop = loop;
            audioSource.playOnAwake = false;

            if (playOnAwake)
                Play();
        }

        private IEnumerator EaseMusic()
        {
            audioSource.volume = 0;
            isEasing = true;

            for (float time = 0; time < easingTime; time += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0, volume, time / easingTime);
                yield return null;
            }

            audioSource.volume = volume;
            isEasing = false;
        }

        public void Play()
        {
            if (!behaveAsMusic)
            {
                AudioSystemController.PlaySourceAsSound(this);
                return;
            }

            AudioSystemController.PlaySourceAsMusic(this);

            if (!AudioSystemController.IsMusicEnabled() || !easingTime)
                return;

            StartCoroutine(EaseMusic());
        }

        public void SetVolume(float volume)
        {
            if (isEasing)
                return;

            if (behaveAsMusic && AudioSystemController.IsMusicEnabled())
                audioSource.volume = this.volume = volume;
            else if (AudioSystemController.IsSoundsEnabled())
                audioSource.volume = this.volume = volume;
        }
    }
}