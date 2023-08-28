using UnityEngine;

public class PauseController : MonoBehaviour
{
  public AudioSource audioSource;
  public GameObject pauseScreen, endScreen;
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (endScreen.activeSelf) return;
      if (pauseScreen.activeSelf)
      {
        CloseMenu();
      }
      else
      {
        OpenMenu();
      }
    }
  }
  public void OpenMenu()
  {
    Time.timeScale = 0;
    audioSource.Pause();
    pauseScreen.SetActive(true);
  }

  public void CloseMenu()
  {
    Time.timeScale = 1;
    audioSource.UnPause();
    pauseScreen.SetActive(false);
  }
}
