using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FinesRegister.Services.SMS;

public interface ISmsService
{
    Task SendSmsAsync(string to, string message);
}

public class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;

    public SmsService(IConfiguration configuration)
    {
        _configuration = configuration;
        TwilioClient.Init(
            _configuration["Twilio:AccountSid"],
            _configuration["Twilio:AuthToken"]
        );
    }

    public async Task SendSmsAsync(string to, string message)
    {
        var from = _configuration["Twilio:FromNumber"];
        await MessageResource.CreateAsync(
            body: message,
            from: new Twilio.Types.PhoneNumber(from),
            to: new Twilio.Types.PhoneNumber(to)
        );
    }
}