using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpFOV : MonoBehaviour
{
    Camera mainCam;
    float initialFov;
    [SerializeField] public float endFov = 160f;
    [SerializeField] public float speed = 1.0f;
    [SerializeField] public float timeInbetween = 5f;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GetComponent<Camera>();
        initialFov = mainCam.fieldOfView;
        StartCoroutine(increaseFOV());
    }


    IEnumerator increaseFOV()
    {

        for(float t = 0; t <= 1; t += Time.deltaTime * speed)
        {
            mainCam.fieldOfView = Mathf.Lerp(initialFov, endFov, t);
            yield return null;
        }

        yield return new WaitForSeconds(timeInbetween);
        StartCoroutine(decreaseFOV());

    }

    IEnumerator decreaseFOV()
    {

        for (float t = 0; t <= 1; t += Time.deltaTime * (speed * 2))
        {
            mainCam.fieldOfView = Mathf.Lerp(endFov, initialFov, t);
            yield return null;
        }

    }




}
