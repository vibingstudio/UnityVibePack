using UnityEngine;
using System;

namespace VibePack.UI
{
    public enum WaveType
    {
        Sine,
        Square,
        Triangle,
        Saw,
    }

    [Serializable]
    public struct CharacterConfiguration
    {
        public char[] characters;
        public AudioSource audioSource;
        public bool useSynth;
        [Range(-1000, 1000)] public double frequency;
    }

    [CreateAssetMenu(menuName = "VibePack/SynthesizerConfig", fileName = "SynthesizerConfig")]
    public class DialogueSynthesizerConfiguration : ScriptableObject
    {
        public WaveType wave;
        public float relativeVolume;
        public CharacterConfiguration[] characterConfigurations;
    }
}
