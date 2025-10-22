using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Unity.Profiling;
using UnityEngine;
using Vosk;
using Newtonsoft.Json;

public class VoskSpeechToText : MonoBehaviour
{
    [Tooltip("Location of the model, relative to the Streaming Assets folder.")]
    public string ModelPath = "vosk-model-small-ja-0.22";

    [Tooltip("The source of the microphone input.")]
    public VoiceProcessor VoiceProcessor;

    [Tooltip("The Max number of alternatives that will be processed.")]
    public int MaxAlternatives = 3;

    [Tooltip("Should the recognizer start when the application is launched?")]
    public bool AutoStart = false;

    [Tooltip("The phrases that will be detected. If left empty, all words will be detected.")]
    public List<string> KeyPhrases = new List<string>();

    //Cached version of the Vosk Model.
    private Model _model;
    
    [SerializeField] private Text recognizerStatusText;

    //Cached version of the Vosk recognizer.
    private VoskRecognizer _recognizer;

    //Conditional flag to see if a recognizer has already been created.
    //TODO: Allow for runtime changes to the recognizer.
    private bool _recognizerReady;

    //Called when the the state of the controller changes.
    public Action<string> OnStatusUpdated;

    //The absolute path to the decompressed model folder.
    private string _decompressedModelPath;

    //A string that contains the keywords in Json Array format
    private string _grammar = "";

    //Flag that is used to wait for the the script to start successfully.
    private bool _isInitializing;

    //Flag that is used to check if Vosk was started.
    private bool _didInit;

    //Threading Logic

    // Flag to signal we are ending
    private bool _running;

    //Thread safe queue of microphone data.
    private readonly ConcurrentQueue<short[]> _threadedBufferQueue = new ConcurrentQueue<short[]>();

    //Thread safe queue of resuts
    private readonly ConcurrentQueue<string> _threadedResultQueue = new ConcurrentQueue<string>();


    private static readonly ProfilerMarker voskRecognizerCreateMarker = new ProfilerMarker("VoskRecognizer.Create");
    private static readonly ProfilerMarker voskRecognizerReadMarker = new ProfilerMarker("VoskRecognizer.AcceptWaveform");

    //If Auto start is enabled, starts vosk speech to text.
    private void Start()
    {
        if (AutoStart)
        {
            StartVoskStt();
        }
    }

    /// <summary>
    /// Start Vosk Speech to text
    /// </summary>
    /// <param name="keyPhrases">A list of keywords/phrases. Keywords need to exist in the models dictionary, so some words like "webview" are better detected as two more common words "web view".</param>
    /// <param name="modelPath">The path to the model folder relative to StreamingAssets. If the path has a .zip ending, it will be decompressed into the application data persistent folder.</param>
    /// <param name="startMicrophone">"Should the microphone after vosk initializes?</param>
    /// <param name="maxAlternatives">The maximum number of alternative phrases detected</param>
    public void StartVoskStt(List<string> keyPhrases = null, string modelPath = default, bool startMicrophone = false,
        int maxAlternatives = 3)
    {
        if (_isInitializing)
        {
            Debug.LogError("Initializing in progress!");
            return;
        }

        if (_didInit)
        {
            Debug.LogError("Vosk has already been initialized!");
            return;
        }

        if (!string.IsNullOrEmpty(modelPath))
        {
            ModelPath = modelPath;
        }

        if (keyPhrases != null)
        {
            KeyPhrases = keyPhrases;
        }

        MaxAlternatives = maxAlternatives;
        StartCoroutine(DoStartVoskStt(startMicrophone));
    }

    //Decompress model, load settings, start Vosk and optionally start the microphone
    private IEnumerator DoStartVoskStt(bool startMicrophone)
    {
        _isInitializing = true;
        yield return WaitForMicrophoneInput();

        yield return Decompress();

        OnStatusUpdated?.Invoke("Loading Model from: " + _decompressedModelPath);
        //Vosk.Vosk.SetLogLevel(0);
        _model = new Model(_decompressedModelPath);

        yield return null;

        OnStatusUpdated?.Invoke("Initialized");
        VoiceProcessor.OnFrameCaptured += VoiceProcessorOnOnFrameCaptured;
        VoiceProcessor.OnRecordingStop += VoiceProcessorOnOnRecordingStop;

        if (startMicrophone)
            VoiceProcessor.StartRecording();

        _isInitializing = false;
        _didInit = true;

        ToggleRecording();
    }

