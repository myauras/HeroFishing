using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;

[MapTo("gameSetting")]
public partial class DBGameSetting : IRealmObject {
    [Required]
    [MapTo("_id")]
    [PrimaryKey]
    public string ID { get; set; }
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    #region timer(各計時器的時間設定)
    [MapTo("onlineCheckSec")]
    public int? OnlineCheckSec { get; set; }
    #endregion

    #region address(各網址設定)
    [MapTo("storeURL_Apple")]
    public string StoreURL_Apple { get; set; }
    [MapTo("storeURL_Google")]
    public string StoreURL_Google { get; set; }
    [MapTo("customerServiceURL")]
    public string CustomerServiceURL { get; set; }
    #endregion


    #region gameState(目前遊戲狀態的資料)
    [MapTo("envVersion")]
    public string EnvVersion { get; set; }
    [MapTo("gameVersion")]
    public string GameVersion { get; set; }
    [MapTo("maintain")]
    public bool? Maintain { get; set; }
    [MapTo("maintainEndAt")]
    public DateTimeOffset? MaintainEndAt { get; set; }
    [Required]
    [MapTo("maintainExemptPlayerIDs")]
    public IList<string> MaintainExemptPlayerIDs { get; }
    [MapTo("minimumGameVersion")]
    public string MinimumGameVersion { get; set; }
    [MapTo("matchmakerEnable")]
    public bool? MatchmakerEnable { get; set; }
    [MapTo("matchmakerIP")]
    public string MatchmakerIP { get; set; }
    [MapTo("matchmakerPort")]
    public int? MatchmakerPort { get; set; }
    #endregion



    #region scheduledInGameNotification(遊戲內跳通知設定)
    [MapTo("content")]
    public string ScheduledNoticication_Content { get; set; }
    [MapTo("startAt")]
    public DateTimeOffset? StartAt { get; set; }
    [MapTo("endAt")]
    public DateTimeOffset? EndAt { get; set; }
    [MapTo("enable")]
    public bool? Enable { get; set; }
    [MapTo("index")]
    public int? Index { get; set; }
    #endregion


}