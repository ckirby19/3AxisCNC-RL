using UnityEngine;

public class CubeLogic : MonoBehaviour
{
    [HideInInspector]
    public bool isInContact;
    [HideInInspector]
    public Collider cubeCollider;
	[Tooltip("Reset cube position without restarting episode")]
	public bool TriggerResetCube;

    private float yMotionStageLengthX = 0.3f; //300mm
    private float yMotionStageLengthY = 0.18f;
    private float cubeLengthX = 0.04f; //50mm
    private float cubeLengthY = 0.04f;
    private float cubeLengthZ = 0.04f;
    private Transform yMotionStage;
    private Rigidbody cubeRB;

	private void Start()
	{
        yMotionStage = this.transform.parent.parent.Find("MotionAgent/YMotionStage/YMotion");
        cubeRB = GetComponent<Rigidbody>();
        ResetCube();
	}

	private void Update()
	{
		if (TriggerResetCube)
		{
			TriggerResetCube = false;
			ResetCube();
		}
    
    }
private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Goal")
        {
            GameEvents.current.TriggerCubeContactWithGoal();
            ResetCube();
        }
        else if (collision.gameObject.tag == "EndEffector")
        {
            GameEvents.current.TriggerCubeContactWithEndEffector();
            ResetCube();
        }
	}

	private void ResetCube()
    {
		var randomPositionAcrossStage = new Vector3(
            UnityEngine.Random.Range((-yMotionStageLengthX + cubeLengthX) / 2f, (yMotionStageLengthX - cubeLengthX) / 2f),
			0.02f,
            0
		);

        cubeRB.velocity = new Vector3(0f, 0f, 0f);
        cubeRB.angularVelocity = new Vector3(0f, 0f, 0f);
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		this.transform.localPosition = randomPositionAcrossStage; 
    }
}
