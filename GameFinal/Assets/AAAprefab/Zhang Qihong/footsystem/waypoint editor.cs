using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
public class WaypointEditor 
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(WayPoint waypoint, GizmoType gizmoType)
    {
     if((gizmoType & GizmoType.Selected) != 0)
     {
         Gizmos.color = Color.blue;
     }
     else
     {
         Gizmos.color = Color.blue * 0.5f;
        }
    
        Gizmos.DrawSphere(waypoint.transform.position, 0.1f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(waypoint.transform.position+(waypoint.transform.right*waypoint.wayPointWidth/2f), waypoint.transform.position - (waypoint.transform.right * waypoint.wayPointWidth)/2f);
        if(waypoint.previousWayPoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.wayPointWidth / 2f;
            Vector3 offsetTo = waypoint.previousWayPoint.transform.right * waypoint.previousWayPoint.wayPointWidth / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.previousWayPoint.transform.position + offsetTo);
        }
        if (waypoint.nextWayPoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * waypoint.wayPointWidth / 2f;
            Vector3 offsetTo = waypoint.previousWayPoint.transform.right * -waypoint.previousWayPoint.wayPointWidth / 2f;

            Gizmos.DrawLine(waypoint.transform.position - offset, waypoint.previousWayPoint.transform.position + offsetTo);
        }
    }

}
