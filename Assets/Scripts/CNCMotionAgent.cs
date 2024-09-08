
using System.Threading;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// A CNC ML agent
/// </summary>
public class CNCMotionAgent : Agent
{
	[Tooltip("Whether this is training or gameplay mode")]
	public bool trainingMode;

	[Tooltip("Whether to show debug visuals")]
	public bool debugMode;

	[Tooltip("Maximum force that motors can apply to move each axis")]
	public float moveForce = 0.1f;

	// [Tooltip("XY acceleration (mm/sec^2)")]
	// public float xyAcceleration = 240;

	// [Tooltip("Z acceleration (mm/sec^2)")]
	// public float zAcceleration = 60;

	// [Tooltip("XY Max feed rate (mm/s)")]
	// public float xyMaxSpeed = 20;

	// [Tooltip("XY Max feed rate (mm/s)")]
	// public float zMaxSpeed = 20;

	// private int stepsPerMM = 1600;
	// private int stepsPerRev = 200;
	// private int microStepsPerFullStep = 32;
	// private int mmPerRev = 4;
	// private float mmPerStep = 0.005f;
	// private float motorMaxTorque = 0.25f; //Nm
	// private float motorMaxVoltage = 12; //V

	// The "agent" in this case has multiple rigid bodies associated with it
	private Transform xTransform;
	private Transform yTransform;
	private Transform zTransform;
	private Transform contactStickTransform;
	private Rigidbody xRigidBody;
	private Rigidbody yRigidBody;
	private Rigidbody zRigidBody;
	private Rigidbody contactStickRigidBody;
	private Rigidbody cubeRigidBody;
	private Transform Cube;
	private Vector3 endEffectorPosition;
	private CapsuleCollider capsule;
	private Vector3 disVector;
	private float xDistance;
	private float yDistance;
	private float zDistance;
	private float minXDistance;
	private float minYDistance;
	private float minZDistance;
	private float yMotionStageLengthX = 0.3f;
	private float yMotionStageLengthZ = 0.18f;
	private float cubeLengthX = 0.05f;
	private float cubeLengthY = 0.05f;
	private float cubeLengthZ = 0.05f;
	// Reward based on current distance and min distance between cube and endeffector

	// Negative reward over time
	private float timeBasedRewardDecrement = 0.001f;

	/// <summary>
	/// Number of times touched cube this episode
	/// </summary>
	public int NumberOfTimesTouched { get; private set; }
	private void Start()
	{
		base.Awake();
		contactStickTransform = this.transform.Find("Test Stick/Test_Stick");
		xTransform = this.transform.Find("XMotionStage/XMotion");
		yTransform = this.transform.Find("YMotionStage/YMotion");
		zTransform = this.transform.Find("ZMotionStage/ZMotion");

		xRigidBody = xTransform.GetComponent<Rigidbody>();
		yRigidBody = yTransform.GetComponent<Rigidbody>();
		zRigidBody = zTransform.GetComponent<Rigidbody>();
		contactStickRigidBody = contactStickTransform.GetComponent<Rigidbody>();

		capsule = contactStickTransform.GetComponent<CapsuleCollider>();
		endEffectorPosition = capsule.center + contactStickRigidBody.transform.position;

		Cube = this.transform.parent.Find("TestCube/Test_Cube");

		GameEvents.current.OnCubeContactWithGoal += CubeHitGoal;
		GameEvents.current.OnCubeContactWithEndEffector += CubeHitEndEffector;

	}

	public override void OnEpisodeBegin()
	{
		base.OnEpisodeBegin();
		Debug.Log("Starting episode");
		NumberOfTimesTouched = 0;
		UpdateDistance();

		minXDistance = xDistance;
	}

	public void UpdateDistance()
	{
		endEffectorPosition = capsule.center + contactStickRigidBody.transform.position;
		disVector = endEffectorPosition - Cube.transform.position;
		xDistance = Mathf.Abs(disVector.x);
	}

	public void CheckDistanceReward()
	{
		// Moving towards
		if (xDistance <= minXDistance)
		{
			AddReward(0.01f);
			minXDistance = xDistance;
		}
		// moving away
		else
		{
			if (this.transform.parent.name != "TrainingArea") 
				{ Debug.Log($"Moving away in stage {this.transform.parent.name}!"); }
			AddReward(-0.005f);
		}
	}
	public void FixedUpdate()
	{
		UpdateDistance();

		if (trainingMode) CheckDistanceReward();
	}

	void OnDrawGizmos(){
		if (!Application.isPlaying) return;
		if (debugMode){
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(endEffectorPosition, Cube.transform.position);
		}
	}

	private void CubeHitGoal()
	{
		if (trainingMode)
		{
			AddReward(10f);
			Debug.Log($"Cube hit goal in stage {this.transform.parent.name}! Ending episode, final reward is {GetCumulativeReward()}");
		}
		EndEpisode();

	}
	private void CubeHitEndEffector()
	{
		NumberOfTimesTouched += 1;
		Debug.Log($"Cube hit end effector in stage {this.transform.parent.name} " +
			$"{NumberOfTimesTouched} times, " +
			$"current reward is {GetCumulativeReward()} " +
			$"in episode {this.CompletedEpisodes}");
		if (trainingMode)
		{
			AddReward(0.1f);
		}
	}

	/// <summary>
	/// Called when action is received from player or NN
	/// 
	/// We have a vector of actions 
	/// Index 0: move X stage (+1 right, -1 left)
	/// Index 1: move y stage (+1 towards back, -1 towards front)
	/// Index 2: move z stage (+1 up, -1 down)
	/// </summary>
	/// <param name="actions"></param>
	public override void OnActionReceived(ActionBuffers actions)
	{
		xRigidBody.AddForce(Vector3.right * Mathf.Clamp(actions.ContinuousActions[0],-1f,1f) * moveForce);
		//yRigidBody.AddForce(Vector3.forward * Mathf.Clamp(actions.ContinuousActions[1],-1f,1f) * moveForce);
		//zRigidBody.AddForce(Vector3.up * Mathf.Clamp(actions.ContinuousActions[2],-1f,1f) * moveForce);
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

		//if (Input.GetKey(KeyCode.W)) yMotion = -1;
		//else if (Input.GetKey(KeyCode.S)) yMotion = 1;
		//else yMotion = 0;

		if (Input.GetKey(KeyCode.A)) xMotion = 1;
		else if (Input.GetKey(KeyCode.D)) xMotion = -1;
		else xMotion = 0;

		//if (Input.GetKey(KeyCode.E)) zMotion = 1;
		//else if (Input.GetKey(KeyCode.F)) zMotion = -1;
		//else zMotion = 0;

		var continActionsOut = actionsOut.ContinuousActions;
		continActionsOut[0] = xMotion;
		//continActionsOut[1] = yMotion;
		//continActionsOut[2] = zMotion;
	}
}