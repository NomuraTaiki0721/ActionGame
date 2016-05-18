using UnityEngine;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	RaycastHit2D hit1, hit2, hit3;
	bool isGround;
	int jumpCount;
	public GameObject bullet;
	public Life life;


	bool jumpGesture1 = false;
	bool jumpGesture2 = false;
	int count = 0;

	private Renderer renderer;
	private bool GameClear = false;
	public Text clearText;

	//kinect
	private KinectSensor kinect;



	//Body
	public BodySourceManager bodyManager;


	//GestureBuilder
	private VisualGestureBuilderDatabase gestureDataBase;
	private VisualGestureBuilderFrameSource gestureFrameSource;
	private VisualGestureBuilderFrameReader gestureFrameReader;

	//Gesture
	private Gesture jump;
	private Gesture jumpProgress;

	//MainCamera
	public GameObject mainCamera;


	// Use this for initialization
	void Start ()
	{
		//Application.targetFrameRate = 1;

		KinectConect();
		isGround = false;
		renderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!GameClear)
		{
			Jump();
			Ground();

			if (!Input.GetKey(KeyCode.RightArrow))
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(4, GetComponent<Rigidbody2D>().velocity.y);
				if (isGround)
				{
					Anime.offset.y = 0.25f;
				}
				transform.localScale = new Vector3(1, 1, 1);

				CameraPosition();

			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(-4, GetComponent<Rigidbody2D>().velocity.y);
				if (isGround)
				{
					Anime.offset.y = 0.25f;
				}
				transform.localScale = new Vector3(-1, 1, 1);

				CameraPosition();
			}
			else
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
				if (isGround)
				{
					Anime.offset.y = 0.75f;
				}
			}
			if (GetComponent<Rigidbody2D>().velocity.y <= -10)
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -15);
			}

			if (Input.GetKeyDown("left ctrl"))
			{
				Instantiate(bullet, transform.position + new Vector3(0f, 0.05f, 0f), transform.rotation);
			}



			if (!gestureFrameSource.IsTrackingIdValid)
			{
				FindValidBody();
			}
		}else
		{
			clearText.enabled = true;
			Invoke("CallTitle", 5);
		}
	}


	void FindValidBody()
	{
		if (bodyManager != null) {
			Body[] bodies = bodyManager.GetData();
			if (bodies != null)
			{
				foreach (Body body in bodies)
				{
					if (body.IsTracked)
					{
						SetBody(body.TrackingId);
						break;
					}
				}
			}
		}
	}

	public void SetBody(ulong id)
	{
		if(id > 0)
		{
			gestureFrameSource.TrackingId = id;
			gestureFrameReader.IsPaused = false;
		}else
		{
			gestureFrameSource.TrackingId = 0;
			gestureFrameReader.IsPaused = true;
		}
	}

	void Jump()
	{
		if ((Input.GetKeyDown(KeyCode.UpArrow)||  jumpGesture1) && isGround == true)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 15);
		}
		else if((Input.GetKeyDown(KeyCode.UpArrow)|| jumpGesture1) && isGround == false && jumpCount >= 1)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 12);
			jumpCount--;
		}

		if (((Input.GetKeyUp(KeyCode.UpArrow))|| jumpGesture2) && GetComponent<Rigidbody2D>().velocity.y >= 2)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 4);
		}

		if (GetComponent<Rigidbody2D>().velocity.y >= 1)
		{
			Anime.offset.y = 0;
		}
		if (GetComponent<Rigidbody2D>().velocity.y <= 0)
		{
			Anime.offset.y = 0.5f;
		}

	}

	void Ground()
	{
		hit1 = Physics2D.Raycast(transform.position, -Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Ground"));
		hit2 = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0), -Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Ground"));
		hit3 = Physics2D.Raycast(transform.position - new Vector3(0.2f, 0), -Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Ground"));


		if (hit1.collider == null && hit2.collider == null && hit3.collider == null)
		{
			isGround = false;
			return;
		}
		else
		{
			isGround = true;
			jumpCount = 2;
		}
	}


   
	void KinectConect()
	{
		kinect = KinectSensor.GetDefault();
		if (kinect == null)
			{
			if (kinect != null)
			{
				if (!kinect.IsOpen)
				{
					Debug.Log("is OPEN");
					kinect.Open();
				}

			}
		}
			
			Initialize();
	}

	private  void Initialize()
	{
		//Gestureの初期設定
		var databasePath = Path.Combine(Application.streamingAssetsPath, "Jump.gbd");
		gestureDataBase =  VisualGestureBuilderDatabase.Create(databasePath);
	
		gestureFrameSource = VisualGestureBuilderFrameSource.Create(kinect, 0);


		//Gestreuをdatabaseから取り出す
		foreach (var gesture in gestureDataBase.AvailableGestures)
		{
			if(gesture.Name == "Jump")
			{
				jump = gesture;
				Debug.Log("OK1");
			}
			if (gesture.Name == "Seated")
			{
				jumpProgress = gesture;
				Debug.Log("OK2");
			}
			
			this.gestureFrameSource.AddGesture(gesture);
		}
		
		//GestureReader
		gestureFrameReader = gestureFrameSource.OpenReader();
		if(gestureFrameReader != null)
		{
			gestureFrameReader.IsPaused = true;
			gestureFrameReader.FrameArrived += gestureFrameReader_FrameArrived;
			Debug.Log("IS PAUSED");
		}

		bodyManager = this.gameObject.GetComponent<BodySourceManager>();
		
	}
	

	private void gestureFrameReader_FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
	{
		

		using (var gestureFrame = e.FrameReference.AcquireFrame())
		{
			// ジェスチャーの判定結果がある場合
			if (gestureFrame != null)
			{
				var discreteResults = gestureFrame.DiscreteGestureResults;
				//Debug.Log("FRAME ARRIVED");

				if ((discreteResults != null) && (discreteResults.ContainsKey(this.jump)))
				{
					var result = discreteResults[this.jump];
					if (result.Detected == true)
					{
						if (count == 0)
						{
							jumpGesture1 = true;
							jumpGesture2 = false;
							count++;
							Debug.Log("JUMP START");
						}
						else
						{
							jumpGesture1 = false;
						}

						//Debug.Log("working");
					}
					else
					{
						if (count != 0)
						{
							count = 0;
							jumpGesture2 = true;
							Debug.Log("JUMPEND");
						}
						else
						{
							jumpGesture2 = false;
						}
					}
				}
			}
		}
	}

	void CameraPosition()
	{
		if (transform.position.x > mainCamera.transform.position.x - 4)
		{
			Vector3 cameraPos = mainCamera.transform.position;

			cameraPos.x = transform.position.x + 4;
			mainCamera.transform.position = cameraPos;
		}

		Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

		Vector2 pos = transform.position;

		pos.x = Mathf.Clamp(pos.x, min.x + 0.5f, max.x);
		transform.position = pos;
	}
	

	IEnumerator Damage()
	{
		gameObject.layer = LayerMask.NameToLayer("PlayerDamege");

		int count = 10;
		while(count > 0)
		{
			renderer.material.color = new Color(1 ,1, 1, 0);
			yield return new WaitForSeconds(0.1f);

			renderer.material.color = new Color(1, 1, 1, 1);
			yield return new WaitForSeconds(0.1f);

			count--;
		}
		gameObject.layer = LayerMask.NameToLayer("Player");
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Goal")
		{
			GameClear = true;
		}
		else if (col.tag == "Enemy")
		{
			life.LifeDown();
			StartCoroutine("Damage");
		}
	}

	void CallTitle()
	{
		SceneManager.LoadScene("Title");
	}
}

