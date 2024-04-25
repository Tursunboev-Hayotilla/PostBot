namespace PostBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string token = "6896150607:AAE2zr-u0CkGQEspW5V4JTIHQ11i7zzZKzI";

            TelegramBotHandler handler = new TelegramBotHandler(token);

            try
            {
                await handler.BotHandle();
            }
            catch (Exception ex)
            {
                throw new Exception("What's up");
            }
        }
    }
}
