using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using VibePack.Utility;
using VibePack.Input;
using UnityEngine;
using System;
using TMPro;

namespace VibePack.UI
{
    /// <summary>
    /// Custom UnityEvent to recieve Emotion changes.
    /// </summary>
    [Serializable] public class EmotionEvent : UnityEvent<Emotion> { }

    /// <summary>
    /// Custom UnityEvent for Actions
    /// </summary>
    [Serializable] public class ActionEvent : UnityEvent<string> { }

    /// <summary>
    /// Custom UnityEvent for recieving Characters.
    /// </summary>
    [Serializable] public class TextRevealEvent : UnityEvent<char> { }

    /// <summary>
    /// Emotions available for DialogueSystem.
    /// </summary>
    public enum Emotion { happy, sad, suprised, angry };

    /// <summary>
    /// Class to display text with custom effects. Inherits from TextMeshProUGUI.
    /// </summary>
    public class TMP_Animated : TextMeshProUGUI, IAwaitable
    {
        /// <summary>
        /// InputEvent to be used as mouseClick
        /// </summary>
        [Title("TMP_Animated")]
        [SerializeField] InputEvent clickEvent;
        /// <summary>
        /// Gradient used for the custom effects that need it.
        /// </summary>
        [SerializeField] Gradient gradient;
        /// <summary>
        /// Speed for revealing each character when displaying text.
        /// </summary>
        [SerializeField] float textSpeed = 10;

        /// <summary>
        /// Event where the emotion change will be sent.
        /// </summary>
        public EmotionEvent onEmotionChange;
        /// <summary>
        /// Event where the custom action will be sent.
        /// </summary>
        public ActionEvent onAction;
        /// <summary>
        /// Event where the character revealed will be sent.
        /// </summary>
        public TextRevealEvent onTextReveal;
        /// <summary>
        /// Event called when the dialogue finished displaying.
        /// </summary>
        public UnityEvent onDialogueFinish;

        /// <summary>
        /// Tags currently available.
        /// </summary>
        readonly string[] customTags = new string[] { "speed", "pause", "emotion", "action", "shake", "wave", "bob", "domino", "gradient-word", "gradient-letters" };
        /// <summary>
        /// List to store data for the custom effects.
        /// </summary>
        readonly List<EffectData> effectData = new List<EffectData>();

        /// <summary>
        /// Spacing used for the RainbowLetters effect.
        /// </summary>
        const float spacing = 0.1f;

        Coroutine effectsCoroutine;
        bool mousePressed;
        bool busy;

        /// <summary>
        /// Class used for custom effects.
        /// </summary>
        class EffectData
        {
            readonly Action<EffectData> action;

            public List<VertexData> vertices = new List<VertexData>();
            public int effectIndex;
            public float height;
            public float speed;
            public int length;
            public float time;
            public int index;
            public string id;

            public EffectData(Action<EffectData> action) => this.action = action;

            public void Run() => action?.Invoke(this);
        }
        
        /// <summary>
        /// Holds the positions for a characters vertices.
        /// </summary>
        class VertexData
        {
            public Vector3 topLeft;
            public Vector3 topRight;
            public Vector3 bottomLeft;
            public Vector3 bottomRight;
        }

        public bool ShouldWait() => busy;

        public CustomYieldInstruction Await() => new Awaiter(this);

        /// <summary>
        /// Sets the graient to be used for the effects.
        /// </summary>
        /// <param name="gradient"></param>
        public void SetGradient(Gradient gradient) => this.gradient = gradient;

        public void StopEffects()
        {
            if (busy || effectsCoroutine == null)
                return;

            effectData.Clear();
            StopCoroutine(effectsCoroutine);
            effectsCoroutine = null;
        }

