using UnityEngine;
using System.Collections;

public class Anime : MonoBehaviour {

    public static Vector2 offset = new Vector2(0, 0.75f);
    int frame;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        frame++;

        if (frame % 6 == 0)
        {
            offset.x += 0.25f;

            if(offset.x >= 1){
                offset.x = 0;
            }
        }

        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
	}
}
