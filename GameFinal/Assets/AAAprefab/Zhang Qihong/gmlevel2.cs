using UnityEngine;

public class gmlevel2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject barrier;
    public int killcount;
    public int maxkillcount;
    public GameObject hint,hint2;
    public static gmlevel2 Instance { get; private set; }
    void Start()
    {
        killcount = 0;
        hint.SetActive(true);
        hint2.SetActive(false);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void togglehint()
    {
        hint.SetActive(false);
        hint2.SetActive(true);
    }
    public void togglehint2()
    {
        hint2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (killcount >= maxkillcount)
        {
            barrier.SetActive(false);
            togglehint2();
        }
    }
}
