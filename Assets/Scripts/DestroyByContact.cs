using UnityEngine;

//Criado por Moises em 28/08/2019

public class DestroyByContact : MonoBehaviour
{
    private bool firstCollision;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            firstCollision = collision.gameObject.GetComponent<PlayerController>().GetFirstCollision();
            if (firstCollision)
            {
                collision.gameObject.GetComponent<PlayerController>().SetFirstCollision(false);
                collision.gameObject.GetComponent<PlayerController>().DeadRex();
                Destroy(collision.gameObject);
            }
        }

        if (collision.tag == "Obstacle")
        {
            if(gameObject.tag != "Obstacle")
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