        /// <summary>
        /// Reads the given text and dicsplays it with the custom effects.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <returns>YieldInstruction to await for the text display animation to be finished.</returns>
        public void ReadText(string text)
        {
            if (busy)
                return;

            busy = true;
            base.text = string.Empty;
            string[] subTexts = text.Split('<', '>');
            string displayText = "";

            for (int i = 0; i < subTexts.Length; i++)
            {
                if (i % 2 == 0)
                    displayText += subTexts[i];
                else if (!IsCustomTagLambda(subTexts[i].Replace(" ", "")))
                    displayText += $"<{subTexts[i]}>";
            }

            base.text = displayText;
            maxVisibleCharacters = 0;
            OnPreRenderText += info => effectData.ForEach(d => d.Run());

            effectsCoroutine = StartCoroutine(RunEffectsManual());
            StartCoroutine(Read(subTexts));
            InputManager.On(clickEvent).Bind(OnMouseClick);
        }

        private IEnumerator Read(string[] subTexts)
        {
            for (int subCounter = 0; subCounter < subTexts.Length; subCounter++)
            {
                if (subCounter % 2 == 1)
                {
                    yield return EvaluateTag(subTexts[subCounter], maxVisibleCharacters);
                    continue;
                }

                for (int visibleCounter = 0; visibleCounter < subTexts[subCounter].Length; visibleCounter++)
                {
                    onTextReveal?.Invoke(subTexts[subCounter][visibleCounter]);
                    maxVisibleCharacters++;
                    havePropertiesChanged = false;

                    foreach (EffectData data in effectData)
                        data.vertices.Clear();

                    yield return new WaitForSeconds((mousePressed ? 0.4f : 1f) / textSpeed);
                }
            }

            yield return null;
            onDialogueFinish?.Invoke();
            busy = mousePressed = false;
            InputManager.On(clickEvent).Unbind(OnMouseClick);
        }

        private IEnumerator RunEffectsManual()
        {
            while (true)
            {
                if (!havePropertiesChanged)
                {
                    effectData.ForEach(d => d.Run());
                    UpdateTextGeometry();
                    UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                }

                yield return null;
            }
        }

        private bool IsCustomTagLambda(string tag) => Array.Exists(customTags, validTag => tag.StartsWith(validTag) || tag.StartsWith('/' + validTag));

        private void OnMouseClick(InputValue ctx) => mousePressed = ctx.isPressed;

        /// <summary>
        /// Gets the length of a effect.
        /// </summary>
        /// <param name="data">Effect to get the length of.</param>
        /// <returns>Effect's length.</returns>
        private int GetLength(EffectData data) => data.length == 0 ? maxVisibleCharacters - data.index : data.length;

        /// <summary>
        /// Makes sure an effect has enough vertex data to handle the amount of 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int FillData(EffectData data)
        {
            int effectCharacterCount = GetLength(data);

            if (data.vertices.Count >= effectCharacterCount)
                return effectCharacterCount;

            int characterCount = effectCharacterCount - data.vertices.Count;
            int dataCount = data.vertices.Count;

            for (int i = 0; i < characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[data.index + dataCount + i];
                int vertexIndex = charInfo.vertexIndex;
                Vector3[] sourceVertices = textInfo.CopyMeshInfoVertexData()[charInfo.materialReferenceIndex].vertices;

                VertexData vertex = new VertexData()
                {
                    topLeft = sourceVertices[vertexIndex + 0],
                    topRight = sourceVertices[vertexIndex + 1],
                    bottomLeft = sourceVertices[vertexIndex + 2],
                    bottomRight = sourceVertices[vertexIndex + 3],
                };

                data.vertices.Add(vertex);
            }

            return effectCharacterCount;
        }

