using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Death : MonoBehaviour
{
    [SerializeField] GameObject player;
    int lives;
    void Start()
    {
        lives = 3;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnBecameInvisible()
    {
        lives--;
        if (lives == 0)
        {
            player.SetActive(false);
        }
        else
        {
            player.GetComponent<Rigidbody2D>().transform.position = new Vector3(0, 0, 0);
        }
    }
}
