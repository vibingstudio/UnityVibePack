using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using System;
using TMPro;

public class DialogSystem : TextMeshProUGUI
{
    public DialogReferences dialogRefScript;
    public enum Emotions { Idle, Angry, Suprised, Amazed };
    [Serializable] public class EmotionEvent : UnityEvent<Emotions>{}

    List<int> charToWaveIndex;
    bool addValueToWaveTextArray;
    bool isThereAnyWaveText;

    List<int> charToSignEffectIndex;
    bool isThereAnySignText;
    bool addValueToSignTextArray;

    List<int> charToRainbowWordEffectIndex;
    bool isThereAnyRainbowWordEffectText;
    bool addValueToRainbowWordEffectTextArray;
    float rainbowSpeed;
    float rainbowGradientSpeed;

    List<int> charToRainbowEffectIndex;
    bool isThereAnyRainbowEffectText;
    bool addValueToRainbowEffectTextArray;
    Coroutine rainbowWordCoroutine;
    Coroutine rainbowCoroutine;

    List<int> charToShakeEffectIndex; //DONE
    bool isThereAnyShakeEffectText;
    bool addValueToShakeEffectTextArray;
    Coroutine shakeCoroutine;

    Coroutine dominoEffectCoroutine;
    List<int> charToDominoEffectIndex;
    bool addValueToDominoEffectArray;
    bool isThereAnyDominoEffectText;

    Color32[] oldRainbowColorList;
    Color32[] gradientColors;
    public EmotionEvent onEmotionChange;
    public bool isDialogFinished;
    Color32[] newVertexColors;
    int materialIndex;
    int vertexIndex;
    private bool hasTextChanged;
    Coroutine talkCoroutine;

