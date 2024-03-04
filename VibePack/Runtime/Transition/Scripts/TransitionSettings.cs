using UnityEngine;
using System;

namespace VibePack.Transitions
{
    [Serializable]
    public class TransitionSettings
    {
        public TransitionType transitionType;
        public TransitionTextureId textureId;
        public Color color;
        public float transitionTime = 1;
        public bool changeValues;

        public TransitionSettings(Color color, float transitionTime = 1, bool changeValues = false)
        {
            this.transitionTime = transitionTime;
            this.color = color;
            this.changeValues = changeValues;
            transitionType = TransitionType.Alpha;
        }

        public TransitionSettings(TransitionTextureId textureId, Color color = new Color(), float transitionTime = 1, bool changeValues = false)
        {
            this.transitionTime = transitionTime;
            this.textureId = textureId;
            this.color = color;
            this.changeValues = changeValues;
            transitionType = TransitionType.Texture;
        }
    }
}