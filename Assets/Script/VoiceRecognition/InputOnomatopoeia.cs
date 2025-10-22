using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOnomatopoeia : MonoBehaviour
{
    private bool isFirst = true;

    [SerializeField] VoskSpeechToText VST;
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            VST.KeyPhrases.Clear();
            VST.KeyPhrases.Add("ƒeƒXƒg");
        }
    }
}
