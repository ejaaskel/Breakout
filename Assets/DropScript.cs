using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropScript : MonoBehaviour
{
    public enum DropType
    {
        WidenPaddle,
        MultiBall,
        BreakBall,
        ShrinkPaddle
    }

    GameController gameController;

    DropType dropType;
    float widthModifier = 0.2f;

    private void DoDropEffect(GameObject gameObjectHit)
    {
        switch (dropType)
        {
            case DropType.WidenPaddle:
                GameObject paddle = GameObject.FindGameObjectWithTag("Paddle");
                float width = paddle.transform.localScale.x;
                width += widthModifier;
                paddle.transform.localScale = new Vector3(width, paddle.transform.localScale.y, paddle.transform.localScale.z);
                break;
            case DropType.MultiBall:
                GameObject ball = null;
                if (gameObjectHit.CompareTag("Ball"))
                {
                    ball = gameObjectHit;
                }
                else
                {
                    GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
                    ball = balls[Random.Range(0, balls.Length)];
                }

                ball.GetComponent<BallScript>().CreateMultiBall();
                break;
            case DropType.BreakBall:
                ball = null;
                if (gameObjectHit.CompareTag("Ball"))
                {
                    ball = gameObjectHit;
                }
                else
                {
                    GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
                    foreach (GameObject ballInPlay in balls) {
                        if (!ballInPlay.GetComponent<BallScript>().GetBreakBall())
                        {
                            ball = ballInPlay;
                        }
                        //Select last ball if the list been iterated through, this'll just extend the effect then
                        if (ballInPlay == balls[balls.Length - 1])
                        {
                            ball = ballInPlay;
                        }
                    }
                }

                if (ball == null)
                {
                    Debug.Log("SHOULDN*T BE HERE!!!!");
                    ball = GameObject.FindGameObjectWithTag("Ball");
                }

                ball.GetComponent<BallScript>().InitBreakBall();
                break;
            case DropType.ShrinkPaddle:
                paddle = GameObject.FindGameObjectWithTag("Paddle");
                width = paddle.transform.localScale.x;
                width -= widthModifier;
                paddle.transform.localScale = new Vector3(width, paddle.transform.localScale.y, paddle.transform.localScale.z);
                break;
        }
    }

    private void SetDropType(DropType type)
    {
        dropType = type;
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        switch(type)
        {
            case DropType.WidenPaddle:
                spriteRenderer.color = Color.cyan;
                break;
            case DropType.MultiBall:
                spriteRenderer.color = Color.gray;
                break;
            case DropType.BreakBall:
                spriteRenderer.color = Color.magenta;
                break;
            case DropType.ShrinkPaddle:
                spriteRenderer.color = Color.yellow;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle") || collision.gameObject.CompareTag("Ball"))
        {
            DoDropEffect(collision.gameObject);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        SetDropType(gameController.GetNextDropType());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * gameController.GetDropMoveSpeed());
    }
}
