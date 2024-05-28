using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Grab_Input_Password : MonoBehaviour
{
    [SerializeField] GameObject Enter_Again2;
    public void GrabField(string input)
    {
        Regex Password = new Regex(@"\b\d{6}\b");
        if (Password.IsMatch(input))
        {
            Debug.Log("set ip");
            Login_info.password = input;
        }
        else
        {
            Enter_Again2.SetActive(true);
        }
    }
}
