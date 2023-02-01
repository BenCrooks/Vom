using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reloadMaterial : MonoBehaviour
{
    [SerializeField] Material matToLoad;
    // Start is called before the first frame update
    void Start()
    {
        // GetComponent<SpriteRenderer>().material = Resources.Load(matToLoad.name, Material);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
