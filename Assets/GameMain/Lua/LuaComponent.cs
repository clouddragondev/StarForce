using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GameFramework.Resource;
using XLua;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public class LuaComponent : GameFrameworkComponent
    {
        private Dictionary<string, byte[]> m_CachedLuaScripts = new Dictionary<string, byte[]>();
        private LuaEnv luaEnv = null;
        public delegate void OnLoadScriptSuccess(string fileName);
        public delegate void OnLoadScriptFailure(string fileName, LoadResourceStatus status, string errorMessage);
        public const string LuaAssetExtInBundle = ".lua";
        //
        //  GameFrameworkComponent的派生类
        //  在你不想惹事的情况下 不要覆盖awake方法
        // 
        protected void Start()
        {
            this.luaEnv = new LuaEnv();
            // 新增自定义装载
            this.luaEnv.AddLoader(CustomLoader);
            this.luaEnv.DoString("require('main')");
            // this.luaEnv.DoString("package.path = {0}");
            // 加载依赖的动态库
            // this.luaEnv.AddBuildin("luasocket", null);
        }

        public LuaEnv GetLuaEnv()
        {
            return this.luaEnv;
        }

        public void SafeDoString(string scriptContent, string chunkName = "chunk")
        {
            if (this.luaEnv != null)
            {
                try
                {
                    this.luaEnv.DoString(scriptContent, chunkName);
                }
                catch (System.Exception ex)
                {
                    string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                    Debug.LogError(msg, null);
                }
            }
        }
        public static byte[] CustomLoader(ref string filepath)
        {
            filepath = Application.dataPath + "/LuaScripts/" + filepath;
            // 标记一下 lua脚本的格式类型可以在这里设置
            filepath = filepath.Replace(".", "/") + ".lua";
            
            if(GameEntry.Lua.m_CachedLuaScripts.ContainsKey(filepath))
            {
                return GameEntry.Lua.m_CachedLuaScripts[filepath];
            }

            byte[] newvalue = File.ReadAllBytes(filepath);
            GameEntry.Lua.m_CachedLuaScripts[filepath] = newvalue;

            return newvalue;
        }
        public void LoadFile(string assetPath, string fileName, OnLoadScriptSuccess onSuccess, OnLoadScriptFailure onFailure = null)
        {
            //if (m_CachedLuaScripts.ContainsKey(fileName) || Application.isEditor && GameEntry.GetComponent<BaseComponent>().EditorResourceMode)
            //{
                if (onSuccess != null)
                {
                    onSuccess(fileName);
                }

                return;
            //}

            // Load lua script from AssetBundle.
            var innerCallbacks = new LoadAssetCallbacks(
                loadAssetSuccessCallback: OnLoadAssetSuccess,
                loadAssetFailureCallback: OnLoadAssetFailure);
            var userData = new LoadLuaScriptUserData { FileName = fileName, OnSuccess = onSuccess, OnFailure = onFailure };
            // 如果是debug 走 .lua
            // 如果是发布版本走 .bytes
            assetPath += LuaAssetExtInBundle;
            //GameEntry.GetComponent<ResourceComponent>().LoadAsset(assetPath, innerCallbacks, userData);
        }

        //public void OnDestroy()
        //{
        //    if (this.luaEnv != null)
        //    {
        //        this.luaEnv.Dispose();
        //    }
        //    this.luaEnv = null;
        //}

        private void Update()
        {
            if (this.luaEnv != null)
            {
                this.luaEnv.Tick();
            }
        }

        private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            var myUserData = userData as LoadLuaScriptUserData;
            if (myUserData == null) return;

            if (myUserData.OnFailure != null)
            {
                myUserData.OnFailure(myUserData.FileName, status, errorMessage);
            }
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            var myUserData = userData as LoadLuaScriptUserData;
            TextAsset textAsset = asset as TextAsset;
            if (textAsset == null)
            {
                throw new GameFramework.GameFrameworkException("The loaded asset should be a text asset.");
            }

            if (!m_CachedLuaScripts.ContainsKey(myUserData.FileName))
            {
                m_CachedLuaScripts.Add(myUserData.FileName, textAsset.bytes);
            }

            if (myUserData.OnSuccess != null)
            {
                myUserData.OnSuccess(myUserData.FileName);
            }
        }

        private class LoadLuaScriptUserData
        {
            public string FileName;
            public OnLoadScriptSuccess OnSuccess;
            public OnLoadScriptFailure OnFailure;
        }
    }
}