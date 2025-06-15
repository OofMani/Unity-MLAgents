using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class GoToBallAgent : Agent
{

    [SerializeField] private Transform targetBall;

    [SerializeField] public float moveSpeed = 1f; // Speed of the agent

    [SerializeField] private Material winMaterial; // Material to apply when the agent wins
    [SerializeField] private Material loseMaterial; // Material to apply when the agent loses
    [SerializeField] private MeshRenderer  agentSkin; 
    public Vector3 initialPosition;
    void Start()
    {
        initialPosition = transform.localPosition;

    }

    public override void OnEpisodeBegin()
    {
        // Reset the environment at the beginning of each episode
        // This could include resetting the localPosition of the agent and the ball

        transform.localPosition = new Vector3(Random.Range(-1.5f, 3.5f), -3.31f, Random.Range(0.5f, 5f)); // Reset agent's position
        transform.localRotation = Quaternion.identity; // Reset agent's rotation

        targetBall.localPosition = new Vector3(Random.Range(-1.5f, 3.5f), -3.31f, Random.Range(0.5f, 5f)); // Reset ball's position
        targetBall.localRotation = Quaternion.identity; // Reset ball's rotation
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations about the environment
        // This could include the localPosition of the agent, the localPosition of the ball, etc.

        sensor.AddObservation(transform.localPosition); // Agent's localPosition
        sensor.AddObservation(targetBall.localPosition); // Ball's localPosition

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Handle the actions received from the neural network
        // This could include moving the agent towards the ball based on the actions

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];


        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ball>(out Ball ball))
        {
            SetReward(1.0f); // Reward for reaching the ball
            EndEpisode(); // End the episode after reaching the ball

            // Optionally, change the agent's material to indicate success
            agentSkin.material = winMaterial; // Change to win material

        }

        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1.0f); // Penalty for hitting a wall
            EndEpisode(); // End the episode after hitting a wall

            agentSkin.material = loseMaterial; // Change to lose material
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Provide manual control for testing
        // This could include using keyboard input to move the agent
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal"); // Left/Right movement
        continuousActions[1] = Input.GetAxis("Vertical"); // Forward/Backward movement
    }
        
}
