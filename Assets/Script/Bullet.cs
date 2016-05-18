using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    private GameObject player;
    private int speed = 10;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");

        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

        rigidbody2D.velocity = new Vector2(speed*player.transform.localScale.x, rigidbody2D.velocity.y);

        Vector2 tmp = transform.localScale;
        tmp.x = player.transform.localScale.x;
        transform.localScale = tmp;

        Destroy(gameObject, 3);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
