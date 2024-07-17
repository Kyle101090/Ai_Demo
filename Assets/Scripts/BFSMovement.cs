using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float avoidanceRadius = 0.5f; 
    public float avoidanceStrength = 1f; 
    public float initialPositionOffset = 0.1f; 

    private List<GameObject> path = new List<GameObject>();
    private int currentTargetIndex = 0;

    void Update()
    {
        if (path.Count > 0)
        {
            MoveAlongPath();
        }
    }


    public void SetPath(List<GameObject> newPath)
    {
        path = newPath;
        currentTargetIndex = 0;
        if (path.Count > 0)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-initialPositionOffset, initialPositionOffset),
                Random.Range(-initialPositionOffset, initialPositionOffset),
                0);
            transform.position = path[0].transform.position + randomOffset; 
            Debug.Log("Agent started at: " + transform.position + " targeting: " + path[0].transform.position);
        }
    }

    private void MoveAlongPath()
    {
        if (currentTargetIndex < path.Count)
        {
            GameObject targetTile = path[currentTargetIndex];
            Vector3 targetPosition = targetTile.transform.position;

            Vector3 movementDirection = (targetPosition - transform.position).normalized;
            Vector3 avoidanceDirection = AvoidOtherAgents();
            Vector3 finalDirection = (movementDirection + avoidanceDirection).normalized;

            transform.position += finalDirection * moveSpeed * Time.deltaTime;
            Debug.Log("Agent moving towards: " + targetPosition + " current position: " + transform.position);

            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentTargetIndex++;
                Debug.Log("Agent reached target tile: " + targetTile.name + " moving to next index: " + currentTargetIndex);
            }
        }
    }

    private Vector3 AvoidOtherAgents()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
        Vector3 avoidanceVector = Vector3.zero;

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject) // Ignore self
            {
                Vector3 direction = transform.position - collider.transform.position;
                avoidanceVector += direction.normalized / direction.sqrMagnitude;
            }
        }

        return avoidanceVector * avoidanceStrength;
    }
}
