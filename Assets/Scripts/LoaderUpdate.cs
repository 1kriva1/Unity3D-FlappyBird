    using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LoaderUpdate : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {        
        Loader.LoadTargetScene();
    }
}
