using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public float waitForTime = 5f;
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(waitForTime);
        Destroy(gameObject);
    }
}
