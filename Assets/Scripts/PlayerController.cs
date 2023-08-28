using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Transform cube;
  public Camera gameCamera;
  public float radius;
  public Vector3 velocity;
  public LayerMask obstacles;
  public ParticleSystem particles;
  public TrailRenderer trail;
  private float cameraDistance;
  private int score;

  void Start()
  {
    cameraDistance = Vector3.Distance(gameCamera.transform.position, transform.position);
  }

  void Update()
  {
    if(Time.timeScale == 0) return;
    var mousePos = Input.mousePosition;
    var worldPos = gameCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cameraDistance));
    worldPos.z = 0;
    cube.localPosition = LimitToRadius(worldPos, Vector3.zero);
    transform.position += velocity * Time.deltaTime;
    trail.transform.localPosition = new Vector3(Mathf.PingPong(Time.time, 0.1f) - 0.1f, 0, 0);
  }

  Vector2 LimitToRadius(Vector3 position, Vector3 center)
  {
    var dist = Vector3.Distance(position, center);
    return dist <= radius ? position : position.normalized * radius;
  }

  void OnTriggerEnter(Collider other)
  {
    if (!other.gameObject.IsInLayerMask(obstacles)) return;
    particles.Play();
    score += 3;
    GetComponentInChildren<MeshRenderer>().material.SetFloat("_Score", score);
    Destroy(other.gameObject);
  }
}
