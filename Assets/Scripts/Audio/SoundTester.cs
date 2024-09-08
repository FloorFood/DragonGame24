using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            AudioManager.instance.PlaySound("move ghost");
        }
        if (Input.GetKeyDown("s"))
        {
            AudioManager.instance.PlaySound("move hand");
        }
        if (Input.GetKeyDown("d"))
        {
            AudioManager.instance.PlaySound("slap ghost");
        }
        if (Input.GetKeyDown("f"))
        {
            AudioManager.instance.PlaySound("slap miss");
        }
        if (Input.GetKeyDown("g"))
        {
            AudioManager.instance.PlaySound("approach 1");
        }
        if (Input.GetKeyDown("h"))
        {
            AudioManager.instance.PlaySound("approach 2");
        }
        if (Input.GetKeyDown("j"))
        {
            AudioManager.instance.PlaySound("approach 3");
        }
        if (Input.GetKeyDown("k"))
        {
            AudioManager.instance.PlaySound("approach 4");
        }
        if (Input.GetKeyDown("l"))
        {
            AudioManager.instance.PlaySound("jumpscare");
        }
    }
}