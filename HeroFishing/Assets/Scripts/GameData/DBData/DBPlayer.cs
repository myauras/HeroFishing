using Realms;
using MongoDB.Bson;

public partial class DBPlayer : IRealmObject {
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId ID { get; set; }
    [MapTo("name")]
    public string Name { get; set; }
}