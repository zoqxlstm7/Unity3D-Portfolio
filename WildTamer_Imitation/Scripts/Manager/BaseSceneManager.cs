using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneManager : MonoBehaviour
{
    #region Unity Methods
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
    #endregion UnityMethods

    #region Ohter Methods
    protected virtual void InitialzieScene() { }
    protected virtual void UpdateScene() { }
    #endregion Other Methods
}
