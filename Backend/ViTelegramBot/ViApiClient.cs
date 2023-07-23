using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;
using ViTelegramBot.Entities;
using ViTelegramBot.Http;

namespace ViTelegramBot;

public class ViApiClient
{
    private ViSessionList SessionList { get; }
    private ViApiHttpClient ApiHttpClient { get; }
    public ViApiClient(string hostname, string sessionListPath)
    {
        SessionList = new ViSessionList(sessionListPath);
        ApiHttpClient = new ViApiHttpClient(hostname);
    }
/*    public ViResult<object> GetDicts(ChatId id)
    {
        string methodName = nameof(GetDicts);

        ViResult<string> jwtResult = GetJwt(id);
        if (jwtResult.ResultCode is ViResultTypes.NotFounded && jwtResult.ResultValue is null)
        {
            return new ViResult<object>(ViResultTypes.NotFounded, null, methodName, jwtResult.Message);
        }

        HttpClient httpClient = new HttpClient();


    }*/
    public ViResult<string> GetJwt(long id)
    {
        string methodName = nameof(GetJwt);

        ViSession? session = SessionList.Where(s => s.TelegramId == id).FirstOrDefault();
        
        if (session is not null && (session.IsNotExpried() is true)) //Если Jwt есть в сессиях и действителен
        {
            return new ViResult<string>(ViResultTypes.Founded, session.JwtToken, methodName, $"Jwt for chatId: {id} was found in local storage.");
        }

        if (session is not null && (session.IsNotExpried() is false)) //Если Jwt есть в сессиях, но устарел
        {
            ViResult<string> apiResult = ApiHttpClient.GetJwtFromApi(id);
            if (apiResult.ResultCode is ViResultTypes.Founded && apiResult.ResultValue is not null)
            {
                string jwtToken = apiResult.ResultValue;
                SessionList.Remove(session);
                SessionList.Add(new ViSession(id, jwtToken));
                return new ViResult<string>(ViResultTypes.Founded, jwtToken, methodName, $"Jwt for chatId: {id} was found in API database and updated locally.");
            }
        }


        { ViResult<string> apiResult = ApiHttpClient.GetJwtFromApi(id);
        if (apiResult.ResultCode is ViResultTypes.Founded && apiResult.ResultValue is not null)
        {
            string jwtToken = apiResult.ResultValue;
            SessionList.Add(new ViSession(id, jwtToken));
            return new ViResult<string>(ViResultTypes.Founded, jwtToken, methodName, $"Jwt for chatId: {id} was found in API database and added locally.");
        }}


        return new ViResult<string>(ViResultTypes.NotFounded, null!, methodName, $"Jwt for chatId: {id} was not found.");



    }
}
