//using System;
//using System.Collections.Generic;
//using Realms;
//using MongoDB.Bson;
//using System.Diagnostics;

//public partial class DBData : IRealmObject {
//    [MapTo("_id")]
//    [PrimaryKey]
//    public ObjectId? ID { get; set; }
//    [MapTo("createdAt")]
//    public DateTimeOffset? createdAt { get; set; }
//    [MapTo("updatedAt")]
//    public DateTimeOffset? updatedAt { get; set; }

//}

//[MapTo("Timestamp")]
//public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;