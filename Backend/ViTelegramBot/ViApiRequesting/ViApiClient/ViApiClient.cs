using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ViTelegramBot.Entities;

namespace ViTelegramBot.ApiRequesting;

public partial class ViApiClient
{
    private ViSessionList SessionList { get; }
    private ViApiHttpClient ApiHttpClient { get; }
    public ViApiClient(ServiceProvider serviceProvider)
    {
        string sessionListPath = serviceProvider.GetRequiredService<IConfiguration>().GetRequiredSection("SessionsPath").Value!;
        SessionList = new ViSessionList(sessionListPath);

        ApiHttpClient = new ViApiHttpClient(serviceProvider);
    }


    private async Task<ViResult<string>> GetJwtAsync(long id)
    {
        string methodName = nameof(GetJwtAsync);

        ViSession? session = SessionList.Where(s => s.TelegramId == id).FirstOrDefault();

        if (session is not null && (session.IsNotExpried() is true)) //Если Jwt есть в сессиях и действителен
        {
            return new ViResult<string>(ViResultTypes.Founded, session.JwtToken, methodName, $"Jwt for chatId: {id} was found in local storage.");
        }


        if (session is not null && (session.IsNotExpried() is false)) //Если Jwt есть в сессиях, но устарел
        {
            ViResult<string> apiResult = await ApiHttpClient.GetJwtFromApiAsync(id);
            if (apiResult.ResultCode is ViResultTypes.Founded && apiResult.ResultValue is not null)
            {
                string jwtToken = apiResult.ResultValue;
                SessionList.Remove(session);
                SessionList.Add(new ViSession(id, jwtToken));
                return new ViResult<string>(ViResultTypes.Founded, jwtToken, methodName, $"Jwt for chatId: {id} was found in API database and updated locally.");
            }
        }

        {
            ViResult<string> apiResult = await ApiHttpClient.GetJwtFromApiAsync(id);
            if (apiResult.ResultCode is ViResultTypes.Founded && apiResult.ResultValue is not null)
            {
                string jwtToken = apiResult.ResultValue;
                SessionList.Add(new ViSession(id, jwtToken));
                return new ViResult<string>(ViResultTypes.Founded, jwtToken, methodName, $"Jwt for chatId: {id} was found in API database and added locally.");
            }
        }

        return new ViResult<string>(ViResultTypes.Fail, null!, methodName, $"Jwt for chatId: {id} was not found.");
    }

    public async Task<ViResult<string>> SignUpUserAsync(long id, string firstname)
    {
        string methodName = nameof(SignUpUserAsync);

        ViResult<string> tryGetJwtResult = await GetJwtAsync(id);
        if (tryGetJwtResult.ResultCode is ViResultTypes.Founded || tryGetJwtResult.ResultValue is not null)
        {
            return new ViResult<string>(ViResultTypes.Founded, tryGetJwtResult.ResultValue, methodName, $"Jwt for chatId: {id} already exists.");
        }

        ViResult<string> addApiResult = await ApiHttpClient.AddUserToApi(id, firstname);
        if (addApiResult.ResultCode is ViResultTypes.Created && addApiResult.ResultValue is not null)
        {
            addApiResult.MethodName = methodName;
            return addApiResult;
        }

        return new ViResult<string>(ViResultTypes.Fail, null, methodName, $"The user chatid:{id} was not able to add.");
    }

}
