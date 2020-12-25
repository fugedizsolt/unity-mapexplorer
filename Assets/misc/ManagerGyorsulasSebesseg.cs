using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGyorsulasSebesseg : ScriptableObject
{
	public readonly float	VELOCITY_ADD = 0.002f;		// meter/sec

	private float		gyorsulas = 0.0f;
	private float		velocityTarget = 0.0f;
	private float		velocity = 0.0f;
	private long		lastTimeChangeVelocity = 0;
	private int			indexAddPow2 = 0;
	private bool		kesleltetes = false;
	private bool		gyorsulasSzamol = true;


	public ManagerGyorsulasSebesseg()
	{
	}

	public void changeVelocityTarget( bool forward,long now )
	{
		int		dirForward = ( forward==true ) ? 1 : -1;

		Debug.Log( string.Format( "changeVelocityTarget called {0} now{1}",forward,now ) );
		if ( gyorsulasSzamol==true )
		{
			if ( now-lastTimeChangeVelocity>200 )
			{
				indexAddPow2 = 0;
				kesleltetes = false;
			}
			else
			{
				if ( kesleltetes==false )
					indexAddPow2 += 1;
			}
			lastTimeChangeVelocity = now;
		}

		if ( gyorsulasSzamol==false || kesleltetes==false )
		{
			float velocityAdd = VELOCITY_ADD*(1L<<indexAddPow2);
			if ( velocityTarget==0.0f )
				velocityTarget = dirForward*velocityAdd;
			else if ( dirForward*(velocityTarget-velocity)>0 )
				velocityTarget += dirForward*velocityAdd;
			else
			{
				if ( velocityTarget!=velocity )
					velocityTarget = velocity;
				else
					velocityTarget += dirForward*velocityAdd;
			}
		}
		Debug.Log( string.Format( "changeVelocityTarget result indexAddPow2:{0} velocityTarget:{1}",indexAddPow2,velocityTarget ) );
	}

	public void setNullValue()
	{
		gyorsulas = 0.0f;
		velocityTarget = 0.0f;
		velocity = 0.0f;
	}

	public float updateVelocity( float timerInterpolation )
	{
		gyorsulas = (velocityTarget-velocity);
		if ( Mathf.Abs(gyorsulas)<0.00001 )
		{
			velocityTarget = velocity;
			gyorsulas = 0.0f;
		}

		float velocityNew = velocity + gyorsulas*timerInterpolation;
		if ( velocity!=0 && velocity*velocityNew<=0 )
		{
			setNullValue();
			velocityNew = 0;
			kesleltetes = true;
		}
		velocity = velocityNew;
		return velocityNew;
	}

	public float getGyorsulas()
	{
		return gyorsulas;
	}
	public float getVelocity()
	{
		return velocity;
	}
	public float getVelocityTarget()
	{
		return velocityTarget;
	}
	public float getIndexAddPow2()
	{
		return (float)indexAddPow2/100.0f;
	}
}
