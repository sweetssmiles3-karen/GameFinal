using UnityEngine;

public class waypointgetter : MonoBehaviour
{

    [Header("AI Character")]
    AInavigating character;
    public WayPoint currentWayPoint;
    int direction;

    private void Awake()
    {
        character = GetComponent<AInavigating>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = Mathf.RoundToInt(Random.Range(0f, 1f));
        character.locateDestination(currentWayPoint.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        //this code will make nextWayPoint as the next destination
        if (character.destinationReached)
        {


            currentWayPoint = currentWayPoint.nextWayPoint;//make the next node as the next destination
            character.locateDestination(currentWayPoint.GetPosition());//pass the next node as the next destination
        }
    }
}