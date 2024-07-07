using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=fsDE_mO4RZM
public class PoolableObject : MonoBehaviour
{
    public ObjectPool Parent;

    public virtual void OnDisable()
    {
        Parent.ReturnObjectToPool(this);
    }


}
