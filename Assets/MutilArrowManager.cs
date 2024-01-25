using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutilArrowManager : MonoBehaviour
{
    public static MutilArrowManager _Instance;

    public bool 万箭齐发;
    public int 箭支数目 = 5;

    // Start is called before the first frame update
    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
