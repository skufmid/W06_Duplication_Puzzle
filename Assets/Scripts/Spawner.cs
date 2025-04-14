using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private void OnMouseDown()
    {
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
