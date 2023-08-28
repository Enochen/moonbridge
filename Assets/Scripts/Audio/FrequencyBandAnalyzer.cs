using UnityEngine;

/** Modified from youtube tutorial: https://www.youtube.com/watch?v=ruq1Rfj7V3k */
public class FrequencyBandAnalyzer
{
  public AudioSource audioSource;
  readonly static int binsCount = 512;
  public readonly float[] samples = new float[binsCount];
  readonly float[] sampleBuffer = new float[binsCount];

  public float smoothFactor = 6;
  public float scalar = 1;

  public float[] frequencyBands = new float[64];

  void UpdateFreqBands()
  {
    int count = 0;
    int sampleCount = 1;
    int power = 0;

    for (int i = 0; i < 64; i++)
    {
      float average = 0;

      if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
      {
        power++;
        sampleCount = (int)Mathf.Pow(2, power);
        if (power == 3)
          sampleCount -= 2;
      }

      for (int j = 0; j < sampleCount; j++)
      {
        average += samples[count] * (count + 1);
        count++;
      }

      average /= count;
      frequencyBands[i] = average;
    }
  }

  public void UpdateBuffer(AudioSource audioSource)
  {
    audioSource.GetSpectrumData(sampleBuffer, 0, FFTWindow.BlackmanHarris);
    for (int i = 0; i < samples.Length; i++)
    {
      if (sampleBuffer[i] > samples[i])
      {
        samples[i] = sampleBuffer[i];
      }
      else
      {
        samples[i] = Mathf.Lerp(samples[i], sampleBuffer[i], Time.deltaTime * smoothFactor);
      }
    }
    UpdateFreqBands();
  }
}
