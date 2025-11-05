using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.VoRec
{
    [System.Serializable]
    public class KeyWordData
    {
        public string keyword;
    }
    public class InputOnomatopoeia : MonoBehaviour
    {
        private bool isFirst = true;
        [SerializeField] private string keyWord;
        [SerializeField] VoskSpeechToText VST;
        [SerializeField] private SetKeyword setKeywordData;
        [SerializeField] private StageData stageData;
        private List<string> sceneKeywords = new List<string>();

        private void Awake()
        {
            if (setKeywordData == null)
            {
                Debug.LogWarning("sceneKeywordData が割り当てられていません。Inspectorでアセットをセットしてください。");
                return;
            }
            string sceneName = stageData.currentStageName;
            sceneKeywords = setKeywordData.GetKeywordsForScene(sceneName);

            if (sceneKeywords.Count>0)
            {
                keyWord = sceneKeywords[0];
            }
            else
            {
                Debug.LogWarning($"SceneKeywordData にシーン「{sceneName}」のキーワードが登録されていません");
            }
        }
        private void Start()
        {
            VST.KeyPhrases.Clear();
            // sceneKeywords に入っている複数キーワードを一括で追加
            if (sceneKeywords != null && sceneKeywords.Count > 0)
            {
                VST.KeyPhrases.AddRange(sceneKeywords);
            }
            else
            {
                // もし sceneKeywords が空なら単一の keyWord（保険）を追加
                if (!string.IsNullOrEmpty(keyWord))
                    VST.KeyPhrases.Add(keyWord);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (isFirst == true)
                {
                    GetComponent<VoskSpeechToText>().StartVoskStt();
                    isFirst = false;
                }
                else
                {
                    GetComponent<VoskSpeechToText>().ToggleRecording();
                }
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                GetComponent<VoskSpeechToText>().ToggleRecording();
            }

            if (VST == null)
            {
                Debug.LogWarning("VST が割り当てられていません。Inspectorで VoskSpeechToText をセットしてください。");
                return;
            }

        }
    }
}

