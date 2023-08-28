using UnityEngine;

public static class Global
{
  public static AudioClip currentSong;
  public static SpectralFluxAnalyzer sfAnalyzer;
  public static FrequencyBandAnalyzer fbAnalyzer;

  public static int IndexFromTime(float curTime)
  {
    float lengthPerSample = currentSong.length / currentSong.samples;
    return Mathf.FloorToInt(curTime / lengthPerSample);
  }
}