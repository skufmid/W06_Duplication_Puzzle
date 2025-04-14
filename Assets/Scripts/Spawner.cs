using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private void OnMouseDown()
    {
        int playerLayer = LayerMask.GetMask("Player");
        int ObstacleLayer = LayerMask.GetMask("Obstacle");

        Collider2D hit = Physics2D.OverlapBox(transform.position, transform.localScale, 0f, playerLayer + ObstacleLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            Debug.Log("플레이어가 충돌 중이므로 클릭 무시");
            return;
        }
        Spawn();
    }

    public void Spawn()
    {
        if (gameObject.scene.isLoaded)
        {
            Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
