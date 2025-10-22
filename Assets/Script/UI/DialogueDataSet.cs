using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    [CreateAssetMenu(fileName = "DialogueDataSet", menuName = "Game/Dialogue Data Set")]
    public class DialogueDataSet : ScriptableObject
    {
        public DialogueData[] dialogues; 
    }
}
