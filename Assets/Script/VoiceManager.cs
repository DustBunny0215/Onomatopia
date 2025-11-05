using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance;
    public string recognizedWord = "";
    void Awake()
    {
        Instance = this;
    }
    public void SetRecognizedWord(string word)
    {
        Debug.Log("aa");
        recognizedWord = word;
    }

}