    private struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
    }

    private void Update()
    {
        ForceMeshUpdate(true);
        
        if (isThereAnyWaveText)
            WaveText();
        if (isThereAnySignText)
            SignText();

        if (isThereAnyRainbowWordEffectText && rainbowWordCoroutine == null)
            rainbowWordCoroutine = StartCoroutine(RainbowWordEffect());
        else if (rainbowWordCoroutine != null && !isThereAnyRainbowWordEffectText)
        {
            StopCoroutine(rainbowWordCoroutine);
            rainbowWordCoroutine = null;
        }

        if (isThereAnyRainbowEffectText && rainbowCoroutine == null)
            rainbowCoroutine = StartCoroutine(RainbowEffect());
        else if (rainbowCoroutine != null && !isThereAnyRainbowEffectText)
        {
            StopCoroutine(rainbowCoroutine);
            rainbowCoroutine = null;
        }

        if (isThereAnyShakeEffectText && shakeCoroutine == null)
            shakeCoroutine = StartCoroutine(ShakeEffect());
        else if (shakeCoroutine != null && !isThereAnyShakeEffectText)
        {
            StopCoroutine(rainbowCoroutine);
            shakeCoroutine = null;
        }

        if (isThereAnyDominoEffectText && dominoEffectCoroutine == null)
            dominoEffectCoroutine = StartCoroutine(DominoEffect());
        else if (dominoEffectCoroutine != null && !isThereAnyDominoEffectText)
        {
            StopCoroutine(dominoEffectCoroutine);
            dominoEffectCoroutine = null;
        }
    }

    public void ReadText(string newText)
    {
        isThereAnyWaveText = false;
        isThereAnySignText = false;
        isThereAnyRainbowWordEffectText = false;
        isThereAnyRainbowEffectText = false;
        isThereAnyShakeEffectText = false;
        isThereAnyDominoEffectText = false;
        dialogRefScript.arrowIcon.SetActive(false);
        isDialogFinished = false;
        text = string.Empty;
        string[] subTexts = newText.Split('<', '>');
        string displayText = "";

        for (int i = 0; i < subTexts.Length; i++)
        {
            if (i % 2 == 0)
                displayText += subTexts[i];
            else if (!IsCustomTag(subTexts[i].Replace(" ", "")))
                displayText += $"<{subTexts[i]}>";
        }

        bool IsCustomTag(string tag)
        {
            return tag.StartsWith("speed=") || tag.StartsWith("pause=") || tag.StartsWith("emotion=") || 
                tag.StartsWith("wave") || tag.StartsWith("/wave") || tag.StartsWith("sign") || tag.StartsWith("/sign") ||
                tag.StartsWith("rainbow") || tag.StartsWith("/rainbow") || tag.StartsWith("rainbowWord") || tag.StartsWith("/rainbowWord") ||
                tag.StartsWith("shake") || tag.StartsWith("/shake") || tag.StartsWith("domino") || tag.StartsWith("/domino");
        }

        text = displayText;
        maxVisibleCharacters = 0;
        StartCoroutine(Read(subTexts, displayText));
    }

     private IEnumerator Read(string[] subTexts, string displayText)
    {
        int subCounter = 0;
        int visibleCounter = 0;
        charToWaveIndex = new List<int>();
        charToSignEffectIndex = new List<int>();
        charToRainbowWordEffectIndex = new List<int>();
        charToRainbowEffectIndex = new List<int>();
        charToShakeEffectIndex = new List<int>();
        charToDominoEffectIndex = new List<int>();

        while (subCounter < subTexts.Length)
        {
            if (subCounter % 2 == 1)
            {
                yield return EvaluateTag(subTexts[subCounter].Replace(" ", ""));
                subCounter++;
                continue;
            }

            while (visibleCounter < subTexts[subCounter].Length)
            {
                if (addValueToWaveTextArray)
                    charToWaveIndex.Add(maxVisibleCharacters);
                if (addValueToSignTextArray)
                    charToSignEffectIndex.Add(maxVisibleCharacters);
                if (addValueToRainbowWordEffectTextArray)
                    charToRainbowWordEffectIndex.Add(maxVisibleCharacters);
                if (addValueToRainbowEffectTextArray)
                    charToRainbowEffectIndex.Add(maxVisibleCharacters);
                if (addValueToShakeEffectTextArray)
                    charToShakeEffectIndex.Add(maxVisibleCharacters);
                if (addValueToDominoEffectArray)
                    charToDominoEffectIndex.Add(maxVisibleCharacters);

                visibleCounter++;
                maxVisibleCharacters++;
                if (dialogRefScript.isNPC && dialogRefScript.isUsingAudioSource && visibleCounter < subTexts[subCounter].Length &&
                        !dialogRefScript.a_Sound.isPlaying && !dialogRefScript.e_Sound.isPlaying &&
                        !dialogRefScript.i_Sound.isPlaying && !dialogRefScript.o_Sound.isPlaying &&
                        !dialogRefScript.u_Sound.isPlaying)
                {
                    switch (subTexts[subCounter][visibleCounter].ToString())
                    {
                        case "a":
                        case "A":
                            dialogRefScript.a_Sound.Play();
                            break;
                        case "e":
                        case "E":
                            dialogRefScript.e_Sound.Play();
                            break;
                        case "i":
                        case "I":
                            dialogRefScript.i_Sound.Play();
                            break;
                        case "o":
                        case "O":
                            dialogRefScript.o_Sound.Play();
                            break;
                        case "u":
                        case "U":
                            dialogRefScript.u_Sound.Play();
                            break;
                    }
                }
                else if (dialogRefScript.isNPC && !dialogRefScript.isUsingAudioSource && visibleCounter < subTexts[subCounter].Length)
                {
                    switch (subTexts[subCounter][visibleCounter].ToString())
                    {
                        case "a":
                        case "A":
                            StartCoroutine(dialogRefScript.synthesizerScript.PlaySoundCoroutine(dialogRefScript.frequencyLenghtTime, dialogRefScript.frequency_A,
                                1));
                            break;
                        case "e":
                        case "E":
                            StartCoroutine(dialogRefScript.synthesizerScript.PlaySoundCoroutine(dialogRefScript.frequencyLenghtTime, dialogRefScript.frequency_E,
                                1));
                            break;
                        case "i":
                        case "I":
                            StartCoroutine(dialogRefScript.synthesizerScript.PlaySoundCoroutine(dialogRefScript.frequencyLenghtTime, dialogRefScript.frequency_I,
                                1));
                            break;
                        case "o":
                        case "O":
                            StartCoroutine(dialogRefScript.synthesizerScript.PlaySoundCoroutine(dialogRefScript.frequencyLenghtTime, dialogRefScript.frequency_O,
                                1));
                            break;
                        case "u":
                        case "U":
                            StartCoroutine(dialogRefScript.synthesizerScript.PlaySoundCoroutine(dialogRefScript.frequencyLenghtTime, dialogRefScript.frequency_U,
                                1));
                            break;
                    }
                }

                if (talkCoroutine == null)
                    talkCoroutine = StartCoroutine(Talk());

                yield return new WaitForSeconds(1f / dialogRefScript.currentSpeed);
            }
            visibleCounter = 0;
            subCounter++;
        }

        yield return null;

        WaitForSeconds EvaluateTag(string tag)
        {
            if (tag.Length <= 0)
                return null;

            if (tag.StartsWith("pause="))
                return new WaitForSeconds(float.Parse(tag.Split('=')[1]));

            if (tag.StartsWith("speed="))
            {
                dialogRefScript.currentSpeed = float.Parse(tag.Split('=')[1]);
                dialogRefScript.normalSpeed = float.Parse(tag.Split('=')[1]);
            }
            else if (tag.StartsWith("emotion="))
            {
                onEmotionChange.Invoke((Emotions)System.Enum.Parse(typeof(Emotions), tag.Split('=')[1]));
                if (dialogRefScript.haveEmotions)
                {
                    dialogRefScript.npcPortraitAnimator.Play("Character" + tag.Split('=')[1]);
                    switch (tag.Split('=')[1])
                    {
                        case "Idle":
                                dialogRefScript.characterSprite.sprite = dialogRefScript.idleSprite;
                            if(dialogRefScript.idleOpenMouth != null)
                                dialogRefScript.characterMouth.sprite = dialogRefScript.idleOpenMouth;
                            break;
                        case "Angry":
                                dialogRefScript.characterSprite.sprite = dialogRefScript.angrySprite;
                            if (dialogRefScript.angryOpenMouth != null)
                                dialogRefScript.characterMouth.sprite = dialogRefScript.angryOpenMouth;
                            break;
                        case "Amazed":
                                dialogRefScript.characterSprite.sprite = dialogRefScript.amazedSprite;
                            if (dialogRefScript.amazedOpenMouth != null)
                                dialogRefScript.characterMouth.sprite = dialogRefScript.amazedOpenMouth;
                                break;
                        case "Suprised":
                                /*dialogRefScript.characterSprite.sprite = dialogRefScript.suprisedMouth;
                            if (dialogRefScript.SuprisedOpenMouth != null)
                                dialogRefScript.characterMouth.sprite = dialogRefScript.SuprisedOpenMouth;*/
                                break;
                    }
                }
            }
            else if (tag.StartsWith("wave"))
            {
                addValueToWaveTextArray = true;
                isThereAnyWaveText = true;
            }
            else if (tag.StartsWith("/wave"))
                addValueToWaveTextArray = false;
            else if (tag.StartsWith("sign"))
            {
                addValueToSignTextArray = true;
                isThereAnySignText = true;
            }
            else if (tag.StartsWith("/sign"))
                addValueToSignTextArray = false;

            else if (tag.StartsWith("rainbowWord"))
            {
                addValueToRainbowWordEffectTextArray = true;
                isThereAnyRainbowWordEffectText = true;
            }
            else if (tag.StartsWith("/rainbowWord"))
                addValueToRainbowWordEffectTextArray = false;
            else if (tag.StartsWith("rainbow"))
            {
                addValueToRainbowEffectTextArray = true;
                isThereAnyRainbowEffectText = true;
            }
            else if (tag.StartsWith("/rainbow"))
                addValueToRainbowEffectTextArray = false;
            else if (tag.StartsWith("shake"))
            {
                addValueToShakeEffectTextArray = true;
                isThereAnyShakeEffectText = true;
            }
            else if (tag.StartsWith("/shake"))
                addValueToShakeEffectTextArray = false;
            else if (tag.StartsWith("domino"))
            {
                addValueToDominoEffectArray = true;
                isThereAnyDominoEffectText = true;
            }
            else if (tag.StartsWith("/domino"))
                addValueToDominoEffectArray = false;

            return null;
        }

        isDialogFinished = true;
        dialogRefScript.arrowIcon.SetActive(true);
    }

    private void UpdateTextGeometry()
    {
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            UpdateGeometry(meshInfo.mesh, i);
        }
    }

    private void WaveText()
    {
        for(int i = 0; i < charToWaveIndex.Count; i++)
        {
            var charInfo = textInfo.characterInfo[charToWaveIndex[i]];
            if (!charInfo.isVisible)
                continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0 , Mathf.Sin(Time.time * dialogRefScript.waveSpeed + orig.x * 0.01f)
                    * dialogRefScript.waveHeight, 0);
            }
        }
        UpdateTextGeometry();
    }

    private void SignText()
    {
        for (int i = 0; i < charToSignEffectIndex.Count; i++)
        {
            var charInfo = textInfo.characterInfo[charToSignEffectIndex[i]];
            if (!charInfo.isVisible)
                continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; j++)
            {
                var origin = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = origin + new Vector3(Mathf.Cos(Time.time * dialogRefScript.signSpeed + origin.y * 0.01f)
                    * dialogRefScript.signHeight, Mathf.Sin(Time.time * dialogRefScript.signSpeed + origin.x * 0.01f)
                    * dialogRefScript.signHeight, 0);
            }
        }
        UpdateTextGeometry();
    }

    private IEnumerator RainbowWordEffect()
    {
        Color32 c0 = dialogRefScript.colorGradient.Evaluate(0f);

        while (isThereAnyRainbowWordEffectText)
        {
            for (int i = 0; i < charToRainbowWordEffectIndex.Count; i++)
            {
                materialIndex = textInfo.characterInfo[charToRainbowWordEffectIndex[i]].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                int vertexIndex = textInfo.characterInfo[charToRainbowWordEffectIndex[i]].vertexIndex;
                Color32 c1 = dialogRefScript.colorGradient.Evaluate((rainbowSpeed) % 1);
                rainbowSpeed += dialogRefScript.rainbowWordSpeed * 0.0001f;

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c1;
                newVertexColors[vertexIndex + 3] = c1;
                c0 = c1;
            }

            UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null;
        }

        yield return null;
        rainbowWordCoroutine = null;
    }

    private IEnumerator RainbowEffect()
    {
        Color32 c0 = color;
        Color32 c1 = dialogRefScript.colorGradient.Evaluate(0f);
        gradientColors = new Color32[charToRainbowEffectIndex.Count + 1];
        oldRainbowColorList = new Color32[charToRainbowEffectIndex.Count + 1];

        bool pass = false;
        bool firstTime = true;
        int count = 0;
        float timer = 0;

        while (!pass)
        {
            if(count != charToRainbowEffectIndex.Count)
            {
                firstTime = true;
                count = charToRainbowEffectIndex.Count;
                gradientColors = new Color32[charToRainbowEffectIndex.Count + 1];
                oldRainbowColorList = new Color32[charToRainbowEffectIndex.Count + 1];
            }
            for (int i = 0; i < count; i++)
            {
                timer = 0.00001f;
                materialIndex = textInfo.characterInfo[charToRainbowEffectIndex[i]].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                vertexIndex = textInfo.characterInfo[charToRainbowEffectIndex[i]].vertexIndex;
                c1 = dialogRefScript.colorGradient.Evaluate(rainbowGradientSpeed % 1);
                rainbowGradientSpeed += dialogRefScript.rainbowGradientSpeed * 0.001f;
                if (firstTime)
                {
                    newVertexColors[vertexIndex + 0] = c0;
                    newVertexColors[vertexIndex + 1] = c0;
                    newVertexColors[vertexIndex + 2] = c1;
                    newVertexColors[vertexIndex + 3] = c1;
                    c0 = c1;
                    gradientColors[i] = c1;
                }
                else if (i > 0)
                {
                    newVertexColors[vertexIndex + 0] = oldRainbowColorList[i - 1];
                    newVertexColors[vertexIndex + 1] = oldRainbowColorList[i - 1];
                    newVertexColors[vertexIndex + 2] = oldRainbowColorList[i];
                    newVertexColors[vertexIndex + 3] = oldRainbowColorList[i];
                    gradientColors[i] = oldRainbowColorList[i - 1];
                }
                else if (i == 0)
                {
                    newVertexColors[vertexIndex + 0] = c1;
                    newVertexColors[vertexIndex + 1] = c1;
                    newVertexColors[vertexIndex + 2] = oldRainbowColorList[i];
                    newVertexColors[vertexIndex + 3] = oldRainbowColorList[i];
                    gradientColors[i] = c1;
                }

            }

            c0 = gradientColors[0];

            while (timer < dialogRefScript.rainbowGradientEffectSpeed)
            {
                for(int b = 0; b < count; b++)
                {
                    materialIndex = textInfo.characterInfo[charToRainbowEffectIndex[b]].materialReferenceIndex;
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    vertexIndex = textInfo.characterInfo[charToRainbowEffectIndex[b]].vertexIndex;
                    newVertexColors[vertexIndex + 0] = c0;
                    newVertexColors[vertexIndex + 1] = c0;
                    newVertexColors[vertexIndex + 2] = gradientColors[b + 1];
                    newVertexColors[vertexIndex + 3] = gradientColors[b + 1];
                    c0 = gradientColors[b];
                    UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                }
                timer += Time.deltaTime;
                yield return null;
            }
            if (!isThereAnyRainbowEffectText)
                pass = true;
            firstTime = false;
            for (int a = 0; a < gradientColors.Length; a++)
                oldRainbowColorList[a] = gradientColors[a];
            yield return null;
        }
        yield return null;
        rainbowCoroutine = null;
    }

    private IEnumerator ShakeEffect()
    {
        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        ForceMeshUpdate();

        Matrix4x4 matrix;

        int loopCount = 0;
        hasTextChanged = true;

        // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
        VertexAnim[] vertexAnim = new VertexAnim[1024];
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].angleRange = UnityEngine.Random.Range(10f, 25f);
            vertexAnim[i].speed = UnityEngine.Random.Range(1f, 3f);
        }

        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        while (true)
        {
            // Get new copy of vertex data if the text has changed.
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            if (hasTextChanged)
            {
                // Update the copy of the vertex data for the text object.
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

                hasTextChanged = false;
            }

            // If No Characters then just yield and wait for some text to be added
            if (charToShakeEffectIndex.Count == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }


            for (int i = 0; i < charToShakeEffectIndex.Count; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[charToShakeEffectIndex[i]];

                // Skip characters that are not visible and thus have no geometry to manipulate.
                if (!charInfo.isVisible)
                    continue;

                // Retrieve the pre-computed animation data for the given character.
                VertexAnim vertAnim = vertexAnim[charToShakeEffectIndex[i]];

                // Get the index of the material used by the current character.
                int materialIndex = charInfo.materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = charInfo.vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // Determine the center point of each character at the baseline.
                //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));
                Vector3 jitterOffset = new Vector3(UnityEngine.Random.Range(-.25f, .25f), UnityEngine.Random.Range(-.25f, .25f), 0);

                matrix = Matrix4x4.TRS(jitterOffset * dialogRefScript.curveScale, Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5f, 5f) * dialogRefScript.angleMultiplier), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]) + offset;
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]) + offset;
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]) + offset;
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]) + offset;

                vertexAnim[i] = vertAnim;
            }

            UpdateTextGeometry();

            loopCount += 1;

            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator DominoEffect()
    {
        for (int i = 0; i < charToDominoEffectIndex.Count; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[charToDominoEffectIndex[i]];
            if (!charInfo.isVisible)
                continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            float time = 0;
            while (time < dialogRefScript.dominoEffectTimeToChangeChar)
            {
                for (int j = 0; j < 4; j++)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * dialogRefScript.dominoEffectSpeed + orig.x * 0.01f)
                        * dialogRefScript.characterDominoEffectHeight, 0);
                }
                UpdateTextGeometry();
                time += Time.deltaTime;
                yield return null;
            }
        }
        dominoEffectCoroutine = null;
    }

    private IEnumerator Talk()
    {
        dialogRefScript.characterMouth.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        dialogRefScript.characterMouth.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        talkCoroutine = null;
    }
}
