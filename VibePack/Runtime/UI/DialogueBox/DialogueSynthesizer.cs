using VibePack.Extensions;
using System.Collections;
using VibePack.Audio;
using UnityEngine;
using System.Linq;
using System;

namespace VibePack.UI
{
    [RequireComponent(typeof(TMP_Animated))]
    public class DialogueSynthesizer : AudioObject
    {
        [SerializeField] DialogueSynthesizerConfiguration configuration;

        readonly double samplingFrequency = 48000;

        TMP_Animated tmp_animated;
        Coroutine playCoroutine;

        WaveType wave = WaveType.Sine;
        float volumeMultiplier;
        double frequency = 440;
        double phase;
        float gain;

        protected override void Awake()
        {
            tmp_animated = GetComponent<TMP_Animated>();

            if (behaveAsMusic)
                AudioSystemController.AddMusicObject(this);
            else
                AudioSystemController.AddSoundObject(this);

            audioSource = GetComponent<AudioSource>();

            audioSource.playOnAwake = true;
            audioSource.volume = 1;
            audioSource.loop = false;
        }

        private void OnEnable() => tmp_animated.onTextReveal.AddListener(OnCharacterRevealed);

        private void OnDisable() => tmp_animated.onTextReveal.RemoveListener(OnCharacterRevealed);

        private void OnCharacterRevealed(char character)
        {
            int index = Array.FindIndex(configuration.characterConfigurations, c => c.characters.Contains(character));
            
            if (!index.InRange(0, configuration.characterConfigurations.Length))
                return;

            CharacterConfiguration config = configuration.characterConfigurations[index];
            volumeMultiplier = configuration.relativeVolume;

            if (config.useSynth)
            {
                if (playCoroutine != null)
                    StopCoroutine(playCoroutine);

                wave = configuration.wave;
                frequency = config.frequency;
                playCoroutine = StartCoroutine(PlaySynth());
                return;
            }

            audioSource = config.audioSource;
            AudioSystemController.PlaySourceAsSound(this);
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            double increment = frequency * 2 * Mathf.PI / samplingFrequency;

            for (int i = 0; i < data.Length; i += channels)
            {
                phase += increment;
                data[i] = wave switch
                {
                    WaveType.Square => 0.6f * (Mathf.Sin((float)phase) >= 0 ? 1 : -1),
                    WaveType.Triangle => Mathf.PingPong((float)phase, 1.0f),
                    WaveType.Saw => (float)(phase % (1 / 0.5f) * 0.5f * 2) - 1,
                    _ => Mathf.Sin((float)phase),
                };

                data[i] *= volumeMultiplier * gain;
                if (channels == 2)
                    data[i + 1] = data[i];

                if (phase > (Mathf.PI * 2))
                    phase -= Mathf.PI * 2;
            }
        }

        private IEnumerator PlaySynth()
        {
            gain = AudioSystemController.soundsVolume;
            yield return new WaitForSeconds(0.25f);
            gain = 0;
        }

        public void SetConfig(DialogueSynthesizerConfiguration configuration) => this.configuration = configuration;
    }
}
