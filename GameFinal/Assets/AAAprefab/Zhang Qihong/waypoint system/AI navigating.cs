using JetBrains.Annotations;
using UnityEngine;

public class AInavigating : MonoBehaviour
{
    [Header("character info")]
    public float movingSpeed = 3f;
    public float turningSpeed = 5f;
    public float stoppSpeed = 0.1f;

    [Header("Destination Var")]
    public Vector3 destination;
    public bool destinationReached = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
    }

    public void Walk() { 
        if(transform.position != destination) 
            {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0f;
            float destinationDistance = destinationDirection.magnitude;

            if(destinationDistance >= stoppSpeed)
            {
                //turning
                destinationReached = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
                //moving AI
                transform.Translate(Vector3.forward* movingSpeed * Time.deltaTime);

            }
            else
            {
                destinationReached = true;
                transform.position = destination;
                
            }
        }
        
    }
    public void locateDestination(Vector3 destination)
    {
                this.destination = destination;
        destinationReached = false;

    }
}
