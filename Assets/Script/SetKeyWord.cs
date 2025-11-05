using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.VoRec
{
    [CreateAssetMenu(fileName ="KeyWordDataSet",menuName ="Game/KeywordDataSet")]
    public class SetKeyword : ScriptableObject
    {
        [System.Serializable]
        public class SceneKeyword
        {
            public string sceneName;
            public List<string> keywords;
        }

        public List<SceneKeyword> sceneKeywords = new List<SceneKeyword>();

        // シーン名に対応するキーワードリストを返す（見つからなければ空リスト）
        public List<string> GetKeywordsForScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return new List<string>();

            var match = sceneKeywords.Find(s => s.sceneName == sceneName);
            if (match != null && match.keywords != null)
                return new List<string>(match.keywords); // コピーを返す（安全のため）
            return new List<string>();
        }
    }
}
