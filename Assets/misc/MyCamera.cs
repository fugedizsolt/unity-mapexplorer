using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
	enum ACC_DIR_TYPE
	{
		FORWARD,
		RIGHT,
		UP,
		ROTATE
	}

	private readonly float	SCALE_ROTATE	= 10.0f;
	private readonly float	MOUSE_HOLTTER	= 10.0f;
	private readonly float	MOUSE_SCALE		= 60.0f/20.0f;	// 100 pixelenkent 60 fok/sec
	private readonly float	VELOCITY_DEGREE_MULT	= (float)(1.0f/180.0f*Mathf.PI);	// fok/sec, radianban 2PI/60 /sec

	private bool bUseMouseInput = false;

	private Vector3	location;
	private Quaternion	quat = new Quaternion();
	private Quaternion	tmpquat = new Quaternion();

	private Vector3	mouseCenter;

	private float		velocityYaw = 0.0f;		// fok/sec
	private float		velocityPitch = 0.0f;	// fok/sec
	private float		velocityRoll = 0.0f;	// fok/sec

	private Vector3	vecLookForward = new Vector3( 0,0,0 );
	private Vector3	vecLookRight = new Vector3( 0,0,0 );
	private Vector3	vecLookUp = new Vector3( 0,0,0 );

	private Vector3	vecMoveForward = new Vector3( 0,0,0 );
	private Vector3	vecMoveRight = new Vector3( 0,0,0 );
	private Vector3	vecMoveUp = new Vector3( 0,0,0 );

	private ManagerGyorsulasSebesseg	gyorsulasForward;
	private ManagerGyorsulasSebesseg	gyorsulasRight = null;
	private ManagerGyorsulasSebesseg	gyorsulasUp = null;
	private ManagerGyorsulasSebesseg	gyorsulasRotate = null;

	private ACC_DIR_TYPE prevDir = ACC_DIR_TYPE.FORWARD;
	private int counter = 0;

	private UnityEngine.UI.Text objPosInfo;

	void Start()
	{
		Debug.Log( "Hello Fuge at Start()!" );
		//location = transform.position;

		//vecMoveForward = transform.forward;
		//gyorsulasForward = new ManagerGyorsulasSebesseg( true );
		gyorsulasForward = ScriptableObject.CreateInstance<ManagerGyorsulasSebesseg>();
		gyorsulasRight = ScriptableObject.CreateInstance<ManagerGyorsulasSebesseg>();
		gyorsulasUp = ScriptableObject.CreateInstance<ManagerGyorsulasSebesseg>();
		gyorsulasRotate = ScriptableObject.CreateInstance<ManagerGyorsulasSebesseg>();
	}

	// Update is called once per frame
	void Update()
	{
		if ( objPosInfo==null )
			objPosInfo = GameObject.FindGameObjectWithTag( "posInfo" ).GetComponent<UnityEngine.UI.Text>();

		objPosInfo.text = string.Format( 
			"counter:{0}\n" + 
			"position:{1,0:F2},{2,0:F2},{3,0:F2}\n" +
			"forward:{4,0:F2}\n" +
			"right:{5,0:F2}\n" +
			"up:{6,0:F2}\n",
			counter,
			transform.position.x,
			transform.position.y,
			transform.position.z,
			0,0,0 );

		counter++;
		if ( counter<10 )
			Debug.Log( "Time.deltaTime:" + Time.deltaTime + " realtimeSinceStartup:" + Time.realtimeSinceStartup );

		if ( counter==1 )
		{
			Cursor.lockState = CursorLockMode.Locked;
			Debug.Log( "Locked Input.mousePosition.x=" + Input.mousePosition.x + " y=" + Input.mousePosition.y );
		}
		//if ( counter%60==0 ) Debug.Log( "Input.mousePosition.x=" + Input.mousePosition.x + " y=" + Input.mousePosition.y );

		float deltaTime = Time.deltaTime;
		//float velocityForward = gyorsulasForward.updateVelocity( deltaTime );
		//float velocityRight = gyorsulasRight.updateVelocity( deltaTime );
		//float velocityUp = gyorsulasUp.updateVelocity( deltaTime );
		//float velocityRoll = gyorsulasRotate.updateVelocity( deltaTime ) * SCALE_ROTATE;

		if ( Input.GetKeyDown( KeyCode.S )==true )
		{
			bUseMouseInput = !bUseMouseInput;
			if ( Cursor.lockState==CursorLockMode.Locked )
			{
				Cursor.lockState = CursorLockMode.None;
				mouseCenter = Input.mousePosition;
				Debug.Log( "None Input.mousePosition.x=" + Input.mousePosition.x + " y=" + Input.mousePosition.y );
			}
		}
		else
		{
			if ( bUseMouseInput==true )
			{
				setYawPitchFromMousePos();

				//velocityRoll = 100f;
				//velocityYaw = 20f;

				float segedFloat = VELOCITY_DEGREE_MULT * deltaTime;
				tmpquat = Quaternion.Euler( 
					velocityPitch*segedFloat,
					velocityYaw*segedFloat,
					velocityRoll*segedFloat );

				transform.rotation *= tmpquat;

				//Matrix4x4 m4x4rot = Matrix4x4.Rotate( transform.rotation );
				//Matrix4x4 m4x4rottmp = m4x4rot * Matrix4x4.Rotate( tmpquat );
				//transform.rotation = m4x4rottmp.rotation;

				Vector3 vecForward = transform.rotation * Vector3.forward;
				//Matrix4x4.Rotate( quat );

				transform.position += vecForward * (deltaTime/10);
			}
		}

	}

	public void setYawPitchFromMousePos()
	{
		float   dax,day;

		Vector3 mouseDiff = Input.mousePosition -  mouseCenter;
		float mx = mouseDiff.x;
		float my = mouseDiff.y;

		if ( System.Math.Abs( mx )<MOUSE_HOLTTER )
			dax = 0.0f;
		else if ( mx>0 )
			dax = (mx-MOUSE_HOLTTER)*MOUSE_SCALE;
		else
			dax = (mx+MOUSE_HOLTTER)*MOUSE_SCALE;
		if ( System.Math.Abs( my )<MOUSE_HOLTTER )
			day = 0.0f;
		else if ( my>0 )
			day = (my-MOUSE_HOLTTER)*MOUSE_SCALE;
		else
			day = (my+MOUSE_HOLTTER)*MOUSE_SCALE;

		velocityYaw = dax;
		velocityPitch = -day;
	}
}
