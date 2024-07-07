using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerString : MonoBehaviour
{
    private string Tag;
    public void SetTag(string tag)
    {
        this.Tag = tag;
    }

    public string GetTag()
    {
        return Tag;
    }
}
