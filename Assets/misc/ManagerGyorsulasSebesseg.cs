using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGyorsulasSebesseg : ScriptableObject
{
	public readonly float	VELOCITY_ADD = 0.2f;		// meter/sec

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
		float	velocityAdd;

		if ( gyorsulasSzamol==true )
		{
			if ( now-lastTimeChangeVelocity>200 )
			{
				indexAddPow2 = 0;
				kesleltetes = false;
			}
			else
				indexAddPow2 += 1;
			lastTimeChangeVelocity = now;
		}

		if ( gyorsulasSzamol==false || kesleltetes==false )
		{
			velocityAdd = VELOCITY_ADD*(1L<<indexAddPow2);
			if ( velocityTarget==0.0f )
				velocityTarget = dirForward*VELOCITY_ADD;
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
	}

	public void setNullValue()
	{
		gyorsulas = 0.0f;
		velocityTarget = 0.0f;
		velocity = 0.0f;
	}

	public float updateVelocity( float timerInterpolation )
	{
		gyorsulas = (velocityTarget-velocity)*timerInterpolation*30f;
		if ( gyorsulas*gyorsulas<0.00001 )
			gyorsulas = 0.0f;

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

	public float getIndexAddPow2()
	{
		return (float)indexAddPow2/100.0f;
	}
}
