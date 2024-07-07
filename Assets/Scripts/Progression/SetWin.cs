using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ProgressManager.GetInstance().setWin(true);   
    }
}
