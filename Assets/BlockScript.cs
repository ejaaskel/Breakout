using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    int hitPoints = 0;
    GameController gameController;

    [SerializeField] GameObject drop;

    private void AwardPoints()
    {
        int pointsToAward = 0;
        pointsToAward += 10;
        if (hitPoints <= 0)
        {
            pointsToAward += 50;
        }
        gameController.AddPoints(pointsToAward);
    }

    public void SetHitpoints(int hitPoints)
    {
        this.hitPoints = hitPoints;
        SetColor();
    }

    public void InstaKill()
    {
        TakeDamage(hitPoints, gameObject);
    }

    public void TakeDamage(int damage, GameObject damager)
    {
        hitPoints -= damage;


        AwardPoints();

        if (hitPoints <= 0)
        {
            float spawnDrop = Random.Range(0.0f, 1.0f);
            if (spawnDrop < gameController.GetDropSpawnRate() && damager.CompareTag("Ball") && !damager.GetComponent<BallScript>().GetBreakBall())
            {
                Instantiate(drop, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
            return;
        }
        SetColor();
    }

    void SetColor()
    {
        //Colors to indicate hit points (straight out of bloons tower defense
        //Red, blue green yellow, pink purple, light blue, black, white
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch (hitPoints)
        {
            case 1:
                spriteRenderer.color = Color.red;
                break;
            case 2:
                spriteRenderer.color = Color.blue;
                break;
            case 3:
                spriteRenderer.color = Color.green;
                break;
            case 4:
                spriteRenderer.color = Color.yellow;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //DEBUG blocks may have 0 hitpoints on start, otherwise this shouldn't happen as GameContoller sets
        //hitpoints right after instantiation
        if (hitPoints == 0)
        {
            hitPoints = 2;
        }
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime*gameController.GetBlockMoveSpeed());
    }
}
