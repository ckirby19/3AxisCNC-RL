using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// A CNC ML agent
/// </summary>
public class CNCMotionAgent : Agent
{
	[Tooltip("Agents camera")]
	public Camera agentCamera;

	[Tooltip("Whether this is training or gameplay mode")]
	public bool trainingMode;

	[Tooltip("Maximum force that motors can apply to move each axis")]
	public float moveForce = 2f;

	[Tooltip("XY acceleration (mm/sec^2)")]
	public float xyAcceleration = 240;

	[Tooltip("Z acceleration (mm/sec^2)")]
	public float zAcceleration = 60;

	[Tooltip("XY Max feed rate (mm/s)")]
	public float xyMaxSpeed = 20;

	[Tooltip("XY Max feed rate (mm/s)")]
	public float zMaxSpeed = 20;

	private int stepsPerMM = 1600;
	private int stepsPerRev = 200;
	private int microStepsPerFullStep = 32;
	private int mmPerRev = 4; 
	private float mmPerStep = 0.005f;
	private float motorMaxTorque = 0.25f; //Nm
	private float motorMaxVoltage = 12; //V

	// The "agent" in this case has multiple rigid bodies associated with it
	private GameObject xGameObject;
	private GameObject yGameObject;
	private GameObject zGameObject;
	private GameObject contactStickGameObject;
	private Rigidbody xRigidBody;
	private Rigidbody yRigidBody;
	private Rigidbody zRigidBody;
	private Rigidbody contactStickRigidBody;

	private bool frozen = false;

	private System.Random random = new System.Random();

	/// <summary>
	/// Amount of times agent pushed block off of stage
	/// </summary>
	public int TimesBlockedPushedOff { get; private set; }

	public override void Initialize()
	{
		contactStickGameObject = GameObject.Find("Test_Stick");
		xGameObject = GameObject.Find("XMotion");
		yGameObject = GameObject.Find("YMotion");
		zGameObject = GameObject.Find("ZMotion");

		xRigidBody = xGameObject.GetComponent<Rigidbody>();
		yRigidBody = yGameObject.GetComponent<Rigidbody>();
		zRigidBody = zGameObject.GetComponent<Rigidbody>();
		contactStickRigidBody = contactStickGameObject.GetComponent<Rigidbody>();
	}

	public override void OnEpisodeBegin()
	{
		if (trainingMode)
		{

		}
	}

	/// <summary>
	/// When behaviour set to heuristic only, this func called.
	/// Return values fed into <see cref="OnActionReceived(ActionBuffers)"/>
	/// instead of NN
	/// </summary>
	/// <param name="actionsOut"></param>
	public override void Heuristic(in ActionBuffers actionsOut)
	{
		// Use the keyboard to move each platform
		Vector3 xMotion = Vector3.zero;
		Vector3 yMotion = Vector3.zero;
		Vector3 zMotion = Vector3.zero;

		if (Input.GetKey(KeyCode.W));
		else if (Input.GetKey(KeyCode.S));

		if (Input.GetKey(KeyCode.A));
		else if (Input.GetKey(KeyCode.F));

		if (Input.GetKey(KeyCode.E));
		else if (Input.GetKey(KeyCode.C));

		if (Input.GetKey(KeyCode.UpArrow));
		else if (Input.GetKey(KeyCode.DownArrow));

		if (Input.GetKey(KeyCode.LeftArrow));
		else if (Input.GetKey(KeyCode.RightArrow));

		//Vector3 combined = (forward + left + up).normalized;

		//var continActionsOut = actionsOut.ContinuousActions;
		//continActionsOut[0] = combined.x;
		//continActionsOut[1] = combined.y;
		//continActionsOut[2] = combined.z;
		//continActionsOut[3] = pitch;
		//continActionsOut[4] = yaw;
	}