        /// <summary>
        /// Shake effect.
        /// </summary>
        /// <param name="effectData"></param>
        private void Shake(EffectData effectData)
        {
            int effectCharacterCount = FillData(effectData);

            for (int i = 0; i < effectCharacterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[effectData.index + i];

                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                Vector3 jitterOffset = new Vector3(UnityEngine.Random.Range(-effectData.speed, effectData.speed), UnityEngine.Random.Range(-effectData.speed, effectData.speed), 0);

                destinationVertices[vertexIndex + 0] = effectData.vertices[i].topLeft + jitterOffset;
                destinationVertices[vertexIndex + 1] = effectData.vertices[i].topRight + jitterOffset;
                destinationVertices[vertexIndex + 2] = effectData.vertices[i].bottomLeft + jitterOffset;
                destinationVertices[vertexIndex + 3] = effectData.vertices[i].bottomRight + jitterOffset;
            }
        }

        /// <summary>
        /// Wave effect.
        /// </summary>
        /// <param name="data"></param>
        private void Wave(EffectData data)
        {
            int effectCharacterCount = FillData(data);

            for (int i = 0; i < effectCharacterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[data.index + i];

                if (!charInfo.isVisible)
                    continue;

                Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                int vertexIndex = charInfo.vertexIndex;

                Vector3 height = new Vector3(0, Mathf.Sin((Time.time * data.speed) + i) * data.height, 0);
                vertices[vertexIndex + 0] = data.vertices[i].topLeft + height;
                vertices[vertexIndex + 1] = data.vertices[i].topRight + height;
                vertices[vertexIndex + 2] = data.vertices[i].bottomLeft + height;
                vertices[vertexIndex + 3] = data.vertices[i].bottomRight + height;
            }
        }

        /// <summary>
        /// Bobbing effect.
        /// </summary>
        /// <param name="data"></param>
        private void Bob(EffectData data)
        {
            int effectCharacterCount = FillData(data);
            float height = Mathf.Sin(Time.time * data.speed) * data.height;

            for (int i = 0; i < effectCharacterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[data.index + i];

                if (!charInfo.isVisible)
                    continue;

                Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                int vertexIndex = charInfo.vertexIndex;

                vertices[vertexIndex + 0] = data.vertices[i].topLeft + new Vector3(0, height, 0);
                vertices[vertexIndex + 1] = data.vertices[i].topRight + new Vector3(0, height, 0);
                vertices[vertexIndex + 2] = data.vertices[i].bottomLeft + new Vector3(0, height, 0);
                vertices[vertexIndex + 3] = data.vertices[i].bottomRight + new Vector3(0, height, 0);
            }
        }

        /// <summary>
        /// Domino effect.
        /// </summary>
        /// <param name="data"></param>
        private void Domino(EffectData data)
        {
            FillData(data);
            TMP_CharacterInfo charInfo = textInfo.characterInfo[data.index + data.effectIndex];
            
            if (!charInfo.isVisible)
                return;

            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vertexIndex = charInfo.vertexIndex;

            float timeDifference = Time.time - data.time;
            float normalizedTime = timeDifference / data.speed * 2;
            float normalizedHeight = (Mathf.Sin((-Mathf.PI / 2) + (normalizedTime * Mathf.PI)) + 1) / 2;

            Vector3 offset = data.height * normalizedHeight * Vector3.up;
            vertices[vertexIndex + 0] = data.vertices[data.effectIndex].topLeft + offset;
            vertices[vertexIndex + 1] = data.vertices[data.effectIndex].topRight + offset;
            vertices[vertexIndex + 2] = data.vertices[data.effectIndex].bottomLeft + offset;
            vertices[vertexIndex + 3] = data.vertices[data.effectIndex].bottomRight + offset;

            if (timeDifference < data.speed)
                return;

            data.effectIndex++;
            data.time = Time.time - (timeDifference - data.speed);

            if (data.length == 0 || data.effectIndex < data.length)
                return;

            data.effectIndex = 0;
        }

