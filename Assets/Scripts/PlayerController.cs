using UnityEngine;

//Criado por Moises em 28/08/2019

public class PlayerController : MonoBehaviour
{
    public float[] weights;
    private float[] theta;
    public float[] outIntern;
    public float[] outExtern;
    public float[] distance, height, speed, width, heightJump;
    private ObstacleController obstacleController;
    private float points;
    public float forceJump;
    private new Rigidbody2D rigidbody2D;
    private Animator animator;
    private BoxCollider2D[] boxes;
    private bool firstCollision;
    private bool isgrounded;
    private int timeScale;
    private float EPSLON = 0.001f;

    void Start()
    {
        firstCollision = true;
        isgrounded = true;
        theta = new float[5];
        for(int i = 0; i < theta.Length; i++)
        {
            theta[i] = 1;
        }
        outIntern = new float[2];
        outExtern = new float[3];
        points = 0;
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        boxes = gameObject.GetComponents<BoxCollider2D>();
        boxes[0].enabled = true;
        boxes[1].enabled = false;
        animator = GetComponent<Animator>();
        animator.SetBool("runDown", false);
        animator.SetBool("jumping", false);
        animator.SetBool("run", true);
        animator.SetBool("dead", false);
    }

    void Update()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle"); //procura os obstaculos na tela
        if(obstacles.Length > 0)
        {
            distance = new float[obstacles.Length];
            height = new float[obstacles.Length];
            speed = new float[obstacles.Length];
            width = new float[obstacles.Length];
            heightJump = new float[obstacles.Length];
            int ind = 0;
            float minorDistance = 1000f;
            for(int i = 0; i < obstacles.Length; i++)
            {
                obstacleController = obstacles[i].GetComponent<ObstacleController>();
                height[i] = obstacles[i].transform.position.y; //+ obstacleController.GetHeight() 
                speed[i] = obstacleController.GetSpeed();
                width[i] = obstacleController.GetWidth();
                distance[i] = obstacles[i].transform.position.x - gameObject.transform.position.x;
                heightJump[i] = gameObject.transform.position.y;
            }
            for (int j = 0; j < obstacles.Length; j++)
            {
                if(distance[j] > 0)
                {
                    if (distance[j] < minorDistance)
                    {
                        minorDistance = distance[j];
                        ind = j;
                    }
                }
            }
            NeuralNetwork(distance[ind], height[ind], speed[ind], width[ind], heightJump[ind]);
        }
    }

    private void FixedUpdate()
    {
        if (outExtern[0] > outExtern[1] && outExtern[0] > outExtern[2])
        {
            //Deve pular
            if (isgrounded)
            {
                rigidbody2D.AddForce(new Vector3(0, forceJump, 0), ForceMode2D.Impulse);
            }
            boxes[0].enabled = true;
            boxes[1].enabled = false;
            animator.SetBool("runDown", false);
            animator.SetBool("jumping", true);
            animator.SetBool("run", false);
            animator.SetBool("dead", false);
        }

        if (outExtern[1] > outExtern[0] && outExtern[1] > outExtern[2])
        {
            //Deve se abaixar
            if (!isgrounded)
            {
                rigidbody2D.AddForce(new Vector3(0, -forceJump, 0), ForceMode2D.Impulse);
            }
            boxes[0].enabled = false;
            boxes[1].enabled = true;
            animator.SetBool("runDown", true);
            animator.SetBool("jumping", false);
            animator.SetBool("run", false);
            animator.SetBool("dead", false);
        }

        if (outExtern[2] > outExtern[0] && outExtern[2] > outExtern[0])
        {
            //Deve ficar ereto
            boxes[0].enabled = true;
            boxes[1].enabled = false;
            animator.SetBool("runDown", false);
            animator.SetBool("jumping", false);
            animator.SetBool("run", true);
            animator.SetBool("dead", false);
        }
    }

    private void NeuralNetwork(float d, float h, float s, float w, float hJ)
    {
        float net1 = (weights[0] * d) + (weights[1] * h) + (weights[2] * s) + (weights[3] * w) + (weights[4] * hJ) + theta[0];
        float net2 = (weights[5] * d) + (weights[6] * h) + (weights[7] * s) + (weights[8] * w) + (weights[9] * hJ) + theta[1];

        outIntern[0] = FunctionTransfer(net1);
        outIntern[1] = FunctionTransfer(net2);

        float net3 = (weights[10] * outIntern[0]) + (weights[11] * outIntern[1]) + theta[2];
        float net4 = (weights[12] * outIntern[0]) + (weights[13] * outIntern[1]) + theta[3];
        float net5 = (weights[14] * outIntern[0]) + (weights[15] * outIntern[1]) + theta[4];

        outExtern[0] = FunctionTransfer(net3);
        outExtern[1] = FunctionTransfer(net4);
        outExtern[2] = FunctionTransfer(net5);


        float[] deltaks = new float[3];

        float dmin = s * Mathf.Sqrt(2 * h / forceJump); // distancia minima: baseado na cinematica

        if(h < 2.1f && d < (2 * dmin)) // Deveria pular
        {
            deltaks[0] = (0.6f - outExtern[0]) * DotFunctionTransfer(net3); // Pular
            deltaks[1] = (0.4f - outExtern[1]) * DotFunctionTransfer(net4); // Ereto
            deltaks[2] = (0.4f - outExtern[2]) * DotFunctionTransfer(net5); // Agachar
        }
        if (d >= (2 * dmin))
        {
            deltaks[0] = (0.4f - outExtern[0]) * DotFunctionTransfer(net3); // Pular
            deltaks[1] = (0.6f - outExtern[1]) * DotFunctionTransfer(net4); // Ereto
            deltaks[2] = (0.4f - outExtern[2]) * DotFunctionTransfer(net5); // Agachar
        }
        if(h > 2.1f && d < (2 * dmin))
        {
            deltaks[0] = (0.4f - outExtern[0]) * DotFunctionTransfer(net3); // Pular
            deltaks[1] = (0.4f - outExtern[1]) * DotFunctionTransfer(net4); // Ereto
            deltaks[2] = (0.6f - outExtern[2]) * DotFunctionTransfer(net5); // Agachar
        }

        theta[2] = theta[2] + 0.0001f * deltaks[0];
        theta[3] = theta[3] + 0.0001f * deltaks[1];
        theta[4] = theta[4] + 0.0001f * deltaks[2];


        float[] Er = new float[3];
        Er[0] = (Mathf.Pow(deltaks[0], 2)) / 2f;
        Er[1] = (Mathf.Pow(deltaks[1], 2)) / 2f;
        Er[2] = (Mathf.Pow(deltaks[2], 2)) / 2f;
        float er = Er[0] + Er[1] + Er[2];

        if (er > EPSLON)
        {
            float[] delta0 = new float[2];
            delta0[0] = DotFunctionTransfer(net1) * (deltaks[0] * weights[10] + deltaks[1] * weights[12] + deltaks[2] * weights[14]);
            delta0[1] = DotFunctionTransfer(net2) * (deltaks[0] * weights[11] + deltaks[1] * weights[13] + deltaks[2] * weights[15]);

            weights[0] = weights[0] + 0.0001f * delta0[0] * d;
            weights[1] = weights[1] + 0.0001f * delta0[0] * h;
            weights[2] = weights[2] + 0.0001f * delta0[0] * s;
            weights[3] = weights[3] + 0.0001f * delta0[0] * w;
            weights[4] = weights[4] + 0.0001f * delta0[0] * hJ;

            weights[5] = weights[5] + 0.0001f * delta0[1] * d;
            weights[6] = weights[6] + 0.0001f * delta0[1] * h;
            weights[7] = weights[7] + 0.0001f * delta0[1] * s;
            weights[8] = weights[8] + 0.0001f * delta0[1] * w;
            weights[9] = weights[9] + 0.0001f * delta0[1] * hJ;

            weights[10] = weights[10] + 0.0001f * deltaks[0] * outIntern[0];
            weights[11] = weights[11] + 0.0001f * deltaks[0] * outIntern[1];

            weights[12] = weights[12] + 0.0001f * deltaks[1] * outIntern[0];
            weights[13] = weights[13] + 0.0001f * deltaks[1] * outIntern[1];

            weights[14] = weights[14] + 0.0001f * deltaks[2] * outIntern[0];
            weights[15] = weights[15] + 0.0001f * deltaks[2] * outIntern[1];

            theta[0] = theta[0] + 0.0001f * delta0[0];
            theta[1] = theta[1] + 0.0001f * delta0[1];
        }
    }

    private float FunctionTransfer(float x)
    {
        float f = (1) / (1 + Mathf.Exp(-x));
        return f;
    }

    private float DotFunctionTransfer(float x)
    {
        float f_ = Mathf.Exp(-x) / Mathf.Pow((1 + Mathf.Exp(-x)), 2);
        return f_;
    }

    public void SetWeights(float[] weights)
    {
        this.weights = weights;
    }

    public float[] GetWeights()
    {
        return weights;
    }

    public float GetPoints()
    {
        return points;
    }

    public void SetPoints(float points)
    {
        this.points = points;
    }

    public void DeadRex()
    {
        // Deve morrer
        animator.SetBool("runDown", false);
        animator.SetBool("jumping", false);
        animator.SetBool("run", false);
        animator.SetBool("dead", true);
    }

    public BoxCollider2D[] GetBoxes()
    {
        return boxes;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(boxes[0], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[0]);
            Physics2D.IgnoreCollision(boxes[0], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[1]);
            Physics2D.IgnoreCollision(boxes[1], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[0]);
            Physics2D.IgnoreCollision(boxes[1], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[1]);
        }

        if(collision.gameObject.tag == "Ground")
        {
            isgrounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(boxes[0], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[0]);
            Physics2D.IgnoreCollision(boxes[0], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[1]);
            Physics2D.IgnoreCollision(boxes[1], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[0]);
            Physics2D.IgnoreCollision(boxes[1], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[1]);
        }

        if (collision.gameObject.tag == "Ground")
        {
            isgrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(boxes[0], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[0]);
            Physics2D.IgnoreCollision(boxes[0], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[1]);
            Physics2D.IgnoreCollision(boxes[1], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[0]);
            Physics2D.IgnoreCollision(boxes[1], collision.gameObject.GetComponent<PlayerController>().GetBoxes()[1]);
        }

        if (collision.gameObject.tag == "Ground")
        {
            isgrounded = false;
        }
    }

    public bool GetFirstCollision()
    {
        return firstCollision;
    }

    public void SetFirstCollision(bool firstCol)
    {
        this.firstCollision = firstCol;
    }

    public void SetTimeScale(int timeScale)
    {
        Time.timeScale = timeScale;
    }

}
