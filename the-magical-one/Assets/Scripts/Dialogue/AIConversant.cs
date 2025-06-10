using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Text;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class AIConversant : MonoBehaviour
{
    #region Attributes
    [Header("Dialogue Settings")]
    [Tooltip("The dialogue that this NPC will use.")]
    [SerializeField] private Dialogue dialogue;

    [Tooltip("The dialogue will play itself without the player having to click. (Use Voice Audio Recommended)")]
    [SerializeField] private bool dialoguePlaysItself;

    [Tooltip("The dialogue will use voice audio when displayed.")]
    [SerializeField] private bool useVoiceAudio;

    [Header("Bubble")]
    [Tooltip("The dialogue bubble that will be displayed when dialoguing.")]
    [SerializeField] private Image dialogueBubble;
    [Tooltip("The dialogue text that is inside the bubble.")]
    [SerializeField] private TMP_Text dialogueText;

    [Header("Text Settings")]
    [Tooltip("Text will be displayed with a typewriter effect.")]
    [SerializeField] private bool useTypewriterEffect;

    [SerializeField] private float typewriterSpeed = 0.02f;

    [Tooltip("Text will fade in and out.")]
    [SerializeField] private bool useFadeEffect;

    private Dialogue currentDialogue;
    private DialogueNode currentNode;
    private GameManager gameManager;
    private AudioSource audioSource;
    private DialogueTrigger[] dialogueTriggers;
    private WaitForSeconds typewriterWait;
    private StringBuilder stringBuilder = new StringBuilder();

    private bool isDialoguing;

    private int index;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        if(useVoiceAudio)
        {
            audioSource = GetComponent<AudioSource>();
        }

        gameManager = FindFirstObjectByType<GameManager>();

        dialogueTriggers = GetComponents<DialogueTrigger>();

        typewriterWait = new WaitForSeconds(typewriterSpeed);
    }
    #endregion

    #region Normal Methods
    //Checks if there are anymore nodes.
    private bool HasNext()
    {
        if(currentDialogue == null)
        {
            return false;
        }
            
        return (index < currentDialogue.GetAllNodesByIndex());
    }

    //Triggers actions, if there are any, when dialoguing. 
    private void TriggerActions()
    {
        foreach(DialogueTrigger trigger in dialogueTriggers)
        {
            trigger.Trigger(currentNode.GetTriggerActions());
        }
    }

    //Call this coroutine to start the dialogue.
    public void StartDialogue()
    {
        if(dialogue == null || isDialoguing)
        {
            return;
        }

        StartCoroutine(StartDialogueBehaviour());
    }
    #endregion

    #region Setters
    public void SetNewDialogue(Dialogue newDialogue)
    {
        if(newDialogue != null)
        {
            dialogue = newDialogue;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator StartDialogueBehaviour()
    {
        isDialoguing = true;

        gameManager.StartDialogue();

        currentDialogue = dialogue;

        index = 0;

        dialogueText.text = "";

        StartCoroutine(FadeInImageBehaviour(dialogueBubble));

        yield return new WaitForEndOfFrame();

        while(HasNext())
        {
            currentNode = currentDialogue.GetNodeByIndex(index);

            if(dialogueTriggers.Length > 0 && currentNode.GetTriggerActions().Count > 0)
            {
                TriggerActions();
            }

            if(useTypewriterEffect)
            {
                StartCoroutine(TypewriterEffectBehaviour());
            }
            else if(useFadeEffect)
            {
                StartCoroutine(FadeEffectBehaviour());
            }
            else
            {
                StartCoroutine(WriteTextBehaviour());
            }

            if(useVoiceAudio && currentNode.mAudioClip != null)
            {
                audioSource.clip = currentNode.mAudioClip;

                audioSource.Play();

                if(dialoguePlaysItself)
                {
                    yield return new WaitUntil(() => !audioSource.isPlaying);
                }
            }
            else if(dialoguePlaysItself)
            {
                //Need to calculate this based on text length.
                yield return new WaitForSeconds(5f);
            }
            else
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            }

            index++;
        }

        StartCoroutine(QuitDialogue());
    }

    private IEnumerator QuitDialogue()
    {
        yield return new WaitForEndOfFrame();

        dialogueText.text = "";

        isDialoguing = false;

        gameManager.StopDialogue();

        StartCoroutine(FadeOutImageBehaviour(dialogueBubble));

        currentDialogue = null;

        dialogue = null;

        currentNode = null;

        index = 0;
    }
    #endregion

    #region Text Effects
    //Writes the text with a typewriter effect.
    private IEnumerator TypewriterEffectBehaviour()
    {
        StartCoroutine(FadeInBehaviour(dialogueText));

        stringBuilder.Clear();

        dialogueText.text = "";

        string fullText = currentNode.GetDialogueText();

        foreach(char c in fullText)
        {
            stringBuilder.Append(c);

            dialogueText.text = stringBuilder.ToString();

            yield return typewriterWait;
        }
    }

    //Writes the text with a fade effect.
    private IEnumerator FadeEffectBehaviour()
    {
        dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 0f);

        dialogueText.text = currentNode.GetDialogueText();

        StartCoroutine(FadeInBehaviour(dialogueText));

        yield return new WaitForEndOfFrame();
    }

    //Writes the text without any effects.
    private IEnumerator WriteTextBehaviour()
    {
        dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 1f);

        dialogueText.text = currentNode.GetDialogueText();

        yield return new WaitForEndOfFrame();
    }

    //Fades in the text at the start of every node.
    private IEnumerator FadeInBehaviour(TMP_Text text)
    {
        Color objectColor = text.color;

        while(objectColor.a <= 1f)
        {
            objectColor.a += 0.2f * Time.deltaTime;

            text.color = objectColor;

            yield return null;
        }
    }

    //Fades in the dialogue bubble at the start of dialoguing.
    private IEnumerator FadeInImageBehaviour(Image image)
    {
        Color objectColor = image.color;

        if(objectColor.a != 0f)
        {
            objectColor.a = 0f;
        }

        while(objectColor.a <= 1f)
        {
            objectColor.a += 5f * Time.deltaTime;

            image.color = objectColor;

            yield return null;
        }
    }

    //Fades out the dialogue bubble at the end of dialoguing.
    private IEnumerator FadeOutImageBehaviour(Image image)
    {
        Color objectColor = image.color;

        if(objectColor.a != 1f)
        {
            objectColor.a = 1f;
        }

        while(objectColor.a >= 0f)
        {
            objectColor.a -= 5f * Time.deltaTime;

            image.color = objectColor;

            yield return null;
        }
    }
    #endregion
}
