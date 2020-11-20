using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using XLua;
using System.IO;

namespace StarForce
{
    [CSharpCallLua]
    public delegate void TowFNumbers(float v1, float v2);
    [CSharpCallLua]
    public delegate void DOnClose(bool isShutdown, object userData);
    [CSharpCallLua]
    public delegate void DOnInit(ProcedureMenu procedureMenu);
    public class LuaForm : UIFormLogic
    {
        private DOnInit LuaOnInit;
        private Action LuaOnRecycle;
        private Action<object> LuaOnOpen;
        private Action LuaOnClose;
        private Action LuaOnPause;
        private Action LuaOnResume;
        private Action LuaOnCover;
        private Action LuaOnReveal;
        private Action<object> LuaOnRefocus;
        private Action LuaOnUpdate;
        private Action LuaOnDepthChanged;
        private Action LuaInternalSetVisible;
        private TowFNumbers towNumbers;
        private DOnClose dOnClose;

        public const int DepthFactor = 100;

        private const float FadeTime = 0.3f;

        private static Font s_MainFont = null;
        
        private Canvas m_CachedCanvas = null;
        
        private CanvasGroup m_CanvasGroup = null;
        
        private List<Canvas> m_CachedCanvasContainer = new List<Canvas>();

        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return m_CachedCanvas.sortingOrder;
            }
        }

        private void check(Transform transform, LuaTable luaTable)
        {
            luaTable.Set(transform.name, transform);
            Transform[] tform = transform.GetComponents<Transform>();
            for(int i = 0; i < tform.Length; i++)
            {
                this.check(tform[i], luaTable);
            }
        }

        public void luaInit()
        {
            LuaEnv luaEnv = GameEntry.Lua.GetLuaEnv();
            LuaTable scriptEnv = luaEnv.NewTable();
            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);

            meta.Dispose();

            scriptEnv.Set("self", this);
            // 防止误操作 改良一下做法
            string gameObjectName = this.transform.name;
            if(gameObjectName.Contains("(Clone)"))
            {
                gameObjectName = gameObjectName.Split(new string[] { "(Clone)" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            //string contents = "require(\"" + nluas[0] + "\")";
            byte[] contents = File.ReadAllBytes(Application.dataPath + "/LuaScripts/" + gameObjectName + ".lua");
            luaEnv.DoString(contents, gameObjectName, scriptEnv);
            Debug.Log("gameObjectName : "+ gameObjectName);
            LuaOnInit = scriptEnv.GetInPath<DOnInit>(gameObjectName+".OnInit");
            LuaOnRecycle = scriptEnv.GetInPath<Action>(gameObjectName + ".OnRecycle");
            LuaOnOpen = scriptEnv.GetInPath<Action<object>>(gameObjectName + ".OnOpen");

            dOnClose = scriptEnv.GetInPath<DOnClose>(gameObjectName + ".OnClose");
            LuaOnPause = scriptEnv.GetInPath<Action>(gameObjectName + ".OnPause");
            LuaOnResume = scriptEnv.GetInPath<Action>(gameObjectName + ".OnResume");

            LuaOnCover = scriptEnv.GetInPath<Action>(gameObjectName + ".OnCover");
            LuaOnReveal = scriptEnv.GetInPath<Action>(gameObjectName + ".OnReveal");
            LuaOnRefocus = scriptEnv.GetInPath<Action<object>>(gameObjectName + ".OnRefocus");

            towNumbers = scriptEnv.GetInPath<TowFNumbers>(gameObjectName + ".OnUpdate");
            LuaOnDepthChanged = scriptEnv.GetInPath<Action>(gameObjectName + ".OnDepthChanged");
            LuaInternalSetVisible = scriptEnv.GetInPath<Action>(gameObjectName + ".InternalSetVisible");
        }

        public void Close()
        {
            Close(false);
        }

        public void Close(bool ignoreFade)
        {
            StopAllCoroutines();

            if (ignoreFade)
            {
                GameEntry.UI.CloseUIForm(this);
            }
            else
            {
                StartCoroutine(CloseCo(FadeTime));
            }
        }

        public static void SetMainFont(Font mainFont)
        {
            if (mainFont == null)
            {
                Log.Error("Main font is invalid.");
                return;
            }

            s_MainFont = mainFont;
        }

        protected void safeInove(Action action)
        {
            if(action != null)
            {
                action();
            }
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            luaInit();
            if(LuaOnInit != null)
            {
                LuaOnInit((ProcedureMenu)userData);
            }
            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            OriginalDepth = m_CachedCanvas.sortingOrder;
            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();

            /*
            此处代码需要新增特性标签 
            Text[] texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].font = s_MainFont;
                if (!string.IsNullOrEmpty(texts[i].text))
                {
                    texts[i].text = GameEntry.Localization.GetString(texts[i].text);
                }
            }
            */
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            safeInove(LuaOnRecycle);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            if(LuaOnOpen != null)
            {
                LuaOnOpen(userData);
            }
            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            if(dOnClose != null)
            {
                dOnClose(isShutdown, userData);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            safeInove(LuaOnPause);
        }

        protected override void OnResume()
        {
            base.OnResume();
            safeInove(LuaOnResume);
            
            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
            
        }

        protected override void OnCover()
        {
            base.OnCover();
            safeInove(LuaOnCover);
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            safeInove(LuaOnReveal);
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            if(LuaOnRefocus != null)
            {
                LuaOnRefocus(userData);
            }
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) { 
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            //safeInove(LuaOnUpdate);
            if(towNumbers != null)
            {
                towNumbers(elapseSeconds, realElapseSeconds);
            }
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            GetComponentsInChildren(true, m_CachedCanvasContainer);
            for (int i = 0; i < m_CachedCanvasContainer.Count; i++)
            {
                m_CachedCanvasContainer[i].sortingOrder += deltaDepth;
            }

            m_CachedCanvasContainer.Clear();
            //safeInove(LuaOnDepthChanged);
            if(LuaOnDepthChanged != null)
            {
                LuaOnDepthChanged();
            }
        }

        private IEnumerator CloseCo(float duration)
        {
            yield return m_CanvasGroup.FadeToAlpha(0f, duration);
            GameEntry.UI.CloseUIForm(this);
        }
    }
}
