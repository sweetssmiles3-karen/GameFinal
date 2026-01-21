using UnityEngine;
using UnityEditor;
public class WayPointManagerWindow : EditorWindow
{
    [MenuItem("Window/WayPoint Editor Tools")]

    public static void ShowWindow()
    {
        GetWindow<WayPointManagerWindow>("WayPoint Editor Tools");
    }

    public Transform wayPointOrigin;

    private void OnGUI()
    {
       SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("wayPointOrigin"));

        if(wayPointOrigin == null)
        {
            EditorGUILayout.HelpBox("please assign a waypoint origin transform", MessageType.Warning);
            
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            createButtoms();
            EditorGUILayout.EndVertical();
        }
        obj.ApplyModifiedProperties();
    }
    void createButtoms() { 
    if(GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoints();
        }
    }
    void CreateWaypoints() {
        GameObject waypointObject = new GameObject("waypoint" + wayPointOrigin.childCount, typeof(WayPoint));
        waypointObject.transform.SetParent(wayPointOrigin, false);
    
        WayPoint waypoint = waypointObject.GetComponent<WayPoint>();
        if(wayPointOrigin.childCount > 1)
        {
            WayPoint previousWayPoint = wayPointOrigin.GetChild(wayPointOrigin.childCount - 2).GetComponent<WayPoint>();
            waypoint.previousWayPoint = previousWayPoint;
            previousWayPoint.nextWayPoint = waypoint;
            waypointObject.transform.position = previousWayPoint.transform.position + Vector3.forward * 5f;
            waypoint.transform.forward = waypoint.previousWayPoint.transform.forward;

            Selection.activeGameObject = waypoint.gameObject;//last created, last selected

        }
    }
}
