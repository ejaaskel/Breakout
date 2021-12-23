using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    float ballSpeed;
    public Vector2 velocity;

    public Vector2 prevVelocity;

    GameController gameController;
    Rigidbody2D myRigidBody;
    bool turning;

    bool breakBall = false; public bool GetBreakBall() { return breakBall; }
    private float breakBallLeft;

    bool stopVelocityOnStart = true;

    float hitFactor(Vector2 ballPos, Vector2 racketPos,
                float racketWidth)
    {
        // ascii art:
        //
        // 1  -0.5  0  0.5   1  <- x value
        // ===================  <- racket
        //
        return (ballPos.x - racketPos.x) / racketWidth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle") || collision.gameObject.CompareTag("PaddleBottom"))
        {
            float x = hitFactor(transform.position,
                              collision.transform.position,
                              collision.collider.bounds.size.x);

            myRigidBody.velocity = new Vector2(x * 7, myRigidBody.velocity.y).normalized * ballSpeed;
        }
        else if (collision.gameObject.tag.Contains("Block"))
        {
                collision.gameObject.GetComponent<BlockScript>().TakeDamage(1, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Deathzone"))
        {
            if (GameObject.FindGameObjectsWithTag("Ball").Length == 1)
            {
                gameController.BallLost();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("BlockTrigger"))
        {
            if (breakBall)
            {
                collision.transform.parent.gameObject.GetComponent<BlockScript>().InstaKill();
            }
        }
    }

    public void InitBreakBall()
    {
        breakBall = true;
        breakBallLeft = gameController.GetBreakBallDuration();
        GetComponent<SpriteRenderer>().color = Color.red;

        //BreakBall has separate physics layer
        //This is done to prevent slight nudging when hitting a collider
        //so triggers are being used instead for BreakBall
        gameObject.layer = LayerMask.NameToLayer("BreakBall");
    }

    public void StopBreakBall()
    {
        breakBall = false;
        breakBallLeft = 0.0f;
        GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void InitMultiBall(Vector2 oldVelocity, bool breakBall, float breakBallLeft)
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        if (oldVelocity.y < 0)
        {
            //Random.Range(0, 2) * 2 - 1 creates randomly -1 or 1
            myRigidBody.velocity = new Vector2(oldVelocity.x * (Random.Range(0, 2) * 2 - 1), Mathf.Abs(oldVelocity.y));
        }
        else
        {
            myRigidBody.velocity = new Vector2(oldVelocity.x * -1, oldVelocity.y);
            if (myRigidBody.velocity.y > 0.85)
            {
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, -myRigidBody.velocity.y);
            }
        }
        stopVelocityOnStart = false;

        this.breakBall = breakBall;
        this.breakBallLeft = breakBallLeft;
    }

    public void StopVelocity()
    {
        myRigidBody.velocity = Vector2.zero;
    }

    public void ResetVelocity()
    {
        myRigidBody.velocity = Vector2.zero;
        myRigidBody.AddForce(Vector2.up);
    }

    public void CreateMultiBall()
    {
        GameObject newBall = Instantiate(gameObject, transform.position, Quaternion.identity);
        newBall.GetComponent<BallScript>().InitMultiBall(myRigidBody.velocity, breakBall, breakBallLeft);
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        ballSpeed = 5;
        turning = false;

        if (stopVelocityOnStart)
        {
            StopVelocity();
        }
    }

    private void FixedUpdate()
    {
        //Normalize velocity
        myRigidBody.velocity = myRigidBody.velocity.normalized * ballSpeed;
        prevVelocity = myRigidBody.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (breakBall)
        {
            breakBallLeft -= Time.deltaTime;
            if (breakBallLeft <= 0)
            {
                StopBreakBall();
            }
        }
    }
}
