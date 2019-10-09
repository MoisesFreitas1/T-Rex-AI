using UnityEngine;

//Criado por Moises em 28/08/2019

public class ObstacleController : MonoBehaviour
{
    public float speed;
    public float height;
    public float width;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(1,0,0) * speed * Time.deltaTime);
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetHeight()
    {
        return (height + gameObject.transform.position.y);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public float GetWidth()
    {
        return width;
    }
}
