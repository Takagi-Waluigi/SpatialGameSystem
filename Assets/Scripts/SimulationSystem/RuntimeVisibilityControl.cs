using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeVisibilityControl : MonoBehaviour
{
    [SerializeField] bool runTimeVisibility = false;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = runTimeVisibility;
    }
}
