using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class AIConversant : MonoBehaviour
{
    #region Attributes
    public Dialogue dialogue;
    private Dialogue currentDialogue;
    private DialogueNode currentNode;

    // private Transform dialogueBubblePosition;

    private GameObject dialogueBubbleGameObject;

    public Image dialoguePanelText;

    private AudioSource audioSource;

    private bool isDialoguing;
    private bool isEarlyQuit;
    private bool isTyping;
    private GameManager gameManager;

    private int index;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        // dialogueBubblePosition = transform.Find("DialogueBoxPosition");

        dialogueBubbleGameObject = transform.Find("DialogueBubble").gameObject;
        
        audioSource = GetComponent<AudioSource>();

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
                // dialogueBubbleGameObject.transform.position = dialogueBubblePosition.position;

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

                StartCoroutine(EarlyQuitDialogue());
            }
        }
    }

    private IEnumerator StartDialogue(Dialogue newDialogue)
    {
        yield return new WaitForEndOfFrame();

        index = 0;

        currentDialogue = newDialogue;

        currentNode = currentDialogue.GetNodeByIndex(index);

        isEarlyQuit = false;

        // StartCoroutine(FadeInBehaviour(GameObject.Find(currentNode.speakerTextGameObject).GetComponent<TMP_Text>()));
    }

    public IEnumerator QuitDialogue()
    {
        if(!isEarlyQuit)
        {
            yield return new WaitForEndOfFrame();

            GameObject.Find(currentNode.speakerTextGameObject).GetComponent<TMP_Text>().text = "";

            GameObject.Find(currentNode.speakerTextGameObject).GetComponent<TMP_Text>().color = new Color(GameObject.Find(currentNode.speakerTextGameObject).GetComponent<TMP_Text>().color.r, GameObject.Find(currentNode.speakerTextGameObject).GetComponent<TMP_Text>().color.g, GameObject.Find(currentNode.speakerTextGameObject).GetComponent<TMP_Text>().color.b, 0f);

            isDialoguing = false;

            gameManager.StopDialogue();

            StartCoroutine(FadeOutImageBehaviour(dialoguePanelText));

            currentDialogue = null;

            currentNode = null;

            index = 0;

            isEarlyQuit = true;
        }
    }

    //Temporary. I can't think about it right now. Need an early quit for a bux fix but I can't remember what the bug was. Will remove after we send the demo.
    public IEnumerator EarlyQuitDialogue()
    {
        yield return new WaitForEndOfFrame();

        GameObject find = GameObject.Find(currentNode.dialogueTextGameObject);

        if(find != null)
        {
            find.GetComponent<TMP_Text>().text = "";
        }

        GameObject o = GameObject.Find(currentNode.speakerTextGameObject);

        if(o != null)
        {
            o.GetComponent<TMP_Text>().text = "";

            o.GetComponent<TMP_Text>().color = new Color(o.GetComponent<TMP_Text>().color.r,
                o.GetComponent<TMP_Text>().color.g, o.GetComponent<TMP_Text>().color.b, 0f);
        }

        isDialoguing = false;

        gameManager.StopDialogue();

        StartCoroutine(FadeOutImageBehaviour(dialoguePanelText));

        currentDialogue = null;

        dialogue = null;

        currentNode = null;

        index = 0;

        isEarlyQuit = true;
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

                audioSource.clip = currentNode.mAudioClip;

                audioSource.Play();

                StartCoroutine(TypewriterBehaviour());

                yield return new WaitUntil(() => !audioSource.isPlaying);

                StartCoroutine(RunDialogue());
            }
        }
    }

    private IEnumerator WriteTextBehaviour()
    {
        TMP_Text textMeshProUGUI = GameObject.Find(currentNode.dialogueTextGameObject).GetComponent<TMP_Text>();

        // TMP_Text speakerTextMeshProGUI = GameObject.Find(currentNode.speakerTextGameObject).GetComponent<TMP_Text>();

        textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, 0f);

        // speakerTextMeshProGUI.text = currentNode.GetSpeakerText();

        textMeshProUGUI.text = currentNode.GetDialogueText();

        StartCoroutine(FadeInBehaviour(textMeshProUGUI));

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

    private IEnumerator TypewriterBehaviour()
    {
        var textMeshProUGUI = GameObject.Find(currentNode.dialogueTextGameObject).GetComponent<TMP_Text>();

        StartCoroutine(FadeInBehaviour(textMeshProUGUI));

        foreach(char c in currentNode.GetDialogueText())
        {
            textMeshProUGUI.text += c;

            yield return new WaitForSeconds(0.005f);
        }
        
        isTyping = false;
    }
    #endregion
}
