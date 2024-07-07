using System.Collections;
using UnityEngine;

public class WarpTranstion : MonoBehaviour
{
    private float timeElapsed;
    // Start is called before the first frame update

    [SerializeField] private float time = 24f;
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= time)
        {

            PlayerLevelManager.MoveNext();
        }
    }
    
}
