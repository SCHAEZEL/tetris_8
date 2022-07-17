using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        public bool init = false;
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Init()
        {
            base.Init();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;

            GameThreadQueue.Init();
            Network.Init();

            // 设置字体文件
            UIConfig.defaultFont = "SourceHanSansCN_Medium SDF";

            // 启动 LuaManager
            LuaManager.Instance.Startup();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!init)
                return;

            // 调用UI
            LuaState luastate = LuaManager.Instance.GetLuaState();

            //Get the function object
            LuaFunction luaFunc = luastate.GetFunction("GameLogic.OnApplicationFocus");
            if (luaFunc != null)
            {
                luaFunc.BeginPCall();
                luaFunc.Push(focus);
                luaFunc.PCall();
                luaFunc.EndPCall();
            }

            // 针对安卓手机，锁旋转会失效的解决方案
            if (focus)
                ToggleAutoRotation();
        }

        private void ToggleAutoRotation()
        {
            bool AutoRotationOn = DeviceAutoRotationIsOn();
            // 竖屏
            Screen.autorotateToPortrait = AutoRotationOn;
            Screen.autorotateToPortraitUpsideDown = AutoRotationOn;

            // 横屏
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;

            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        private bool DeviceAutoRotationIsOn()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
    using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var context = actClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass systemGlobal = new AndroidJavaClass("android.provider.Settings$System");
            var rotationOn = systemGlobal.CallStatic<int>("getInt", context.Call<AndroidJavaObject>("getContentResolver"), "accelerometer_rotation");
 
            return rotationOn == 1;
        }
#endif
            return true;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy()
        {
            GameThreadQueue.Shutdown();
            Network.Shutdown();

            if (LuaManager.Instance != null)
            {
                LuaManager.Instance.Close();
            }

            Debug.Log("~GameManager was destroyed");
        }

        public void Update()
        {
            GameThreadQueue.Update();
        }
    }