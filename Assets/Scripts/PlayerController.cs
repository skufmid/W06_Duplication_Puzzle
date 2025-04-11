using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int PlayerId; 

    Vector3 move;
    public float Speed;
    public int Direction;
    void FixedUpdate()
    {
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        transform.position += move * Speed * Time.deltaTime * Direction;

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GameManager.Instance.SpawnPlayer();
        //}
        
    }
}
