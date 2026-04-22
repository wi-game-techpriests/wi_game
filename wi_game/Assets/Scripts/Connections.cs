using UnityEngine;

public class Connections : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Connections game started.");
    }

    public void Select(int id)
    {
        Debug.Log("Selected tile with id " + id + " at time " + Time.time);
    }
}
