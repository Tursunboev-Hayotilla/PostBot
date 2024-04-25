using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Security.Cryptography.X509Certificates;

namespace PostBot
{
    public partial class TelegramBotHandler
       {
        public string Token { get; set; }
        public string ChanelsName { get; set; }
        public string PostsText { get; set; }

        public object PostsValue { get; set; }

        public bool IsRealChanelName = true;
        public bool IsRealText = true;
        public bool IsRealPost = true;
        public bool IsOk = false;
        public string TypeOfPost;
        public string post;
        public TelegramBotHandler(string token)
        {
            Token = token;
        }
        public async Task BotHandle()
        {
            var botClient = new TelegramBotClient($"{this.Token}");

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() 
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

           
            cts.Cancel();
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;
            var message = update.Message;
            Console.WriteLine(message.Type);
            if (message.Text == "/create_post" || message.Text == "/start")
            {

                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter chanel's username", cancellationToken: cancellationToken);

                if (message != null)
                {
                    ChanelsName = message.Text;
                    IsRealChanelName = false;
                    IsRealText = true;
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Please enter correct username ", cancellationToken: cancellationToken);
                    return;
                }

            }
            if (IsRealText)
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Please enter  text for post ", cancellationToken: cancellationToken);
                if (message.Type == MessageType.Text)
                {
                    ChanelsName = message.Text; IsRealText = false;
                    IsRealPost = true;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Please enter correct text ", cancellationToken: cancellationToken);
                    return;
                }
                return;
            }
            if (IsRealPost)
            {
                PostsText = message.Text;
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Send photo,video or link for post", cancellationToken: cancellationToken);
                TypeOfPost = message.Type.ToString();
                IsRealPost = false;

            }
            if (message.Text.StartsWith("https://"))
            {
                PostsValue = message.Text;
                IsRealPost = false;
                IsOk = true;
                await botClient.SendTextMessageAsync(chatId: chatId, text: "All right", cancellationToken: cancellationToken);


            }
            if (IsOk)
            {
                IsOk = false;
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("View Post"),
                    new KeyboardButton("Send Post"),
                    new KeyboardButton("Update Post"),
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(
chatId: message.Chat.Id,
cancellationToken: cancellationToken,
text: "No problem!",
replyMarkup: replyKeyboard);
                return;
            }
            if(TypeOfPost.Equals(MessageType.Video))
            {
                await botClient.SendVideoAsync(
                    chatId: chatId, video: (InputFile)PostsValue, caption: ChanelsName, cancellationToken: cancellationToken);
            }


            if (message.Text == "View Post")
            {
                post = $"@{ChanelsName}\n{PostsText}\n{PostsValue}";

                await botClient.SendTextMessageAsync(chatId: chatId, text: post, cancellationToken: cancellationToken
                    );
            }
            else if (message.Text == "Send Post")
            {
                post = $"@{ChanelsName}\n{PostsText}\n{PostsValue}";
                await botClient.SendTextMessageAsync(
                chatId: "@dotNetDontMess", text: post, cancellationToken: cancellationToken);
            }
            else if (message.Text == "Update Post")
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Not aviable now", cancellationToken: cancellationToken);
            }
               /* var replyKeyboard2 = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Update Chanel's username"),
                    new KeyboardButton("Update Post's text"),
                    new KeyboardButton("Update Photo/Video"),
                    new KeyboardButton("<- Back"),
                })
                {
                    ResizeKeyboard = true
                };
                Console.WriteLine(message.Type);
                await botClient.SendTextMessageAsync(chatId: chatId, text: "What do you want to update", replyMarkup: replyKeyboard2, cancellationToken: cancellationToken);
                IsOk2 = true;
            }
            if (message.Text == "Update Chanel's username")
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new chanel name", cancellationToken: cancellationToken);
                ChanelsName = message.Text;
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Updated ", cancellationToken: cancellationToken);
                return;

            }
            else if (message.Text == "Update Post's text")
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new post text", cancellationToken: cancellationToken);
                PostsText = message.Text;
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Updated ", cancellationToken: cancellationToken);
                return;
            }
            else if (message.Text == "Update Photo/Video")
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new post photo/video", cancellationToken: cancellationToken);
                PostsValue = message.Text;
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Updated ", cancellationToken: cancellationToken);
                return;

            }
            else if (message.Text == "<- Back")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                    new KeyboardButton("View Post"),
                    new KeyboardButton("Send Post"),
                    new KeyboardButton("Update Post"),
                })
                {
                    ResizeKeyboard = true
                };
            }
            if (IsOk2)
            {
                if (message.Text == "View Post")
                {

                    post = $"@{ChanelsName}\n{PostsText}\n{PostsValue}";

                    await botClient.SendTextMessageAsync(chatId: chatId, text: post, cancellationToken: cancellationToken
                        );
                }
                else if (message.Text == "Send Post")
                {
                    post = $"@{ChanelsName}\n{PostsText}\n{PostsValue}";
                    await botClient.SendTextMessageAsync(
                    chatId: "@dotNetDontMess", text: post, cancellationToken: cancellationToken);
                }
                else if (message.Text == "Update Post")
                {
                }
                return;
            }*/
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}