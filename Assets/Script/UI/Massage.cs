using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    [System.Serializable]
    public class DialogueData
    {
        //キャラクターの画像
        //public Sprite chaImage;
        public string dialogue;
    }
    public class Massage : MonoBehaviour
    {
        public static Massage Instance { get; private set; }

        [SerializeField] GameObject textNom;
        [SerializeField]Text diaTx;
        //[SerializeField]Image diaIm;
        [SerializeField] private float textSpeed = 0.05f; //一文字の表示スピード

        public DialogueData[] datas;
        public int prevTxNum = 0; //前のテキスト数字 
        private bool isTyping = false; //文字の表示途中かどうか
        private Coroutine typingCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }

        // Start is called before the first frame update
        void Start()
        {
            //textNom.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && textNom.activeSelf)
            {
                if (isTyping)
                {
                    //スキップして全文表示
                    StopCoroutine(typingCoroutine);
                    diaTx.text = datas[prevTxNum].dialogue;
                    isTyping = false;

                }
                else
                {
                    prevTxNum++;//更新
                    if(prevTxNum < datas.Length)
                    {
                        ChengeText(prevTxNum);
                    }

                    else
                    {
                        textNom.SetActive(false);
                    }
                        
                }
            }

        }

        public void ChengeText(int num)
        {
            Debug.Log(num);
            textNom.SetActive(true);
            //diaIm.sprite = datas[num].chaImage;
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeText(datas[num].dialogue));

        }
        private IEnumerator TypeText(string text)
        {
            isTyping = true;
            diaTx.text = "";

            foreach (char c in text)
            {
                diaTx.text += c;
                yield return new WaitForSecondsRealtime(textSpeed);
            }

            isTyping = false;
        }
        public void SetDialogueData(DialogueData[] newDatas)
        {
            datas = newDatas;
            prevTxNum = 0;
        }
    }
}



