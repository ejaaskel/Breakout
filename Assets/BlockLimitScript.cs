using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLimitScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().BlockLimitReached();
        }
    }
}
