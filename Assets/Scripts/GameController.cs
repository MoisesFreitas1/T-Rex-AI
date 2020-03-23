using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Criado por Moises em 28/08/2019

public class GameController : MonoBehaviour
{

    public GameObject Rex;
    public GameObject RexCreator;
    public GameObject[] obstacles;
    public GameObject ObstaclesCreator;
    public int nRex;
    public float spawnWait;
    public float speedObstacles;
    public float acelerationObstacles;
    private float points;
    public GameObject Generation;
    public GameObject HighScore;
    public GameObject Score;
    public GameObject RexN;
    public int generation;
    GameObject[] TRexLive;
    //public float[] weights;
    public float[] Bweights;
    public float[] SBweights;
    public int timeScale;
    private int timeScaleAux;
    
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Time.timeScale = timeScale;
        timeScaleAux = timeScale;
        points = 0;
        generation = 1;
        PlayerPrefs.SetFloat("HighScore", 0);
        Generation.GetComponent<Text>().text = " " + generation;
        HighScore.GetComponent<Text>().text = " " + System.Math.Round(PlayerPrefs.GetFloat("HighScore"), 3);
        Score.GetComponent<Text>().text = " " + System.Math.Round(points, 3);

        for (int i = 0; i < nRex; i++)
        {
            float[] weightsStart = new float[16];
            int timePlay = PlayerPrefs.GetInt("timePlay") + 1;
            PlayerPrefs.SetInt("timePlay", timePlay + 1);

            for (int j = 0; j < weightsStart.Length; j++)
            {
                weightsStart[j] = Random.Range(-1f, 1f);
            }
            ////Best Weights
            PlayerPrefs.SetFloat("BW1", weightsStart[0]);
            PlayerPrefs.SetFloat("BW2", weightsStart[1]);
            PlayerPrefs.SetFloat("BW3", weightsStart[2]);
            PlayerPrefs.SetFloat("BW4", weightsStart[3]);
            PlayerPrefs.SetFloat("BW5", weightsStart[4]);
            PlayerPrefs.SetFloat("BW6", weightsStart[5]);
            PlayerPrefs.SetFloat("BW7", weightsStart[6]);
            PlayerPrefs.SetFloat("BW8", weightsStart[7]);
            PlayerPrefs.SetFloat("BW9", weightsStart[8]);
            PlayerPrefs.SetFloat("BW10", weightsStart[9]);
            PlayerPrefs.SetFloat("BW11", weightsStart[10]);
            PlayerPrefs.SetFloat("BW12", weightsStart[11]);
            PlayerPrefs.SetFloat("BW13", weightsStart[12]);
            PlayerPrefs.SetFloat("BW14", weightsStart[13]);
            PlayerPrefs.SetFloat("BW15", weightsStart[14]);
            PlayerPrefs.SetFloat("BW16", weightsStart[15]);

            PlayerPrefs.SetFloat("SBW1", weightsStart[0]);
            PlayerPrefs.SetFloat("SBW2", weightsStart[1]);
            PlayerPrefs.SetFloat("SBW3", weightsStart[2]);
            PlayerPrefs.SetFloat("SBW4", weightsStart[3]);
            PlayerPrefs.SetFloat("SBW5", weightsStart[4]);
            PlayerPrefs.SetFloat("SBW6", weightsStart[5]);
            PlayerPrefs.SetFloat("SBW7", weightsStart[6]);
            PlayerPrefs.SetFloat("SBW8", weightsStart[7]);
            PlayerPrefs.SetFloat("SBW9", weightsStart[8]);
            PlayerPrefs.SetFloat("SBW10", weightsStart[9]);
            PlayerPrefs.SetFloat("SBW11", weightsStart[10]);
            PlayerPrefs.SetFloat("SBW12", weightsStart[11]);
            PlayerPrefs.SetFloat("SBW13", weightsStart[12]);
            PlayerPrefs.SetFloat("SBW14", weightsStart[13]);
            PlayerPrefs.SetFloat("SBW15", weightsStart[14]);
            PlayerPrefs.SetFloat("SBW16", weightsStart[15]);

            Quaternion spawnRotation = Quaternion.identity;
            GameObject rex = Instantiate(Rex, RexCreator.transform.position, spawnRotation);
            rex.GetComponent<PlayerController>().SetWeights(weightsStart);
        }

