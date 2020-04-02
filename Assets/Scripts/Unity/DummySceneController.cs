using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySceneController : MonoBehaviour
{
    void Start()
    {
        //Increment level and world here
        //End the loop if at the final level


        //Go back to mapviewer with the new settings
        SceneManager.LoadScene("MapViewer");
    }
}
