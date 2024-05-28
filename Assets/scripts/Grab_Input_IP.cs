using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Grab_Input_IP : MonoBehaviour
{
    [SerializeField]GameObject Enter_Again;
    public void GrabField(string input)
    {
        Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        if (ip.IsMatch(input))
        {
            Login_info.ip = input;
        }
        else
        {
            Enter_Again.SetActive(true);
        }
    }
}
