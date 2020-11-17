using System;
using UnityEngine;
// 扩展函数的说明 静态类
public static class UnityExtern
{
    // 1. 扩展函数 静态函数
    // 2. 静态函数 this参数
    public static T Find<T>(this GameObject gameObject,string name)
    {
        return gameObject.transform.Find(name).GetComponent<T>();
    }

    public static void DestroyAllChildren(this GameObject parent){
        for(int i = 0; i < parent.transform.childCount; ++i){
            var child = parent.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
    }

    //
    // c# or unity 单例模式实现的三种方法
    //

}
