using UnityEngine;

public class maps : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject map;
    void Start()
    {
        map.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            tuggleMapActive();
        }
    }

    void tuggleMapActive()
    {
        if (map.activeInHierarchy)
        {
            map.SetActive(false);
        }
        else
        {
            map.SetActive(true);
        }
    }
}
