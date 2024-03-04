using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NPC : MonoBehaviour
{
    public int ID;
    public bool isNPC;
    public bool isUsingAudioSource;
    [SerializeField] bool excecuteDialogOnTriggerZoneAutomatic;

    [SerializeField] DialogSystem dialogScript;
    [SerializeField] DialogReferences dialogRef;
    [SerializeField] GameObject iconKey;
    [SerializeField] GameObject dialogBox;
    GameObject npcSpritePortrait;

    [Header("Sounds")]
    public AudioSource a_Sound;
    public AudioSource e_Sound;
    public AudioSource i_Sound;
    public AudioSource o_Sound;
    public AudioSource u_Sound;

    [Header("Synthesizer Frequencies")]
    [SerializeField] double frequency_A;
    [SerializeField] double frequency_E;
    [SerializeField] double frequency_I;
    [SerializeField] double frequency_O;
    [SerializeField] double frequency_U;

    [Header("Put Here NPC Mouth Sprites")]
    [Header("If The NPC Dont have emotions, just put the")]
    [Header("open and close mouth")]
    [SerializeField] bool haveEmotions;
    [SerializeField] bool canOpenMouth;
    [SerializeField] Sprite idleSprite;
    [SerializeField] Sprite idleOpenMouth;
    [SerializeField] Sprite angrySprite;
    [SerializeField] Sprite angryOpenMouth;
    [SerializeField] Sprite amazedSprite;
    [SerializeField] Sprite amazedOpenMouth;

    [TextArea(15, 20)]
    public string[] textToRead;
    public bool changeText;
    int currentDialog;
    Coroutine readTextCoroutine;
    bool isButtonPressed;
    bool isAutoDialogExcecuted;
    bool isReadingText;

    private void Awake()
    {
        
    }

    private void Start()
    {
        dialogScript = FindObjectOfType<DialogSystem>(true);
        dialogRef = FindObjectOfType<DialogReferences>(true);
        npcSpritePortrait = dialogRef.characterPortrait;
    }

    private void OnEnable()
    {

    }

    private void Update()
    {
        if(isButtonPressed)
            dialogRef.currentSpeed = dialogRef.fastTextSpeed;
        else
            dialogRef.currentSpeed = dialogRef.normalSpeed;
    }

    private void AcceptOrTextSpeedUp(InputAction.CallbackContext context)
    {
        if (excecuteDialogOnTriggerZoneAutomatic && iconKey.activeSelf && readTextCoroutine == null || !isAutoDialogExcecuted && excecuteDialogOnTriggerZoneAutomatic && isReadingText)
        {
            readTextCoroutine = StartCoroutine(ExcecuteDialog());
        }
        else if (context.ReadValueAsButton() && dialogScript.isDialogFinished && readTextCoroutine != null)
            changeText = true;

        if (context.ReadValueAsButton() && !dialogScript.isDialogFinished && readTextCoroutine != null)
            isButtonPressed = true;
        else if(!context.ReadValueAsButton() && !dialogScript.isDialogFinished && readTextCoroutine != null)
            isButtonPressed = false;
    }

    private IEnumerator ExcecuteDialog()
    {
        //show dialog
        dialogBox.SetActive(true);
        dialogRef.isNPC = isNPC;
        dialogRef.isUsingAudioSource = isUsingAudioSource;
        if (isNPC)
        {
            if (isUsingAudioSource)
            {
                dialogRef.a_Sound = a_Sound;
                dialogRef.e_Sound = e_Sound;
                dialogRef.i_Sound = i_Sound;
                dialogRef.o_Sound = o_Sound;
                dialogRef.u_Sound = u_Sound;
            }
            else
            {
                dialogRef.frequency_A = frequency_A;
                dialogRef.frequency_E = frequency_E;
                dialogRef.frequency_I = frequency_I;
                dialogRef.frequency_O = frequency_O;
                dialogRef.frequency_U = frequency_U;
            }
        }
        dialogRef.haveEmotions = haveEmotions;
        dialogRef.canOpenMouth = canOpenMouth;

        if(haveEmotions && canOpenMouth)
        {
            dialogRef.idleSprite = idleSprite;
            dialogRef.idleOpenMouth = idleOpenMouth;
            dialogRef.angrySprite = angrySprite;
            dialogRef.angryOpenMouth = angryOpenMouth;
            dialogRef.amazedSprite = amazedSprite;
            dialogRef.amazedOpenMouth = amazedOpenMouth;
        }
        else if(haveEmotions && !canOpenMouth)
        {
            dialogRef.idleSprite = idleSprite;
            dialogRef.angrySprite = angrySprite;
            dialogRef.amazedSprite = amazedSprite;
        }
        else
            dialogRef.idleSprite = idleSprite;

        //yield return new WaitForSeconds(1f);
        dialogScript.gameObject.SetActive(true);
        npcSpritePortrait.SetActive(true);
        for(int i = 0; i < textToRead.Length; i++)
        {
            print("ID: " + ID);
            currentDialog = i;
            changeText = false;
            dialogScript.ReadText(textToRead[i]);
            while (!changeText)
                yield return null;
        }
        dialogScript.gameObject.SetActive(false);
        npcSpritePortrait.SetActive(false);
        dialogRef.arrowIcon.SetActive(false);
        //dialogBox.GetComponent<Animator>().Play("PanelHide");
        dialogBox.SetActive(false);
        yield return new WaitForSeconds(1f);
        readTextCoroutine = null;
        isReadingText = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("PlayerSolidHitbox") && excecuteDialogOnTriggerZoneAutomatic && !isAutoDialogExcecuted)
        {
            isReadingText = true;
            isAutoDialogExcecuted = true;
            readTextCoroutine = StartCoroutine(ExcecuteDialog());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("PlayerSolidHitbox") && !excecuteDialogOnTriggerZoneAutomatic)
            iconKey.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerSolidHitbox"))
            iconKey.SetActive(false);
    }
}