        /// <summary>
        /// Gradient word effect.
        /// </summary>
        /// <param name="data"></param>
        private void GradientWord(EffectData data)
        {
            int effectCharacterCount = GetLength(data);
            Color col = gradient.Evaluate(Time.time * data.speed % 1);

            for (int i = 0; i < effectCharacterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[data.index + i];

                if (!charInfo.isVisible)
                    continue;

                Color32[] newVertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                int vertexIndex = charInfo.vertexIndex;

                newVertexColors[vertexIndex + 0] = newVertexColors[vertexIndex + 1] = newVertexColors[vertexIndex + 2] = newVertexColors[vertexIndex + 3] = col;
            }
        }

        /// <summary>
        /// Gradient letters effect.
        /// </summary>
        /// <param name="data"></param>
        private void GradientLetters(EffectData data)
        {
            int effectCharacterCount = GetLength(data);

            for (int i = 0; i < effectCharacterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[data.index + i];

                if (!charInfo.isVisible)
                    continue;

                Color32[] newVertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                int vertexIndex = charInfo.vertexIndex;

                newVertexColors[vertexIndex + 0] = newVertexColors[vertexIndex + 1] = gradient.Evaluate((Time.time + (i * spacing)) * data.speed % 1);
                newVertexColors[vertexIndex + 2] = newVertexColors[vertexIndex + 3] = gradient.Evaluate((Time.time + ((i + 1) * spacing)) * data.speed % 1);
            }
        }

        /// <summary>
        /// Reads a tag and creates an EffectData for it.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private WaitForSeconds EvaluateTag(string tag, int index)
        {
            if (tag.Length <= 0)
                return null;

            if (tag.StartsWith('/'))
            {
                string effect = tag[1..];
                for (int i = effectData.Count - 1; i >= 0; i--)
                {
                    if (effectData[i].id != effect || effectData[i].length != 0)
                        continue;

                    EffectData data = effectData[i];
                    data.length = index - effectData[i].index;
                    effectData[i] = data;
                    break;
                }
                return null;
            }
            
            if (tag.StartsWith("speed="))
                textSpeed = float.Parse(tag.Split('=')[1]);
            else if (tag.StartsWith("pause="))
                return new WaitForSeconds(float.Parse(tag.Split('=')[1] ?? "1"));
            else if (tag.StartsWith("emotion="))
                onEmotionChange?.Invoke((Emotion)Enum.Parse(typeof(Emotion), tag.Split('=')[1]));
            else if (tag.StartsWith("action="))
                onAction?.Invoke(tag.Split('=')[1]);
            else if (tag.StartsWith("shake"))
            {
                EffectData data = new EffectData(Shake)
                {
                    index = index,
                    id = "shake",
                    speed = float.Parse(tag.Split('=')[1] ?? "1")
                };

                effectData.Add(data);
            }
            else
            {
                string[] parameters = tag.Split(' ');
                float speed = float.Parse(Array.Find(parameters, s => s.StartsWith("speed=") || s.StartsWith("duration="))?.Split('=')[1] ?? "1");
                float height = float.Parse(Array.Find(parameters, s => s.StartsWith("height=") || s.StartsWith("spacing="))?.Split('=')[1] ?? "1");
                EffectData data = null;

                if (tag.StartsWith("wave"))
                    data = new EffectData(Wave) { id = "wave" };
                else if (tag.StartsWith("bob"))
                    data = new EffectData(Bob) { id = "bob" };
                else if (tag.StartsWith("domino"))
                    data = new EffectData(Domino) { id = "domino", time = Time.time };
                else if (tag.StartsWith("gradient-word"))
                    data = new EffectData(GradientWord) { id = "gradient-word", time = Time.time };
                else if (tag.StartsWith("gradient-letters"))
                    data = new EffectData(GradientLetters) { id = "gradient-letters", time = Time.time };

                data.index = index;
                data.speed = speed;
                data.height = height == 0 ? 1 : height;

                if (data != null)
                    effectData.Add(data);
            }

            return null;
        }

        /// <summary>
        /// Updates the current text's vertices.
        /// </summary>
        private void UpdateTextGeometry()
        {
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                UpdateGeometry(meshInfo.mesh, i);
            }
        }
    }
}

