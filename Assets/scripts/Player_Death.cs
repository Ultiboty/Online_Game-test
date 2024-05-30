using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Player_Death : MonoBehaviour
{
    [SerializeField] GameObject player;
    public int lives=3;
    public Image[] Hearts;
    public Sprite EmptyHeart;
    int player_num = Login_info.counter - 1;
    public Image cup;


    void OnBecameInvisible()
    {
        lives--;
        Hearts[lives].sprite = EmptyHeart;
        if (lives == 0)
        {
            player.SetActive(false);
            player_num--;
            if (player_num == 1)
            {
                cup.rectTransform.position = new Vector3(-18, 350, 0);
            }
        }
        else
        {
            player.GetComponent<Rigidbody2D>().transform.position = new Vector3(0, 0, 0);
        }
    }
}
