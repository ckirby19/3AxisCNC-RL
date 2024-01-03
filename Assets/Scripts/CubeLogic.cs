using System;
using UnityEngine;

public class CubeLogic : MonoBehaviour
{
    [HideInInspector]
    public bool isInContact;
    [HideInInspector]
    public Collider cubeCollider;
    public bool TriggerResetCube;

    private float yMotionStagePos;
    private float yMotionStageLengthX = 0.3f;
    private float yMotionStageLengthY = 0.18f;
    private float cubeLengthX = 0.025f;
    private float cubeLengthY = 0.025f;
    private float cubeLengthZ = 0.025f;
    private GameObject yMotionStage;
    private Rigidbody cubeRB;

	private void Start()
	{
        yMotionStage = GameObject.Find("YMotion");
        cubeRB = GetComponent<Rigidbody>();
        ResetCube();
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Goal")
        {
            GameEvents.current.TriggerCubeContactWithGoal();
            ResetCube();
        }
	}

	private void Update()
	{
		if (TriggerResetCube)
        {
            TriggerResetCube = false;
            ResetCube();
        }
	}

	// Need to fix cube reset procedure in order to use in training
	private void ResetCube()
    {
        yMotionStagePos = yMotionStage.transform.position.z;
		var randomPositionAcrossStage = new Vector3(
            UnityEngine.Random.Range(-yMotionStageLengthX/2 + cubeLengthX, yMotionStageLengthX/2 - cubeLengthX),
			UnityEngine.Random.Range(-yMotionStageLengthY/2 + cubeLengthY, yMotionStageLengthY/2 - cubeLengthY) - yMotionStagePos,
			cubeLengthZ / 2 + 0.0001f
		);

        cubeRB.velocity = new Vector3(0f, 0f, 0f);
        cubeRB.angularVelocity = new Vector3(0f, 0f, 0f);
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.transform.localPosition = randomPositionAcrossStage; 
    }
}
