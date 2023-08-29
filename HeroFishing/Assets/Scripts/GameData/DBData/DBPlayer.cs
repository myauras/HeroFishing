using Realms;
using MongoDB.Bson;

public partial class DBPlayer : IRealmObject {
    [PrimaryKey]
    [MapTo("_id")]
    public string ID { get; set; }
    [MapTo("authType")]
    public string AuthType { get; set; }
}