    //Translates the KeyPhraseses into a json array and appends the `[unk]` keyword at the end to tell vosk to filter other phrases.
    private void UpdateGrammar()
    {
        if (KeyPhrases.Count == 0)
        {
            _grammar = "";
            return;
        }

        var keywords = new List<string>();
        foreach (string keyphrase in KeyPhrases)
        {
            keywords.Add(keyphrase.ToLower());
        }

        // tell vosk to filter other phrases
        keywords.Add("[unk]");

        _grammar = JsonConvert.SerializeObject(keywords);
    }

    //Decompress the model zip file or return the location of the decompressed files.
    private IEnumerator Decompress()
    {
        // 事前に解凍済みフォルダをそのまま使用する簡易版

        // 1) StreamingAssets 配下に存在するか確認
        string streamingPath = Path.Combine(Application.streamingAssetsPath, ModelPath);
        if (Directory.Exists(streamingPath))
        {
            _decompressedModelPath = streamingPath;
            OnStatusUpdated?.Invoke("Using model from StreamingAssets: " + _decompressedModelPath);
            Debug.Log(_decompressedModelPath);
            yield break;
        }

        // 2) PersistentDataPath (既に展開済みの可能性) を確認
        string persistentPath = Path.Combine(Application.persistentDataPath, ModelPath);
        if (Directory.Exists(persistentPath))
        {
            _decompressedModelPath = persistentPath;
            OnStatusUpdated?.Invoke("Using model from PersistentDataPath: " + _decompressedModelPath);
            Debug.Log(_decompressedModelPath);
            yield break;
        }

        // 3) 見つからなければエラー
        Debug.LogError("Model folder not found: " + streamingPath);
        yield break;
    }

    //Wait until microphones are initialized
    private IEnumerator WaitForMicrophoneInput()
    {
        while (Microphone.devices.Length <= 0)
            yield return null;
    }

    //Can be called from a script or a GUI button to start detection.
    public void ToggleRecording()
    {
        Debug.Log("Toogle Recording");
        if (!VoiceProcessor.IsRecording)
        {
            Debug.Log("Start Recording");
            _running = true;
            VoiceProcessor.StartRecording();
            Task.Run(ThreadedWork).ConfigureAwait(false);
        } else
        {
            Debug.Log("Stop Recording");
            _running = false;
            VoiceProcessor.StopRecording();
        }
    }

    //Calls the On Phrase Recognized event on the Unity Thread
    private void Update()
    {
        if (_threadedResultQueue.TryDequeue(out string voiceResult))
        {
            // Parse JSON response and display the highest-confidence phrase (first alternative)
            var result = new RecognitionResult(voiceResult);

            if (!result.Partial && result.Phrases.Length > 0 && !string.IsNullOrEmpty(result.Phrases[0].Text))
            {
                recognizerStatusText.text = result.Phrases[0].Text;
            }
        }
    }

    //Callback from the voice processor when new audio is detected
    private void VoiceProcessorOnOnFrameCaptured(short[] samples)
    {
        _threadedBufferQueue.Enqueue(samples);
    }

    //Callback from the voice processor when recording stops
    private void VoiceProcessorOnOnRecordingStop()
    {
        Debug.Log("Stopped");
    }

    //Feeds the autio logic into the vosk recorgnizer
    private async Task ThreadedWork()
    {
        voskRecognizerCreateMarker.Begin();
        if (!_recognizerReady)
        {
            UpdateGrammar();

            //Only detect defined keywords if they are specified.
            if (string.IsNullOrEmpty(_grammar))
            {
                _recognizer = new VoskRecognizer(_model, 16000.0f);
            } else
            {
                _recognizer = new VoskRecognizer(_model, 16000.0f, _grammar);
            }

            _recognizer.SetMaxAlternatives(MaxAlternatives);
            //_recognizer.SetWords(true);
            _recognizerReady = true;

            Debug.Log("Recognizer ready");
        }

        voskRecognizerCreateMarker.End();

        voskRecognizerReadMarker.Begin();

        while (_running)
        {
            if (_threadedBufferQueue.TryDequeue(out short[] voiceResult))
            {
                if (_recognizer.AcceptWaveform(voiceResult, voiceResult.Length))
                {
                    var result = _recognizer.Result();
                    _threadedResultQueue.Enqueue(result);
                }
            } else
            {
                // Wait for some data
                await Task.Delay(100);
            }
        }

        voskRecognizerReadMarker.End();
    }
}