        speedObstacles = -1;
        acelerationObstacles = -0.1f;
        spawnWait = Random.Range(2f, 4f);
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        speedObstacles = speedObstacles + acelerationObstacles * Time.deltaTime;
        spawnWait = Random.Range(2500f * acelerationObstacles * Time.deltaTime / (speedObstacles), 2700f * acelerationObstacles * Time.deltaTime / (speedObstacles));

        GameObject[] obstaclesScream = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstaclesScream.Length > 0)
        {
            for (int i = 0; i < obstaclesScream.Length; i++)
            {
                if (obstaclesScream[i].transform.position.y.Equals(ObstaclesCreator.transform.position.y))
                {
                    obstaclesScream[i].GetComponent<ObstacleController>().SetSpeed(speedObstacles);
                }
                else
                {
                    obstaclesScream[i].GetComponent<ObstacleController>().SetSpeed(speedObstacles); //Pterodactyl
                }
            }
        }

        TRexLive = GameObject.FindGameObjectsWithTag("Player");
        RexN.GetComponent<Text>().text = " " + TRexLive.Length;
        if(TRexLive.Length != 0)
        {
            float[] weights1 = new float[16];
            //weights = new float[16];
            Bweights = new float[16];
            points = points + (-1 * speedObstacles * Time.deltaTime);
            Score.GetComponent<Text>().text = " " + System.Math.Round(points, 3);
            for (int i = 0; i < TRexLive.Length; i++)
            {
                if (timeScale != timeScaleAux)
                {
                    Time.timeScale = timeScale;
                    timeScaleAux = timeScale;
                    TRexLive[i].GetComponent<PlayerController>().SetTimeScale(timeScale);
                }
                TRexLive[i].GetComponent<PlayerController>().SetPoints(points);
                weights1 = TRexLive[i].GetComponent<PlayerController>().GetWeights();
            }

            if (!weights1.Equals(new float[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})){
                //weights = weights1;
                Bweights = weights1;
            }
            if(TRexLive.Length >= 2)
            {
                int sbest = Random.Range(0, TRexLive.Length);
                SBweights = new float[16];
                SBweights = TRexLive[sbest].GetComponent<PlayerController>().GetWeights();
                while (TRexLive[sbest].GetComponent<PlayerController>().GetWeights().Equals(Bweights))
                {
                    sbest = Random.Range(0, TRexLive.Length);
                    SBweights = TRexLive[sbest].GetComponent<PlayerController>().GetWeights();
                }
            }
        }
        else
        {
            speedObstacles = -1; //Reinicia a velocidade dos obstaculos
            GameObject[] obstaclesToDestroy = GameObject.FindGameObjectsWithTag("Obstacle");
            for (int i = 0; i < obstaclesToDestroy.Length; i++)
            {
                Destroy(obstaclesToDestroy[i]); //Destroi os obstaculos da tela para serem trocados por novos
            }
            if (points > PlayerPrefs.GetFloat("HighScore"))
            {
                PlayerPrefs.SetFloat("HighScore", points);
                PlayerPrefs.SetFloat("BW1", Bweights[0]);
                PlayerPrefs.SetFloat("BW2", Bweights[1]);
                PlayerPrefs.SetFloat("BW3", Bweights[2]);
                PlayerPrefs.SetFloat("BW4", Bweights[3]);
                PlayerPrefs.SetFloat("BW5", Bweights[4]);
                PlayerPrefs.SetFloat("BW6", Bweights[5]);
                PlayerPrefs.SetFloat("BW7", Bweights[6]);
                PlayerPrefs.SetFloat("BW8", Bweights[7]);
                PlayerPrefs.SetFloat("BW9", Bweights[8]);
                PlayerPrefs.SetFloat("BW10", Bweights[9]);
                PlayerPrefs.SetFloat("BW11", Bweights[10]);
                PlayerPrefs.SetFloat("BW12", Bweights[11]);
                PlayerPrefs.SetFloat("BW13", Bweights[12]);
                PlayerPrefs.SetFloat("BW14", Bweights[13]);
                PlayerPrefs.SetFloat("BW15", Bweights[14]);
                PlayerPrefs.SetFloat("BW16", Bweights[15]);

                PlayerPrefs.SetFloat("SBW1", SBweights[0]);
                PlayerPrefs.SetFloat("SBW2", SBweights[1]);
                PlayerPrefs.SetFloat("SBW3", SBweights[2]);
                PlayerPrefs.SetFloat("SBW4", SBweights[3]);
                PlayerPrefs.SetFloat("SBW5", SBweights[4]);
                PlayerPrefs.SetFloat("SBW6", SBweights[5]);
                PlayerPrefs.SetFloat("SBW7", SBweights[6]);
                PlayerPrefs.SetFloat("SBW8", SBweights[7]);
                PlayerPrefs.SetFloat("SBW9", SBweights[8]);
                PlayerPrefs.SetFloat("SBW10", SBweights[9]);
                PlayerPrefs.SetFloat("SBW11", SBweights[10]);
                PlayerPrefs.SetFloat("SBW12", SBweights[11]);
                PlayerPrefs.SetFloat("SBW13", SBweights[12]);
                PlayerPrefs.SetFloat("SBW14", SBweights[13]);
                PlayerPrefs.SetFloat("SBW15", SBweights[14]);
                PlayerPrefs.SetFloat("SBW16", SBweights[15]);
            }
            else
            {
                int nan = 0;
                for(int i = 0; i < Bweights.Length; i++)
                {
                    if (float.IsNaN(Bweights[i]))
                    {
                        nan = nan + 1;
                    }
                }
                if ((points / PlayerPrefs.GetFloat("HighScore") < 0.60f) || nan > 0)
                {
                    //Protocolo Resurect
                    Bweights[0] = PlayerPrefs.GetFloat("BW1");
                    Bweights[1] = PlayerPrefs.GetFloat("BW2");
                    Bweights[2] = PlayerPrefs.GetFloat("BW3");
                    Bweights[3] = PlayerPrefs.GetFloat("BW4");
                    Bweights[4] = PlayerPrefs.GetFloat("BW5");
                    Bweights[5] = PlayerPrefs.GetFloat("BW6");
                    Bweights[6] = PlayerPrefs.GetFloat("BW7");
                    Bweights[7] = PlayerPrefs.GetFloat("BW8");
                    Bweights[8] = PlayerPrefs.GetFloat("BW9");
                    Bweights[9] = PlayerPrefs.GetFloat("BW10");
                    Bweights[10] = PlayerPrefs.GetFloat("BW11");
                    Bweights[11] = PlayerPrefs.GetFloat("BW12");
                    Bweights[12] = PlayerPrefs.GetFloat("BW13");
                    Bweights[13] = PlayerPrefs.GetFloat("BW14");
                    Bweights[14] = PlayerPrefs.GetFloat("BW15");
                    Bweights[15] = PlayerPrefs.GetFloat("BW16");

                    SBweights[0] = PlayerPrefs.GetFloat("SBW1");
                    SBweights[1] = PlayerPrefs.GetFloat("SBW2");
                    SBweights[2] = PlayerPrefs.GetFloat("SBW3");
                    SBweights[3] = PlayerPrefs.GetFloat("SBW4");
                    SBweights[4] = PlayerPrefs.GetFloat("SBW5");
                    SBweights[5] = PlayerPrefs.GetFloat("SBW6");
                    SBweights[6] = PlayerPrefs.GetFloat("SBW7");
                    SBweights[7] = PlayerPrefs.GetFloat("SBW8");
                    SBweights[8] = PlayerPrefs.GetFloat("SBW9");
                    SBweights[9] = PlayerPrefs.GetFloat("SBW10");
                    SBweights[10] = PlayerPrefs.GetFloat("SBW11");
                    SBweights[11] = PlayerPrefs.GetFloat("SBW12");
                    SBweights[12] = PlayerPrefs.GetFloat("SBW13");
                    SBweights[13] = PlayerPrefs.GetFloat("SBW14");
                    SBweights[14] = PlayerPrefs.GetFloat("SBW15");
                    SBweights[15] = PlayerPrefs.GetFloat("SBW16");
                }
            }
            HighScore.GetComponent<Text>().text = " " + System.Math.Round(PlayerPrefs.GetFloat("HighScore"), 3);
            points = 0;
            Score.GetComponent<Text>().text = " " + System.Math.Round(points, 3);

            // Buscar os dois melhores e fazer o crossover e mutation

            for (int i = 0; i < nRex; i++)
            {
                float[] weightsAux = new float[Bweights.Length];
                for(int j = 0; j < Bweights.Length; j++)
                {
                    //Crossover
                    int fatherOrmother = Random.Range(0, 2);
                    float chanceCross = Random.Range(0f, 100f);
                    if (chanceCross < 80f)
                    {
                        if(fatherOrmother == 0)
                        {
                            weightsAux[j] = SBweights[j];
                        }
                        if(fatherOrmother == 1)
                        {
                            weightsAux[j] = Bweights[j];
                        }
                    }
                    else
                    {
                        weightsAux[j] = Bweights[j];
                    }

                    //Mutation
                    float chanceMutation = Random.Range(0f, 100f);
                    if(chanceMutation < 2f) //probabilidade de 10 %
                    {
                        int nWeights = Random.Range(0, Bweights.Length);
                        int[] indexWeights = new int[nWeights];
                        for (int m = 0; m < nWeights; m++)
                        {
                            indexWeights[m] = Random.Range(1, Bweights.Length);
                            for (int l = 0; l < m; l++)
                            {
                                indexWeights[m] = Random.Range(1, Bweights.Length);
                            }
                        }
                        for (int m = 0; m < nWeights; m++)
                        {
                            int index = indexWeights[m];
                            if (float.IsNaN(weightsAux[index]))
                            {
                                weightsAux[index] = Random.Range(-1f, 1f);
                            }
                            float mut = Random.Range(-weightsAux[index], weightsAux[index]);
                            weightsAux[index] = weightsAux[index] + mut;
                        }
                    }
                }
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(Rex, RexCreator.transform.position, spawnRotation).GetComponent<PlayerController>().SetWeights(weightsAux);
            }
            generation = generation + 1;
            Generation.GetComponent<Text>().text = " " + generation;
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(spawnWait / timeScale);
        bool continueSpawn = true;
        while (continueSpawn)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                int choiceObstacle = Random.Range(-1, 5);
                if (choiceObstacle == 0)
                {
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(obstacles[0], ObstaclesCreator.transform.position, spawnRotation).GetComponent<ObstacleController>().SetSpeed(speedObstacles);
                    yield return new WaitForSeconds(spawnWait / timeScale);
                }
                if (choiceObstacle == 1)
                {
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(obstacles[1], ObstaclesCreator.transform.position, spawnRotation).GetComponent<ObstacleController>().SetSpeed(speedObstacles);
                    yield return new WaitForSeconds(spawnWait / timeScale);
                }
                if (choiceObstacle == 2)
                {
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(obstacles[2], ObstaclesCreator.transform.position, spawnRotation).GetComponent<ObstacleController>().SetSpeed(speedObstacles);
                    yield return new WaitForSeconds(spawnWait / timeScale);
                }
                if (choiceObstacle == 3)
                {
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(obstacles[3], ObstaclesCreator.transform.position, spawnRotation).GetComponent<ObstacleController>().SetSpeed(speedObstacles);
                    yield return new WaitForSeconds(spawnWait / timeScale);
                }
                if (choiceObstacle == 4)
                {
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(obstacles[4], ObstaclesCreator.transform.position + new Vector3(0, Random.Range(0f, 1.0f), 0), spawnRotation).GetComponent<ObstacleController>().SetSpeed(speedObstacles);
                    yield return new WaitForSeconds(spawnWait / timeScale); 
                }
            }
        }
    }
}
