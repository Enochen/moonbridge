using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
  public float spawnRadius;
  public GameObject platePrefab;
  public PlayerController playerController;
  public GameObject endScreen;
  AudioSource audioSource;
  List<MeshRenderer> plates;
  float offset;
  async void Start()
  {
    audioSource = GetComponent<AudioSource>();
    audioSource.clip = Global.currentSong;

    var angle = GetRandomAngle();
    plates = new List<MeshRenderer>();
    foreach (var sample in Global.sfAnalyzer.spectralFluxSamples)
    {
      if (!sample.isPeak) continue;
      angle += Random.Range(-Mathf.PI / 4, Mathf.PI / 4) * (0.7f + sample.prunedSpectralFlux);
      var position = new Vector3(
        Mathf.Cos(angle) * spawnRadius,
        Mathf.Sin(angle) * spawnRadius,
        sample.time * 20 + 20);
      var plate = Instantiate(platePrefab, position, Quaternion.identity, transform);
      plates.Add(plate.GetComponent<MeshRenderer>());
    }

    // Delay allows time for player to orient
    await UniTask.Delay(1000);
    audioSource.Play();

    await UniTask.WaitUntil(() => !audioSource.isPlaying && audioSource.time == 0.0f);
    await UniTask.Delay(1500);
    endScreen.SetActive(true);
  }
  private float GetRandomAngle()
  {
    return Random.value * Mathf.PI * 2;
  }

  public void OnMenuButtonClick()
  {
    SceneManager.LoadScene(sceneName: "Menu");
  }
  void Update()
  {
    var change = Global.fbAnalyzer.samples.Max() * 10;
    offset += change;
    foreach (var plate in plates)
    {
      if (plate.IsUnityNull()) continue;
      plate.material.SetFloat("_Offset", offset);
      plate.material.SetFloat("_Brightness", 1 + change);
    }
  }
}
