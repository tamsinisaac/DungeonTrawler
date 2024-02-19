using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class StairTravel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (col("Stairs"))
        {
            SceneManager.LoadScene("Level2");
        }
    }
}
