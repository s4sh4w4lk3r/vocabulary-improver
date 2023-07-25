using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViTelegramBot.Http.JsonEntites
{
    public record class ApiResponse(int ResultCode, string ResultValue, string MethodName, string Message);
}
