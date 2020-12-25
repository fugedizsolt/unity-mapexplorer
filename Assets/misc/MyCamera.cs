using System;
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

	private readonly float	SCALE_ROTATE	= 5000.0f;
	private readonly float	MOUSE_HOLTTER	= 10.0f;
	private readonly float	MOUSE_SCALE		= 60.0f/20.0f;	// 100 pixelenkent 60 fok/sec
	private readonly float	VELOCITY_DEGREE_MULT	= (float)(20.0f/180.0f);	// fok/sec (nem radian) , info:Mathf.PI

	private bool bUseMouseInput = false;

	private Vector3	location;
	private Vector3	mouseCenter;

	private float		velocityYaw = 0.0f;		// fok/sec
	private float		velocityPitch = 0.0f;	// fok/sec

	private ManagerGyorsulasSebesseg	gyorsulasForward;
	private ManagerGyorsulasSebesseg	gyorsulasRight = null;
	private ManagerGyorsulasSebesseg	gyorsulasUp = null;
	private ManagerGyorsulasSebesseg	gyorsulasRotate = null;

	private ACC_DIR_TYPE prevDir = ACC_DIR_TYPE.FORWARD;
	private int counter = 0;

	private UnityEngine.UI.Text objPosInfo;
	private UnityEngine.LineRenderer objFpsLine;

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

		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update()
	{
		updatePosInfo();

		counter++;
		float deltaTime = Time.deltaTime;
		//if ( counter<10 )
		//Debug.Log( String.Format( "counter:{0} Time.deltaTime:{1}  realtimeSinceStartup:{2}",counter,Time.deltaTime,Time.realtimeSinceStartup ) );

		handleKeyboardInput();
		float velocityForward = gyorsulasForward.updateVelocity( deltaTime );
		float velocityRight = gyorsulasRight.updateVelocity( deltaTime );
		float velocityUp = gyorsulasUp.updateVelocity( deltaTime );
		float velocityRoll = gyorsulasRotate.updateVelocity( deltaTime ) * SCALE_ROTATE;

		if ( bUseMouseInput==true )
		{
			setYawPitchFromMousePos();
		}
		else
		{
			velocityPitch = 0;
			velocityYaw = 0;
		}

		float segedFloat = VELOCITY_DEGREE_MULT * deltaTime;
		transform.rotation *= Quaternion.Euler( velocityPitch*segedFloat,velocityYaw*segedFloat,velocityRoll*segedFloat );

		Vector3 vecForward = transform.rotation * Vector3.forward;
		Vector3 vecUp = transform.rotation * Vector3.up;
		Vector3 vecRight = transform.rotation * Vector3.right;

		//transform.position += vecForward * (deltaTime/10);
		transform.position += vecForward * velocityForward;
		transform.position += vecUp * velocityUp;
		transform.position -= vecRight * velocityRight;
	}

	private void handleKeyboardInput()
	{
		long ldeltaTime = (long)(Time.realtimeSinceStartup*1000f);

		if ( Input.GetKeyDown( KeyCode.Escape )==true )
			Application.Quit();
		else if ( Input.GetKeyDown( KeyCode.F1 )==true )
		{
			bUseMouseInput = !bUseMouseInput;
			if ( Cursor.lockState==CursorLockMode.Locked )
			{
				Cursor.lockState = CursorLockMode.None;
				mouseCenter = Input.mousePosition;
				Debug.Log( "None Input.mousePosition.x=" + Input.mousePosition.x + " y=" + Input.mousePosition.y );
			}
		}
		else if ( Input.GetKeyDown( KeyCode.W )==true )
		{
			gyorsulasForward.changeVelocityTarget( true,ldeltaTime );
			prevDir = ACC_DIR_TYPE.FORWARD;
		}
		else if ( Input.GetKeyDown( KeyCode.S )==true )
		{
			gyorsulasForward.changeVelocityTarget( false,ldeltaTime );
			prevDir = ACC_DIR_TYPE.FORWARD;
		}
		else if ( Input.GetKeyDown( KeyCode.A )==true )
		{
			gyorsulasRight.changeVelocityTarget( true,ldeltaTime );
			prevDir = ACC_DIR_TYPE.RIGHT;
		}
		else if ( Input.GetKeyDown( KeyCode.D )==true )
		{
			gyorsulasRight.changeVelocityTarget( false,ldeltaTime );
			prevDir = ACC_DIR_TYPE.RIGHT;
		}
		else if ( Input.GetKeyDown( KeyCode.E )==true )
		{
			gyorsulasRotate.changeVelocityTarget( true,ldeltaTime );
			prevDir = ACC_DIR_TYPE.ROTATE;
		}
		else if ( Input.GetKeyDown( KeyCode.Q )==true )
		{
			gyorsulasRotate.changeVelocityTarget( false,ldeltaTime );
			prevDir = ACC_DIR_TYPE.ROTATE;
		}
		else if ( Input.GetKeyDown( KeyCode.R )==true )
		{
			gyorsulasUp.changeVelocityTarget( true,ldeltaTime );
			prevDir = ACC_DIR_TYPE.UP;
		}
		else if ( Input.GetKeyDown( KeyCode.F )==true )
		{
			gyorsulasUp.changeVelocityTarget( false,ldeltaTime );
			prevDir = ACC_DIR_TYPE.UP;
		}
		else if ( Input.GetKeyDown( KeyCode.X )==true )
		{
			if ( prevDir==ACC_DIR_TYPE.FORWARD )
				gyorsulasForward.setNullValue();
			else if ( prevDir==ACC_DIR_TYPE.RIGHT )
				gyorsulasRight.setNullValue();
			else if ( prevDir==ACC_DIR_TYPE.UP )
				gyorsulasUp.setNullValue();
			else if ( prevDir==ACC_DIR_TYPE.ROTATE )
				gyorsulasRotate.setNullValue();
		}
	}

	private void updatePosInfo()
	{
		if ( objPosInfo==null )
		{
			objPosInfo = GameObject.FindGameObjectWithTag( "posInfo" ).GetComponent<UnityEngine.UI.Text>();
			objFpsLine = GameObject.FindGameObjectWithTag( "fpsLine" ).GetComponent<UnityEngine.LineRenderer>();

			Vector3[] initPositions = new Vector3[objFpsLine.positionCount];
			for ( int ic=0; ic<initPositions.Length; ic+=2 )
			{
				float fpos = 5*ic/2;
				initPositions[ic].Set( fpos,0f,0f );
				initPositions[ic+1].Set( fpos,0f,0f );
			}
			objFpsLine.SetPositions( initPositions );
		}

		objPosInfo.text = string.Format( 
			"counter:{0}\n" + 
			"position:{1,0:F2},{2,0:F2},{3,0:F2}\n" +
			"forward:{4,0:F6} acc:{5,0:F6} tgt:{12,0:F6}\n" +
			"right:{6,0:F6} acc:{7,0:F6}\n" +
			"up:{8,0:F6} acc:{9,0:F6}\n" +
			"roll:{10,0:F6} acc:{11,0:F6}\n",
			counter,
			transform.position.x,
			transform.position.y,
			transform.position.z,
			gyorsulasForward.getVelocity(),gyorsulasForward.getGyorsulas(),
			gyorsulasRight.getVelocity(),gyorsulasRight.getGyorsulas(),
			gyorsulasUp.getVelocity(),gyorsulasUp.getGyorsulas(),
			gyorsulasRotate.getVelocity(),gyorsulasRotate.getGyorsulas(),
			gyorsulasForward.getVelocityTarget() );

		Vector3[] positions = new Vector3[objFpsLine.positionCount];
		objFpsLine.GetPositions( positions );
		for ( int ic=0; ic<objFpsLine.positionCount-2; ic+=2 )
		{
			positions[ic+1].y = positions[ic+3].y;
		}
		positions[objFpsLine.positionCount-1].y = (int)(Time.deltaTime*10000f);
		objFpsLine.SetPositions( positions );
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
