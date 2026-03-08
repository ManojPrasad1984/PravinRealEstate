using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class WhatsAppService
{

    public static void SendMessage(string mobile, string message)
    {

        string accountSid = "TWILIO_SID";
        string authToken = "TWILIO_TOKEN";

        TwilioClient.Init(accountSid, authToken);

        MessageResource.Create(
            from: new PhoneNumber("whatsapp:+14155238886"),
            to: new PhoneNumber("whatsapp:+91" + mobile),
            body: message
        );
    }

}