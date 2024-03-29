﻿using MongoDB.Bson.Serialization.Attributes;
using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Enums;

namespace ViApi.Types.Telegram;

[BsonIgnoreExtraElements]
public class TelegramSession
{
    public Guid UserGuid { get; set; }
    public Guid DictionaryGuid {get; set;}
    public Queue<Word>? GameQueue { get; set; }
    public UserState State { get; set; }
    public int MessageIdToEdit { get; set; }
    public long TelegramId { get; init; }

    private TelegramSession() { }
    public TelegramSession(TelegramUser user, UserState state = UserState.Default)
    {
        UserGuid = user.Guid;
        State = state;
        TelegramId = user.TelegramId;
    }

    public override string ToString() => $"UserGuid: {UserGuid}, TelegramId: {TelegramId}, Selected DictGuid: {DictionaryGuid} State: {State}";
}
