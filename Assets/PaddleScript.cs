using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleScript : MonoBehaviour
{
    Vector2 velocity;
    float moveSpeed;

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 10;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = GetComponent<Rigidbody2D>().velocity;

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon)
        {
            float newXPos = transform.position.x + Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            //TODO: These hardcoded values are not too good
            if (-2.64 < newXPos && newXPos < 2.64)
            {
                transform.Translate(new Vector2(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0));
            }
        }
    }
}
