using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using System.Linq;
using System;

namespace TheDoor.Main {

    public abstract class OwnedData : IScozJsonConvertible {
        [ScozSerializable] public string UID { get; private set; }
        [ScozSerializable] public string OwnerUID { get; private set; }
        [ScozSerializable] public DateTime CreateTime { get; private set; }

        public OwnedData(Dictionary<string, object> _data) {
            SetData(_data);
        }
        public virtual void SetData(Dictionary<string, object> _data) {
            object value;
            UID = _data.TryGetValue("UID", out value) ? Convert.ToString(value) : default(string);
            OwnerUID = _data.TryGetValue("OwnerUID", out value) ? Convert.ToString(value) : default(string);
            //CreateTime = _data.TryGetValue("CreateTime", out value) ? FirebaseManager.GetDateTimeFromFirebaseTimestamp(value) : default(DateTime);
        }
    }
}
