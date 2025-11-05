using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;

public class StageDialogueSetter : MonoBehaviour
{
    public DialogueDataSet dialogueSet;
    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("StageDialogueSetter Start: " + gameObject.scene.name);
        Debug.Log("Massage.Instance = " + Massage.Instance);

        if (Massage.Instance != null && dialogueSet != null)
        {
            //UIにセリフデータを渡す
            Massage.Instance.SetDialogueData(dialogueSet.dialogues);
            //最初のセリフを表示
            Massage.Instance.ChengeText(0);
        }
    }
}
