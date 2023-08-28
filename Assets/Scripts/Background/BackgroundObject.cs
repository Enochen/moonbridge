using UnityEngine;

public class BackgroundObject : MonoBehaviour
{
  public Color color;
  public float intensityFactor = 1;
  public int index;

  MeshRenderer meshRenderer;

  private void Start()
  {
    meshRenderer = GetComponent<MeshRenderer>();
  }

  void Update()
  {
    var intensity = Global.fbAnalyzer.frequencyBands[index] * intensityFactor;
    meshRenderer.material.SetColor("_EmissionColor", color * intensity);
  }
}
