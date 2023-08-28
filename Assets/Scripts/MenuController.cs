using System;
using System.Threading.Tasks;
using DSPLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
  public AudioClip clip;
  public TextMeshProUGUI errorText;
  public TextMeshProUGUI loadingText;
  public TMP_InputField input;
  public Button startButton;
  private static readonly string defaultURL = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

  public async void OnStartButtonClick()
  {
    loadingText.gameObject.SetActive(true);
    errorText.gameObject.SetActive(false);
    input.interactable = false;
    startButton.interactable = false;
    try
    {
      var url = string.IsNullOrWhiteSpace(input.text) ? defaultURL : input.text;
      // Global.currentSong = await Util.RetryIfFail(async () => await Util.GetAudioClip(url));
      Global.currentSong = clip;
      await AnalyzeSong();
      SceneManager.LoadScene(sceneName: "Game");
    }
    catch (Exception)
    {
      loadingText.gameObject.SetActive(false);
      errorText.gameObject.SetActive(true);
      input.interactable = true;
      startButton.interactable = true;
    }
  }

  public struct ClipData
  {
    public int channels;
    public int samples;
    public int frequency;
    public float[] multiChannelSamples;
  }

  private async Task AnalyzeSong()
  {
    ClipData clip = new()
    {
      multiChannelSamples = new float[Global.currentSong.samples * Global.currentSong.channels],
      channels = Global.currentSong.channels,
      samples = Global.currentSong.samples,
      frequency = Global.currentSong.frequency
    };
    Global.currentSong.GetData(clip.multiChannelSamples, 0);
    await Task.Run(() => GetFullSpectrum(clip));
  }

  /** Code snippets taken from: https://medium.com/@jesse_87798/d41c339c135a */
  public void GetFullSpectrum(ClipData clip)
  {
    Global.sfAnalyzer = new();
    try
    {
      var preProcessedSamples = new float[clip.samples];

      var numProcessed = 0;
      var combinedChannelAverage = 0f;
      for (var i = 0; i < clip.multiChannelSamples.Length; i++)
      {
        combinedChannelAverage += clip.multiChannelSamples[i];

        // Each time we have processed all channels samples for a point in time, we will store the average of the channels combined
        if ((i + 1) % clip.channels == 0)
        {
          preProcessedSamples[numProcessed] = combinedChannelAverage / clip.channels;
          numProcessed++;
          combinedChannelAverage = 0f;
        }
      }

      // Once we have our audio sample data prepared, we can execute an FFT to return the spectrum data over the time domain
      var spectrumSampleSize = 1024;
      var iterations = preProcessedSamples.Length / spectrumSampleSize;

      var fft = new FFT();
      fft.Initialize((uint)spectrumSampleSize);

      var sampleChunk = new double[spectrumSampleSize];
      for (int i = 0; i < iterations; i++)
      {
        // Grab the current 1024 chunk of audio sample data
        Array.Copy(preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

        // Apply our chosen FFT Window
        var windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint)spectrumSampleSize);
        var scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
        var scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

        // Perform the FFT and convert output (complex numbers) to Magnitude
        var fftSpectrum = fft.Execute(scaledSpectrumChunk);
        var scaledFFTSpectrum = DSP.ConvertComplex.ToMagnitude(fftSpectrum);
        scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

        // These 1024 magnitude values correspond (roughly) to a single point in the audio timeline
        var curSongTime = i * spectrumSampleSize / (float) clip.frequency;

        // Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks
        Global.sfAnalyzer.AnalyzeSpectrum(Array.ConvertAll(scaledFFTSpectrum, x => (float)x), curSongTime, scaledSpectrumChunk);
      }
    }
    catch (Exception e)
    {
      Debug.Log(e.ToString());
    }
  }
}
