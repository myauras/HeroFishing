using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
using Service.Realms;
using System.Threading.Tasks;
using Scoz.Func;
using Cysharp.Threading.Tasks;

[MapTo("playerState")]
public partial class DBPlayerState : IRealmObject {
    [Required]
    [MapTo("_id")]
    [PrimaryKey]
    public string ID { get; private set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; private set; }
    [MapTo("lastUpdateAt")]
    public DateTimeOffset LastUpdateAt { get; private set; }
    [MapTo("inMatchgameID")]
    [Required]
    public string InMatchgameID { get; private set; }

    public void SetLastUpdateAt(DateTimeOffset _lastUpdateAt) {
        RealmManager.MyRealm.WriteAsync(() => {
            LastUpdateAt = _lastUpdateAt;
        });
    }
    /// <summary>
    /// 呼叫時機為: 1.收到Matchmaker建立/加入房間成功後呼叫 2. 離開遊戲房時傳入(null)將玩家所在Matchgame(遊戲房)清掉
    /// 建立/加入房間時會設定所在Matchgame(遊戲房)的ID並訂閱DBMatchgame資料，若Server房間創好後會收到通知讓玩家主動scoket到Matchgame Server
    /// 離開遊戲房時將InMatchgameID設回null並取消訂閱
    /// </summary>
    public async UniTask SetInMatchgameID(string _matchgameID) {
        await RealmManager.MyRealm.WriteAsync(() => {
            InMatchgameID = _matchgameID;
        });
        if (!string.IsNullOrEmpty(InMatchgameID)) {
            await RealmManager.Subscribe_Matchgame();
        } else {
            RealmManager.Unsubscribe_Matchgame();
        }
    }

}