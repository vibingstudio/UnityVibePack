using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogReferences : MonoBehaviour
{
    [Header("Text Effect Parameters")]
    [Header("Wave Effect")]
    [Range(0f, 20f)]
    public float waveHeight;
    [Range(0f, 20f)]
    public float waveSpeed;
    [Header("Sign Effect")]
    [Range(0f, 20f)]
    public float signHeight;
    [Range(0f, 20f)]
    public float signSpeed;
    [Header("Rainbow Effect")]
    public Gradient colorGradient;
    [Range(0.001f, 20f)]
    public float rainbowWordSpeed;
    [Range(0.001f, 20f)]
    public float rainbowGradientSpeed;
    [Range(0f, 2f)]
    public float rainbowGradientEffectSpeed;
    [Header("Shake Effect")]
    [Range(0f, 20f)]
    public float angleMultiplier;
    [Range(0f, 20f)]
    public float speedMultiplier;
    [Range(0f, 20f)]
    public float curveScale;
    [Header("Domino Effect")]
    [Range(0f, 20f)]
    public float characterDominoEffectHeight;
    [Range(0f, 20f)]
    public float dominoEffectSpeed;
    [Range(0f, 50f)]
    public float dominoEffectTimeToChangeChar;

    [SerializeField] DialogSystem dialogScript;
    public Synthesizer synthesizerScript;
    public Animator npcPortraitAnimator;
    public GameObject characterPortrait;
    public Image characterSprite;
    public Image characterMouth;
    public GameObject arrowIcon;
    public float normalSpeed = 20f;
    public float fastTextSpeed = 50f;
    public float currentSpeed = 20f;
    public bool isNPC;
    public bool isUsingAudioSource;

    [Header("Sounds")]
    public AudioSource a_Sound;
    public AudioSource e_Sound;
    public AudioSource i_Sound;
    public AudioSource o_Sound;
    public AudioSource u_Sound;

    [Header("Synthesizer Frequencies And Parameters")]
    public double frequency_A;
    public double frequency_E;
    public double frequency_I;
    public double frequency_O;
    public double frequency_U;
    public float frequencyLenghtTime;

    [Header("NPC Portrait Sprites")]
    public bool haveEmotions;
    public bool canOpenMouth;
    public Sprite idleSprite;
    public Sprite idleOpenMouth;
    public Sprite angrySprite;
    public Sprite angryOpenMouth;
    public Sprite amazedSprite;
    public Sprite amazedOpenMouth;

    void Start()
    {
        dialogScript.dialogRefScript = this;
    }
}
