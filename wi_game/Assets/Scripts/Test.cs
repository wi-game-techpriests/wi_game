using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public Image backgroundImage; // Assign this in the Inspector with your background image


    void Start()
    {
        Debug.Log("Game Started!"); // Log a message to the console for debugging   
        // Optionally, you can set an initial color for the background image here
        backgroundImage.color = Color.blue; // Set the initial color to blue
    }

    public void StartGame()
    {
        Debug.Log("Start Game button clicked!"); // Log a message to the console for debugging
        backgroundImage.color = Color.green; // Change the background color to green when the start button is clicked
    }

    public void EndGame()
    {
        Debug.Log("Exit Game button clicked!"); // Log a message to the console for debugging
        backgroundImage.color = Color.red; // Change the background color to red when the exit button is clicked
    }

}
