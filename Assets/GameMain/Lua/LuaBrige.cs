/**
    在我能想到的办法里 也就是加一个桥接类比较靠谱
    如果，谁有更好的办法，可以联系我，不给钱的哟 ^_^;
**/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;
namespace StarForce
{
    [LuaCallCSharp]
    public class LuaBrige
    {
        /// <summary>
        /// 获取 Localization 的对应文本
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [LuaCallCSharp]
        public static string LocalizationGetString(string key)
        {
            // Debug.Log("ids : " + key);
            return GameEntry.Localization.GetString(key);
        }

        [LuaCallCSharp]
        public static void UIOpenUIForm(int id)
        {
            // Debug.Log("ids : " + id);
            GameEntry.UI.OpenUIForm(id);
        }

        /// <summary>
        /// 对话框 因为这里只有一个退出功能,偷个懒,没有对这个进行剥离
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        [LuaCallCSharp]
        public static void UIOpenDiagloge(string title, string message)
        {
            GameEntry.UI.OpenDialog(new DialogParams()
            {
                Mode = 2,
                Title = GameEntry.Localization.GetString(title),
                Message = GameEntry.Localization.GetString(message),
                OnClickConfirm = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
            });
        }
    }
}

