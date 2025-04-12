using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class StartWarpPoint : MonoBehaviour
{
    public GameObject EndWarpPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("���� ����");

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.position = EndWarpPoint.transform.position;
        }

        EndWarpPoint.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
