using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public void Quit(){
        print("Quit Application");
        Application.Quit();
    }
}
