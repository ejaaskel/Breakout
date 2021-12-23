using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameController : MonoBehaviour
{
    class SpawnBlockInfo
    {
        public GameObject blockGameObject;
        public Vector2 locationInRow;
        public float sizeScale;
        public int hitPoints;

        public SpawnBlockInfo(GameObject blockGameObject, Vector2 locationInRow, float sizeScale, int hitPoints)
        {
            this.blockGameObject = blockGameObject;
            this.locationInRow = locationInRow;
            this.sizeScale = sizeScale;
            this.hitPoints = hitPoints;
        }
    }

    class SpawnLineInfo
    {
        public List<SpawnBlockInfo> blocks;
        //Sequence can be used in the future to indicate that we want to have the next line follow the current one
        bool partOfSequence;
        public int totalHitpoints;
        public SpawnLineInfo()
        {
            blocks = new List<SpawnBlockInfo>();
        }
    }

    private int ballsLeft;
    GameObject ball;
    [SerializeField] GameObject block;
    Vector3 startPosition;

    private int points;
    private float timeBonusCounter;
    private float timeBonusTime;
    private int timeBonusAmount;

    private float blockMoveSpeed; public float GetBlockMoveSpeed() { return blockMoveSpeed; }
    private float dropMoveMultiplier;
    private float dropMoveSpeed; public float GetDropMoveSpeed() { return blockMoveSpeed * dropMoveMultiplier; }
    private float dropSpawnRate; public float GetDropSpawnRate() { return dropSpawnRate; }
    private float spawnCounter;
    private float spawnTime;

    private bool gameRunning;
    private float gameDurationCounter;

    private float breakBallDuration; public float GetBreakBallDuration() { return breakBallDuration; }

    [SerializeField] Canvas mainMenuCanvas;

    [SerializeField] Canvas gameplayCanvas;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI ballsText;

    [SerializeField] Canvas gameOverCanvas;
    TextMeshProUGUI gameOverText;

    private float spawnHeight;

    List<SpawnLineInfo> spawnPatterns;

    private void PreparePatterns()
    {
        //TODO: Perhaps there is a better way of doing this?
        spawnPatterns = new List<SpawnLineInfo>();

        SpawnLineInfo spawnLineInfo = new SpawnLineInfo();
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-2,0), 0.9f, 1));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-1, 0), 0.9f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(0, 0), 0.9f, 3));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(1, 0), 0.9f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(2, 0), 0.9f, 1));

        foreach (SpawnBlockInfo blockInfo in spawnLineInfo.blocks)
        {
            spawnLineInfo.totalHitpoints += blockInfo.hitPoints;
        }

        spawnPatterns.Add(spawnLineInfo);

        spawnLineInfo = new SpawnLineInfo();
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-2, 0), 0.9f, 3));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-1, 0), 0.9f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(0, 0), 0.9f, 1));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(1, 0), 0.9f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(2, 0), 0.9f, 3));

        foreach (SpawnBlockInfo blockInfo in spawnLineInfo.blocks)
        {
            spawnLineInfo.totalHitpoints += blockInfo.hitPoints;
        }

        spawnPatterns.Add(spawnLineInfo);

        spawnLineInfo = new SpawnLineInfo();
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-1.5f, 0), 0.9f, 3));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(0, 0), 0.9f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(1.5f, 0), 0.9f, 3));

        foreach (SpawnBlockInfo blockInfo in spawnLineInfo.blocks)
        {
            spawnLineInfo.totalHitpoints += blockInfo.hitPoints;
        }

        spawnPatterns.Add(spawnLineInfo);

        spawnLineInfo = new SpawnLineInfo();
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-2, 0), 0.4f, 1));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-1.5f, 0), 0.4f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(-1, 0), 0.4f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(0, 0), 0.9f, 3));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(1, 0), 0.4f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(1.5f, 0), 0.4f, 2));
        spawnLineInfo.blocks.Add(new SpawnBlockInfo(block, new Vector2(2, 0), 0.4f, 1));

        foreach (SpawnBlockInfo blockInfo in spawnLineInfo.blocks)
        {
            spawnLineInfo.totalHitpoints += blockInfo.hitPoints;
        }

        spawnPatterns.Add(spawnLineInfo);
    }

    public void AddPoints(int points)
    {
        this.points += points;
    }

    public DropScript.DropType GetNextDropType()
    {
        var values = System.Enum.GetValues(typeof(DropScript.DropType));
        int random = UnityEngine.Random.Range(0, 4);
        return (DropScript.DropType)values.GetValue(random);
    }

    private void DestroyAllBlocks()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            Destroy(block);
        }
    }

    private void DestroyAllDrops()
    {
        GameObject[] drops = GameObject.FindGameObjectsWithTag("Drop");
        foreach (GameObject drop in drops)
        {
            Destroy(drop);
        }
    }

    private void LoseGame()
    {
        ball.GetComponent<BallScript>().StopVelocity();
        ball.GetComponent<SpriteRenderer>().enabled = false;
        blockMoveSpeed = 0;
        DestroyAllBlocks();
        DestroyAllDrops();

        gameOverText.text = "Game over\nFinal score:\n" + points;
        gameRunning = false;

        gameplayCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
    }

    public void ResetGame()
    {
        ball.transform.position = startPosition;
        ball.GetComponent<BallScript>().ResetVelocity();
        ball.GetComponent<SpriteRenderer>().enabled = true;
        ballsLeft = 3;
        gameRunning = true;

        points = 0;
        timeBonusCounter = 0;

        blockMoveSpeed = 0.125f;

        //Spawn one line immediately with high value
        spawnCounter = 999;
        //Spawn block 1 unity unit, regardless of the speed
        spawnTime = 1 / blockMoveSpeed;

        gameDurationCounter = 0;

        mainMenuCanvas.gameObject.SetActive(false);
        gameplayCanvas.gameObject.SetActive(true);
        gameOverCanvas.gameObject.SetActive(false);
    }

    void ResetBall()
    {
        ball.transform.position = startPosition;
        ball.GetComponent<BallScript>().ResetVelocity();
        ball.GetComponent<BallScript>().StopBreakBall();
    }

    public void BlockLimitReached()
    {
        LoseGame();
    }

    public void BallLost()
    {
        //Refresh reference to ball, as multiball may be the last one left
        ball = GameObject.FindGameObjectWithTag("Ball");

        ballsLeft -= 1;
        ResetBall();
        if (ballsLeft == 0)
        {
            LoseGame();
        }
    }

    void SpawnNewRow()
    {
        SpawnLineInfo nextLine = spawnPatterns[Random.Range(0, spawnPatterns.Count)];
        for (int i = 0; i < nextLine.blocks.Count; i++)
        {
            GameObject newBlock = Instantiate(nextLine.blocks[i].blockGameObject, nextLine.blocks[i].locationInRow, Quaternion.identity);
            newBlock.transform.Translate(new Vector3(0, spawnHeight, 0));
            newBlock.transform.localScale = new Vector3(nextLine.blocks[i].sizeScale, nextLine.blocks[i].sizeScale, nextLine.blocks[i].sizeScale);
            newBlock.gameObject.GetComponent<BlockScript>().SetHitpoints(nextLine.blocks[i].hitPoints);
        }

        int differenceBetweenRows = Random.Range(1, 3);
        spawnTime = differenceBetweenRows / blockMoveSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        PreparePatterns();
        ball = GameObject.FindGameObjectWithTag("Ball");

        spawnHeight = 4.65f;

        //These are temporarily initialized here:
        timeBonusTime = 30.0f;
        timeBonusAmount = 100;
        dropSpawnRate = 0.20f;
        dropMoveMultiplier = 3.0f;
        breakBallDuration = 10.0f;
        

        startPosition = new Vector3(0, -2.5f, 0);

        ballsText = gameplayCanvas.transform.Find("BallsText").GetComponent<TextMeshProUGUI>();
        scoreText = gameplayCanvas.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();

        gameOverText = gameOverCanvas.transform.Find("GameOverText").GetComponent<TextMeshProUGUI>();
    }

    private void AdjustDifficulty ()
    {
        int ballsInPlay = GameObject.FindGameObjectsWithTag("Ball").Length;
        int blocksInPlay = GameObject.FindGameObjectsWithTag("Block").Length;

        float paddleWidth = GameObject.FindGameObjectWithTag("Paddle").transform.localScale.x;

        //Line intervals in unity units 4 - 1
        //Block move speed: 0.125 - 1
        //Drop rate (and type)
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            timeBonusCounter += Time.deltaTime;
            if (timeBonusCounter > timeBonusTime)
            {
                AddPoints(timeBonusAmount);
                timeBonusCounter = 0;
            }

            spawnCounter += Time.deltaTime;
            if (spawnCounter > spawnTime)
            {
                SpawnNewRow();
                spawnCounter = 0;
            }

            gameDurationCounter += Time.deltaTime;
        }

        ballsText.text = "Balls: " + ballsLeft;
        scoreText.text = "Score: " + points;
    }
}
