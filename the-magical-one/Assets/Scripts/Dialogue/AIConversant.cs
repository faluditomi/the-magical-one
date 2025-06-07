using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class AIConversant : MonoBehaviour
{
    #region Attributes
    [Header("Dialogue Settings")]
    [Tooltip("The dialogue that this NPC will use.")]
    [SerializeField] private Dialogue dialogue;

    [Tooltip("The dialogue will play itself without the player having to click.")]
    [SerializeField] private bool dialoguePlaysItself;

    [Tooltip("The dialogue will use voice audio when displayed.")]
    [SerializeField] private bool useVoiceAudio;
    
    [Header("Text Settings")]
    [Tooltip("Text will be displayed with a typewriter effect.")]
    [SerializeField] private bool useTypewriterEffect;

    [Tooltip("Text will fade in and out.")]
    [SerializeField] private bool useFadeEffect;

    private Dialogue currentDialogue;
    private DialogueNode currentNode;
    private GameManager gameManager;
    private AudioSource audioSource;
    public Image dialoguePanelText;

    private bool isDialoguing;
    private bool isTyping;

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
    }
    #endregion

    #region Normal Methods
    //Changes the current node to the next one.
    private void Next()
    {
        currentNode = (index < currentDialogue.GetAllNodesByIndex()) ? currentDialogue.GetNodeByIndex(index) : currentDialogue.GetNodeByIndex(index - 1);
    }

    //Checks if there are anymore nodes.
    private bool HasNext()
    {
        return (index <= currentDialogue.GetAllNodesByIndex() - 1);
    }

    //Triggers actions, if there are any, when dialoguing. 
    private void TriggerActions()
    {
        if(currentNode != null && currentNode.GetTriggerActions().Count > 0)
        {
            foreach(DialogueTrigger trigger in this.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(currentNode.GetTriggerActions());
            }
        }
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
    public IEnumerator RunDialogue()
    {
        if(dialogue != null)
        {
            if(!isDialoguing)
            {
                StartCoroutine(FadeInImageBehaviour(dialoguePanelText));

                isDialoguing = true;

                gameManager.StartDialogue();

                StartCoroutine(StartDialogue(dialogue));

                yield return new WaitForEndOfFrame();
            }
            
            GameObject.Find(currentNode.dialogueTextGameObject).GetComponent<TMP_Text>().text = "";

            StartCoroutine(UpdateUI());

            yield return new WaitForEndOfFrame();

            TriggerActions();

            if(HasNext())
            {
                Next();

                index++;

                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();

                StartCoroutine(QuitDialogue());
            }
        }
    }

    private IEnumerator StartDialogue(Dialogue newDialogue)
    {
        yield return new WaitForEndOfFrame();

        index = 0;

        currentDialogue = newDialogue;

        currentNode = currentDialogue.GetNodeByIndex(index);
    }

    public IEnumerator QuitDialogue()
    {
        yield return new WaitForEndOfFrame();

        GameObject find = GameObject.Find(currentNode.dialogueTextGameObject);

        if(find != null)
        {
            find.GetComponent<TMP_Text>().text = "";
        }

        isDialoguing = false;

        gameManager.StopDialogue();

        StartCoroutine(FadeOutImageBehaviour(dialoguePanelText));

        currentDialogue = null;

        dialogue = null;

        currentNode = null;

        index = 0;
    }

    //Updates the UI text.
    private IEnumerator UpdateUI()
    {
        yield return new WaitForEndOfFrame();

        if(index <= currentDialogue.GetAllNodesByIndex())
        {
            yield return new WaitForEndOfFrame();

            if(!isTyping)
            {
                isTyping = true;

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

                isTyping = false;

                if(useVoiceAudio)
                {
                    audioSource.clip = currentNode.mAudioClip;

                    audioSource.Play();

                    yield return new WaitUntil(() => !audioSource.isPlaying);
                }

                if(dialoguePlaysItself && useVoiceAudio)
                {
                    StartCoroutine(RunDialogue());
                }
            }
        }
    }
    #endregion

    #region Text Effects
    //Writes the text with a typewriter effect.
    private IEnumerator TypewriterEffectBehaviour()
    {
        var textMeshProUGUI = GameObject.Find(currentNode.dialogueTextGameObject).GetComponent<TMP_Text>();

        StartCoroutine(FadeInBehaviour(textMeshProUGUI));

        foreach(char c in currentNode.GetDialogueText())
        {
            textMeshProUGUI.text += c;

            yield return new WaitForSeconds(0.005f);
        }
    }

    //Writes the text with a fade effect.
    private IEnumerator FadeEffectBehaviour()
    {
        TMP_Text textMeshProUGUI = GameObject.Find(currentNode.dialogueTextGameObject).GetComponent<TMP_Text>();

        textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, 0f);

        textMeshProUGUI.text = currentNode.GetDialogueText();

        StartCoroutine(FadeInBehaviour(textMeshProUGUI));

        yield return new WaitForEndOfFrame();
    }

    //Writes the text without any effects.
    private IEnumerator WriteTextBehaviour()
    {
        TMP_Text textMeshProUGUI = GameObject.Find(currentNode.dialogueTextGameObject).GetComponent<TMP_Text>();

        textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, 1f);

        textMeshProUGUI.text = currentNode.GetDialogueText();

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
