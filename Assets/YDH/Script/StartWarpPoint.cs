using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class StartWarpPoint : MonoBehaviour
{
    public GameObject EndWarpPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("¹«¾ð°¡ ´êÀ½");

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.position = EndWarpPoint.transform.position;
        }

        this.gameObject.SetActive(false);
        EndWarpPoint.SetActive(false);
    }
}
