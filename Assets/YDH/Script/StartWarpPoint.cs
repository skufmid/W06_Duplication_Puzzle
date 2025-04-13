using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class StartWarpPoint : MonoBehaviour
{
    public GameObject[] Players;
    public GameObject EndWarpPoint;
    private int prefabId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("무언가 닿음");

        if (collision.gameObject.CompareTag("Player"))
        {
            prefabId = collision.GetComponent<PlayerController>().PlayerId;
            Instantiate(Players[prefabId], EndWarpPoint.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }

        EndWarpPoint.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
