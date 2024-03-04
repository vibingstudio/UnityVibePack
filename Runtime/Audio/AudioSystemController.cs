using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace VibePack.Audio
{
    public class AudioSystemController : MonoBehaviour
    {
        public static bool musicEnabled = true;
        public static bool soundsEnabled = true;

        public static float musicVolume = 1;
        public static float soundsVolume = 1;

        public static List<AudioObject> musicObjects = new List<AudioObject>();
        public static List<AudioObject> soundObjects = new List<AudioObject>();

        private IEnumerator Start()
        {
            yield return null;

            //TOOD: Get Settings From Player Refs
        }

        #region music

        public static bool IsMusicEnabled() => musicEnabled;

        public static void EnableMusic()
        {
            musicEnabled = true;
            musicObjects.RemoveAll(item => item == null);

            foreach (AudioObject obj in musicObjects)
                obj.audioSource.volume = obj.volume;
        }

        public static void DisableMusic()
        {
            musicEnabled = false;
            musicObjects.RemoveAll(item => item == null);

            foreach (AudioObject obj in musicObjects)
                obj.audioSource.volume = 0;
        }

        public static void AddMusicObject(AudioObject audioObject)
        {
            musicObjects.RemoveAll(item => item == null);
            musicObjects.Add(audioObject);
        }

        public static void PlaySourceAsMusic(AudioObject audioObject)
        {
            if (!musicEnabled)
                return;

            audioObject.audioSource.Play();
        }

        public static void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            musicObjects.RemoveAll(item => item == null);

            foreach (AudioObject obj in musicObjects)
                obj.SetVolume(volume);
        }

        #endregion

        #region sounds

        public static bool IsSoundsEnabled() => soundsEnabled;

        public static void EnableSounds()
        {
            soundsEnabled = true;
            soundObjects.RemoveAll(item => item == null);

            foreach (AudioObject obj in soundObjects)
                obj.audioSource.volume = obj.volume;
        }

        public static void DisableSounds()
        {
            soundsEnabled = false;
            soundObjects.RemoveAll(item => item == null);

            foreach (AudioObject obj in soundObjects)
                obj.audioSource.volume = 0;
        }

        public static void AddSoundObject(AudioObject audioObject)
        {
            soundObjects.RemoveAll(item => item == null);
            soundObjects.Add(audioObject);
        }

        public static void PlaySourceAsSound(AudioObject audioObject)
        {
            if (!soundsEnabled)
                return;

            audioObject.audioSource.Play();
        }

        public static void SetSoundVolume(float volume)
        {
            soundsVolume = volume;
            soundObjects.RemoveAll(item => item == null);

            foreach (AudioObject obj in soundObjects)
                obj.SetVolume(volume);
        }

        #endregion

        public static AudioObject PlayClipAt(AudioClip audioClip, Vector3 pos, float volume)
        {
            var tempGameObject = new GameObject(audioClip.name);
            tempGameObject.transform.position = pos;

            AudioObject audioObject = tempGameObject.AddComponent<AudioObject>();
            audioObject.playOnAwake = false;
            audioObject.behaveAsMusic = false;
            audioObject.loop = false;
            audioObject.volume = volume;
            audioObject.SetVolume(volume);

            audioObject.audioSource.clip = audioClip;
            audioObject.audioSource.loop = false;
            audioObject.Play();

            Destroy(tempGameObject, audioClip.length);
            soundObjects.Add(audioObject);

            return audioObject;
        }
    }
}