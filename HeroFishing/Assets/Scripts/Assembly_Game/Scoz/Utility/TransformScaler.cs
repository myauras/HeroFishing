using HeroFishing.Main;
using HeroFishing.Socket;
using HeroFishing.Socket.Matchgame;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

using UnityEngine.Rendering.Universal;

namespace Scoz.Func{
    public class TransformScaler {
        public static void ScaleImgSizeByScreenSize(Transform _trans, Vector2 _refSize) {
            float scaleRatio = ((float)Screen.width / (float)Screen.height) / (_refSize.x / _refSize.y);
            _trans.localScale = Vector3.one*  scaleRatio;
            WriteLog.LogError("_trans.localScale=" + _trans.localScale);
        }
    }
}