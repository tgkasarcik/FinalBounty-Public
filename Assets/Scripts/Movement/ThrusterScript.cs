using UnityEngine;
using UnityEngine.InputSystem;

public class ThrusterScript : MonoBehaviour
{
    private bool slowSpeed = true;
    private Vector3 originalScale;

    [SerializeField] float scaleMultiplier = 2.0f;
    private ParticleSystem pSystem;

    // Start is called before the first frame update
    void OnEnable()
    {
        originalScale = transform.localScale;
        pSystem = GetComponent<ParticleSystem>();
    }
    public void OnHighSpeed(InputValue value)
    {
        slowSpeed = !slowSpeed;

        if (!slowSpeed)
        {
            transform.localScale = originalScale * scaleMultiplier;
        }
        else
        {
            transform.localScale = originalScale;
        }


    }
}
