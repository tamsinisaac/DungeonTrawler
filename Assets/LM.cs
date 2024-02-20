using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LM : MonoBehaviour
{
    public GameObject Diamond;
    private void OnTriggerEnter(Collider other)
    {
        if (Diamond != null)

        if (other.gameObject.CompareTag("Stairs"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
    }
}
