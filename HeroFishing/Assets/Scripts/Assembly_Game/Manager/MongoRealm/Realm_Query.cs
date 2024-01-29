using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitJson;
using MongoDB.Bson;
using Realms;
using Realms.Sync;
using Scoz.Func;

namespace Service.Realms {
    public static partial class RealmManager {

        public static async UniTask<BsonDocument> Query_GetDoc(string _dbCol, string _id) {
            var col = MyDB.GetCollection<BsonDocument>(_dbCol);
            var dbMatchgame = await col.FindOneAsync(new { _id = _id });
            return dbMatchgame;
        }
    }
}