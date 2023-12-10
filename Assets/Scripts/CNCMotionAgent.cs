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
	public float moveForce = 0.2f;

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
	/// Called when action is received from plauyer or NN
	/// 
	/// We have a vector of actions 
	/// Index 0: move X stage (+1 right, -1 left)
	/// Index 1: move y stage (+1 towards back, -1 towards front)
	/// Index 2: move z stage (+1 up, -1 down)
	/// </summary>
	/// <param name="actions"></param>
	public override void OnActionReceived(ActionBuffers actions)
	{
		if (frozen) return; // Used only after training

		xRigidBody.AddForce(Vector3.right * actions.ContinuousActions[0] * moveForce);
		yRigidBody.AddForce(Vector3.forward * actions.ContinuousActions[1] * moveForce);
		zRigidBody.AddForce(Vector3.up * actions.ContinuousActions[2] * moveForce);

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
		int xMotion = 0;
		int yMotion = 0;
		int zMotion = 0;

		if (Input.GetKey(KeyCode.W)) yMotion = -1;
		else if (Input.GetKey(KeyCode.S)) yMotion = 1;
		else yMotion = 0;

		if (Input.GetKey(KeyCode.A)) xMotion = 1;
		else if (Input.GetKey(KeyCode.D)) xMotion = -1;
		else xMotion = 0;

		if (Input.GetKey(KeyCode.E)) zMotion = 1;
		else if (Input.GetKey(KeyCode.F)) zMotion = -1;
		else zMotion = 0;

		var continActionsOut = actionsOut.ContinuousActions;
		continActionsOut[0] = xMotion;
		continActionsOut[1] = yMotion;
		continActionsOut[2] = zMotion;
	}
}