using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SpeechRecognitionTest : MonoBehaviour {
    [SerializeField] private Button recordButton; // Single button for Start/Stop
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI spellingText;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start() {
        recordButton.onClick.AddListener(ToggleRecording);
        InitializeUI();
    }

    private void InitializeUI() {
        text.text = "Hi there! Click on Ice Man and say something. Ice Man will spell it out for you!";
        text.color = Color.black;
        spellingText.text = "";
        UpdateButtonLabel();
    }

    private void Update() {
        if (recording && Microphone.GetPosition(null) >= clip.samples) {
            StopRecording();
        }
    }

    private void ToggleRecording() {
        if (recording) {
            StopRecording();
        } else {
            StartRecording();
        }
        UpdateButtonLabel();
    }

    private void StartRecording() {
        if (Microphone.devices.Length == 0) {
            ShowError("No microphone found. Try again!");
            return;
        }

        try {
            text.color = Color.black;
            text.text = "Listening...";
            spellingText.text = "";

            clip = Microphone.Start(null, false, 10, 44100);
            if (clip == null) {
                throw new System.Exception("Failed to start microphone.");
            }

            recording = true;
        } catch (System.Exception ex) {
            ShowError($"Error starting recording: {ex.Message}");
        }
    }

    private void StopRecording() {
        try {
            if (clip == null || !recording) {
                ShowError("No recording to stop.");
                return;
            }

            int position = Microphone.GetPosition(null);
            if (position <= 0) {
                throw new System.Exception("Microphone did not capture any data.");
            }

            Microphone.End(null);
            var samples = new float[position * clip.channels];
            clip.GetData(samples, 0);
            bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
            recording = false;

            SendRecording();
        } catch (System.Exception ex) {
            ShowError($"Error stopping recording: {ex.Message}");
        }
    }

    private void SendRecording() {
        try {
            if (bytes == null || bytes.Length == 0) {
                throw new System.Exception("No audio to send.");
            }

            text.color = Color.yellow;
            text.text = "Processing...";

            HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
                HandleSpeechRecognitionResponse(response);
            }, error => {
                ShowError($"API error: {error}");
            });
        } catch (System.Exception ex) {
            ShowError($"Error sending recording: {ex.Message}");
        }
    }

    private void HandleSpeechRecognitionResponse(string response) {
        if (IsEnglish(response)) {
            text.color = Color.black;
            text.text = $"Recognized: {response}";
            DisplayWordSpellings(response);
        } else {
            ShowError("Sorry, I can only understand English.");
        }
    }

    private bool IsEnglish(string text) {
        return Regex.IsMatch(text, @"^[a-zA-Z0-9\s.,!?-]*$");
    }

    private void DisplayWordSpellings(string response) {
        if (string.IsNullOrWhiteSpace(response)) {
            spellingText.color = Color.red;
            spellingText.text = "No words detected.";
            return;
        }

        spellingText.color = Color.black;
        spellingText.text = "Spellings:\n";
        foreach (var word in response.Split(' ')) {
            spellingText.text += $"{word.ToUpper()} - {string.Join(" ", word.ToUpper().ToCharArray())}\n";
        }
    }

    private void ShowError(string errorMessage) {
        text.color = Color.red;
        text.text = errorMessage;
        recording = false;
        UpdateButtonLabel();
    }

    private void UpdateButtonLabel() {
        recordButton.GetComponentInChildren<TextMeshProUGUI>().text = recording ? "Stop" : "Start";
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        try {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
                using (var writer = new BinaryWriter(memoryStream)) {
                    WriteWavHeader(writer, samples.Length, frequency, channels);
                    WriteWavSamples(writer, samples);
                }
                return memoryStream.ToArray();
            }
        } catch (System.Exception ex) {
            ShowError($"Error encoding WAV: {ex.Message}");
            return null;
        }
    }

    private void WriteWavHeader(BinaryWriter writer, int sampleLength, int frequency, int channels) {
        writer.Write("RIFF".ToCharArray());
        writer.Write(36 + sampleLength * 2);
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16);
        writer.Write((ushort)1);
        writer.Write((ushort)channels);
        writer.Write(frequency);
        writer.Write(frequency * channels * 2);
        writer.Write((ushort)(channels * 2));
        writer.Write((ushort)16);
        writer.Write("data".ToCharArray());
        writer.Write(sampleLength * 2);
    }

    private void WriteWavSamples(BinaryWriter writer, float[] samples) {
        foreach (var sample in samples) {
            writer.Write((short)(sample * short.MaxValue));
        }
    }
}
