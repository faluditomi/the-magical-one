using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TOS.Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        #region Attributes
        public Dialogue dialogue;

        private Dialogue currentDialogue;

        private DialogueNode currentNode = null;

        private DialogueNode[] children;

        private AudioManager audioManager;
        
        private bool isDialoguing = false;

        private bool isTyping = false;

        private GameObject dialogueBox;

        private AudioSource AudioSource;

        private GameObject continueText;

        private int index = 0;

        [SerializeField] private bool lastDialogue;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            audioManager = FindFirstObjectByType<AudioManager>();

            AudioSource = GetComponent<AudioSource>();

            if(transform.childCount != 0)
            {
                GetComponentInChildren<Canvas>().worldCamera = FindFirstObjectByType<Camera>();
            }
        }

        private void Update()
        {
            if(AudioSource.isPlaying)
            {
                if(continueText == null)
                {
                    return;
                }
                
                if(continueText.activeSelf)
                {
                    continueText.SetActive(false);
                }
                
                return;
            }

            if(continueText != null)
            {
                continueText.SetActive(true);
            }
        }

        #endregion

        #region Normal Methods
        public IEnumerator RunDialogue()
        {
            if(AudioSource.isPlaying)
                {
                    yield break;
                }
                
                if(!isTyping)
                {
                    if(!isDialoguing)
                    {
                        isDialoguing = true;
                        
                        StartCoroutine(StartDialogue(dialogue));
                        
                        yield return new WaitForEndOfFrame();
                    }

                    StartCoroutine(UpdateUI());

                    yield return new WaitForEndOfFrame();

                    if(HasNext())
                    {
                        Next();
                        
                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        isDialoguing = false;
                        
                        GameObject.Find(currentNode.dialogueTextName).GetComponent<TextMeshProUGUI>().text = "";
                        
                        StartCoroutine(QuitDialogue());
                        
                        currentDialogue = dialogue;
                    }
                }
        }

        private IEnumerator StartDialogue(Dialogue newDialogue)
        {
            yield return new WaitForEndOfFrame();

            currentDialogue = newDialogue;

            currentNode = currentDialogue.GetNodeByIndex(index);

            index++;

            dialogueBox = GameObject.Find(currentNode.dialogueBoxName).transform.Find("Image").gameObject;
            
            dialogueBox.SetActive(true);
            
            continueText = dialogueBox.transform.GetChild(0).GetChild(2).gameObject;
        }

        private bool IsActive()
        {
            return currentDialogue != null;
        }

        //Puts the children of the node in an array, which allows us to read the text they have.
        private void Next()
        {
            currentNode = currentDialogue.GetNodeByIndex(index);

            index++;
        }

        //Checks if the dialogue node has anymore children.
        private bool HasNext()
        {
            if(currentDialogue.GetNodeByIndex(index - 1).GetText() == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private IEnumerator QuitDialogue()
        {
            yield return new WaitForEndOfFrame();
            
            currentDialogue = null;
            
            GameObject.Find(currentNode.dialogueBoxName).transform.GetChild(0).gameObject.SetActive(false);

            currentNode = null;

            continueText = null;

            if(lastDialogue)
            {
                FindFirstObjectByType<PlayableDirector>().Play();
            }
        }

        //Updates the UI text.
        private IEnumerator UpdateUI()
        {
            yield return new WaitForEndOfFrame();
            
            GameObject newDialogueBox = GameObject.Find(currentNode.dialogueBoxName).transform.Find("Image").gameObject;
            
            if(newDialogueBox != dialogueBox)
            {
                dialogueBox.GetComponentInChildren<TMP_Text>().text = "";

                dialogueBox.SetActive(false);
                
                dialogueBox = newDialogueBox;

                dialogueBox.SetActive(true);
                
                continueText = dialogueBox.transform.GetChild(0).GetChild(2).gameObject;
            }
            
            GameObject.Find(currentNode.dialogueTextName).GetComponent<TMP_Text>().text = "";

            if(!isTyping)
            {
                isTyping = true;
                
                AudioSource.clip = currentNode.m_AudioClip;

                AudioSource.Play();
                
                StartCoroutine(TypewriterBehaviour());
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator TypewriterBehaviour()
        {
            var textMeshProUGUI = GameObject.Find(currentNode.dialogueTextName).GetComponent<TMP_Text>();

            foreach(char c in currentNode.GetText())
            {
                textMeshProUGUI.text += c;

                yield return new WaitForSeconds(0.005f);
            }

            isTyping = false;
        }
        #endregion
    }
}
