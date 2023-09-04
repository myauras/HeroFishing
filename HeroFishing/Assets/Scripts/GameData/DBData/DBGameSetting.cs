using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;

public enum DBGameSettingDoc {
    timer,
    address,
    gameState,
    scheduledInGameNotification,
}

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
    [MapTo("CustomerServiceURL")]
    public string CustomerServiceURL { get; set; }
    #endregion


    #region gameState(目前遊戲狀態的資料)
    [MapTo("envVersion")]
    public string EnvVersion { get; set; }
    [MapTo("gameVersion")]
    public string GameVersion { get; set; }
    [MapTo("maintain")]
    public bool? Maintain { get; set; }
    [MapTo("maintainEndTime")]
    public DateTimeOffset? MaintainEndTime { get; set; }
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
    [MapTo("endTime")]
    public DateTimeOffset? ScheduledNoticication_EndTime { get; set; }
    [MapTo("startTime")]
    public DateTimeOffset? ScheduledNoticication_StartTime { get; set; }
    [MapTo("enable")]
    public bool? ScheduledNoticication_Enable { get; set; }
    [MapTo("index")]
    public int? ScheduledNoticication_Index { get; set; }
    #endregion


}