using Newtonsoft.Json.Linq;

public class RecognitionResult
{
    public RecognizedPhrase[] Phrases;
    public bool Partial;

    public RecognitionResult(string json)
    {
        JObject resultJson = JObject.Parse(json);

        if (resultJson.TryGetValue("alternatives", out JToken altToken))
        {
            JArray alternatives = (JArray)altToken;
            Phrases = new RecognizedPhrase[alternatives.Count];
            for (int i = 0; i < alternatives.Count; i++)
            {
                Phrases[i] = new RecognizedPhrase((JObject)alternatives[i]);
            }
        }
        else if (resultJson.TryGetValue("result", out JToken resToken))
        {
            Phrases = new[] { new RecognizedPhrase((JObject)resToken) };
        }
        else if (resultJson.TryGetValue("partial", out JToken partialToken))
        {
            Partial = true;
            Phrases = new[] { new RecognizedPhrase { Text = partialToken.Value<string>() } };
        }
        else
        {
            Phrases = new[] { new RecognizedPhrase() };
        }
    }
}

public class RecognizedPhrase
{
    public string Text = string.Empty;
    public float Confidence = 0.0f;

    public RecognizedPhrase() { }

    public RecognizedPhrase(JObject json)
    {
        if (json.TryGetValue("confidence", out JToken confToken))
        {
            Confidence = confToken.Value<float>();
        }

        if (json.TryGetValue("text", out JToken textToken))
        {
            // Vosk adds an extra space at the start of the string.
            Text = textToken.Value<string>().Trim();
        }
    }
}