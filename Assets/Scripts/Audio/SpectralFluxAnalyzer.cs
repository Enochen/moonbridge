using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/** Code snippets taken from: https://medium.com/@jesse_87798/d41c339c135a */

public class SpectralFluxInfo
{
  public float time;
  public float spectralFlux;
  public float threshold;
  public float prunedSpectralFlux;
  public double[] chunk;
  public bool isPeak;
}

public class SpectralFluxAnalyzer
{
  readonly int numSamples = 1024;

  // Sensitivity multiplier to scale the average threshold
  readonly float thresholdMultiplier = 2f;

  // Number of samples to average in our window
  readonly int thresholdWindowSize = 50;

  public List<SpectralFluxInfo> spectralFluxSamples;
  readonly float[] curSpectrum;
  readonly float[] prevSpectrum;

  int indexToProcess;

  public SpectralFluxAnalyzer()
  {
    spectralFluxSamples = new List<SpectralFluxInfo>();
    curSpectrum = new float[numSamples];
    prevSpectrum = new float[numSamples];
    indexToProcess = thresholdWindowSize / 2;
  }

  public void SetCurSpectrum(float[] spectrum)
  {
    curSpectrum.CopyTo(prevSpectrum, 0);
    spectrum.CopyTo(curSpectrum, 0);
  }

  public void AnalyzeSpectrum(float[] spectrum, float time, double[] chunk)
  {
    SetCurSpectrum(spectrum);
    spectralFluxSamples.Add(new()
    {
      time = time,
      chunk = chunk,
      spectralFlux = CalculateRectifiedSpectralFlux()
    });

    if (spectralFluxSamples.Count < thresholdWindowSize) return;
    spectralFluxSamples[indexToProcess].threshold = GetFluxThreshold(indexToProcess);
    spectralFluxSamples[indexToProcess].prunedSpectralFlux = GetPrunedSpectralFlux(indexToProcess);
    int prevIndex = indexToProcess - 1;
    if (IsPeak(prevIndex))
    {
      spectralFluxSamples[prevIndex].isPeak = true;
    }
    indexToProcess++;
  }

  float CalculateRectifiedSpectralFlux() => curSpectrum.Zip(prevSpectrum, (cur, prev) => Mathf.Max(0f, cur - prev)).Sum();

  float GetFluxThreshold(int spectralFluxIndex)
  {
    int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
    int windowEndIndex = Mathf.Min(spectralFluxSamples.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);
    int windowSize = windowEndIndex - windowStartIndex;

    var sum = spectralFluxSamples.GetRange(windowStartIndex, windowSize).Sum(sample => sample.spectralFlux);
    return sum * thresholdMultiplier / windowSize;
  }

  float GetPrunedSpectralFlux(int spectralFluxIndex) => Mathf.Max(0f, spectralFluxSamples[spectralFluxIndex].spectralFlux - spectralFluxSamples[spectralFluxIndex].threshold);

  bool IsPeak(int spectralFluxIndex) => spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
      spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux;
}