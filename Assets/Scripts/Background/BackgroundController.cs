using UnityEngine;

public class BackgroundController : MonoBehaviour
{
  public AudioSource audioSource;
  public int numToSpawn;
  public float radius = 2;
  public GameObject backgroundObject;
  public Vector3 scale = Vector3.up;
  public Color emissionColor;
  public float colorIntensity = 1;
  GameObject[] bgObjects;

  // Start is called before the first frame update
  void Start()
  {
    Global.fbAnalyzer = new();
    bgObjects = new GameObject[numToSpawn];

    float angleSpacing = 2f * Mathf.PI / numToSpawn;

    for (int i = 0; i < bgObjects.Length; i++)
    {
      float angle = i * angleSpacing;
      float x = Mathf.Sin(angle) * radius;
      float y = Mathf.Cos(angle) * radius;

      var newObject = Instantiate(backgroundObject);
      newObject.transform.SetParent(transform);
      newObject.transform.localPosition = new Vector3(x, y, 0);

      newObject.transform.LookAt(transform.position);
      newObject.transform.localRotation *= Quaternion.Euler(-90, 0, 0);

      bgObjects[i] = newObject;
    }

    for (int i = 0; i < bgObjects.Length; i++)
    {
      BackgroundObject objectProps = bgObjects[i].AddComponent<BackgroundObject>();
      objectProps.color = emissionColor;
      objectProps.intensityFactor = colorIntensity;
      objectProps.index = i;
    }
  }

  void Update()
  {
    Global.fbAnalyzer.UpdateBuffer(audioSource);
    for (int i = 0; i < bgObjects.Length; i++)
    {
      bgObjects[i].transform.localScale = backgroundObject.transform.localScale + (scale * Global.fbAnalyzer.frequencyBands[i]);
    }
  }
}
