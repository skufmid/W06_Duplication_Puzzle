using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public int remainCount;
    private TextMeshPro remainCountText;

    private void Awake()
    {
        remainCountText = GetComponentInChildren<TextMeshPro>();
        UpdateText();
    }

    private void OnMouseDown()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (remainCount <= 0) return;

        remainCount--;
        Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
        UpdateText();

        if (remainCount == 0)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateText()
    {
        remainCountText.text = remainCount.ToString();
    }
}
