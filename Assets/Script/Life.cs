using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour {

    public static int lifeCount = 3;
    public Text lifeText;

    public GameObject player;
    public Text gameOvertext;
    private bool gameOver = false;

	// Use this for initialization
	void Start()
    {
        lifeText.text = "HP: " + lifeCount;
	}
	
    public void LifeDown()
    {
        lifeCount -= 1;
        if (lifeCount >= 0) {
            lifeText.text = "HP: " + lifeCount;
        }
    }

	// Update is called once per frame
	void Update () {
        if (lifeCount <= 0) {
            if(gameOver == false)
            {
                GameOver();
            }
        }

        if (gameOver)
        {
            gameOvertext.enabled = true;

            if (Input.GetMouseButton(0))
            {
                SceneManager.LoadScene("Title");
            }
        }
	}

    public void GameOver()
    {
        gameOver = true;
        Destroy(player);
        lifeCount = 3;
    }
}
