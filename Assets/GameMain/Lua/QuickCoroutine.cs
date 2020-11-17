using System;
using System.Collections;
using UnityEngine;

public class QuickCoroutine
{
    GameObject _root;
    MonoBehaviour _coroutineMono;

    public void Init(){
        _root = new GameObject("QuickCoroutine");       
        GameObject.DontDestroyOnLoad(_root);
        _coroutineMono = _root.AddComponent<MonoBehaviour>();
    }

    public Coroutine StartCorontine(IEnumerator routine){
        return _coroutineMono.StartCoroutine(routine);
    }
    
}
