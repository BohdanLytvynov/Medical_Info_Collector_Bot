using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using System.IO;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using static System.IO.File;
using OCR_Core;
using Bar_Code_Core;

namespace Collector_Bot_Api
{
    public class CollectorBot
    {
        #region Events

        string m_pathToToken;

        string m_pathToTempPhotos;

        ReceiverOptions m_recOptions;

        #endregion

        #region Fields

        Bar_Code_Core.IronBarCode m_BarCode;

        OCR m_OCR;

        TelegramBotClient m_bot;

        CancellationTokenSource m_cts;

        #endregion

        #region Properties

        #endregion

        #region Ctor
        public CollectorBot()
        {
            m_BarCode = new Bar_Code_Core.IronBarCode();

            m_OCR = new OCR();

            m_pathToToken = Environment.CurrentDirectory + Path.DirectorySeparatorChar+
                "XRayToken.txt";

            m_pathToTempPhotos = Environment.CurrentDirectory + Path.DirectorySeparatorChar +
                "Temp" + Path.DirectorySeparatorChar + "Photos";

            m_recOptions = new ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() };

            m_cts = new CancellationTokenSource();

            m_bot = new TelegramBotClient(GetToken());            
        }
        #endregion

        #region Methods

        public string TestBotAsync()
        {
            var info = m_bot.GetMeAsync().Result;

            return $"Hi! I am {info.FirstName} with Id: {info.Id}! I am listening!";
        }

        public void Start()
        {
            m_bot.StartReceiving(
                updateHandler: HandleUpdates,
                pollingErrorHandler: ErrorHandler,
                receiverOptions: m_recOptions,
                cancellationToken: m_cts.Token
                ) ;
        }

        private Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            ////Logging must be done
            ///
            return Task.CompletedTask;
        }

        private async Task HandleUpdates(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
        {
            var type = arg2.Type;

            ChatId chatid = arg2.Message.Chat.Id;

            int msgId = 0;

            try
            {
                switch (type)
                {
                    case UpdateType.Unknown:

                        msgId = arg2.Message.MessageId;

                        await m_bot.SendTextMessageAsync(
                            chatId : chatid,
                            "Невідомий тип <strong>ОБ'ЄКТУ!</strong> Я чекаю від вас фото електронного направлення." +
                            "Використовуйте те що зі штрих кодом будьласка!",
                            replyToMessageId: msgId,
                            parseMode: ParseMode.Html,
                            cancellationToken: m_cts.Token
                            );

                        break;
                    case UpdateType.Message:

                        switch (arg2.Message.Type)
                        {
                            case MessageType.Photo:

                                msgId = arg2.Message.MessageId;

                                await m_bot.SendTextMessageAsync();

                                break;

                            case MessageType.Document:

                                Guid phId = Guid.NewGuid();

                                var photoId = arg2.Message.Document.FileId;

                                var fileInfo = await m_bot.GetFileAsync(photoId);
                                
                                var path = m_pathToTempPhotos + Path.DirectorySeparatorChar +
                                    $"{phId}" + ".jpg";

                                FileStream fs = Create(path);

                                await m_bot.DownloadFileAsync(
                                    filePath: fileInfo.FilePath,
                                    destination: fs,
                                    cancellationToken: m_cts.Token
                                    );

                                fs.Close();

                                fs.Dispose();

                                var r = m_OCR.ConvertPhotoToText(path);
                               
                                break;

                            case MessageType.Text:

                                var text = arg2.Message.Text;

                                switch (text)
                                {
                                    case "start":
                                    case "старт":
                                    case "/start":
                                    case "/старт":

                                        await m_bot.SendTextMessageAsync(
                                            chatId: chatid,
                                            "Привіт <strong>Вам</strong> лікарю. Я программа яка допоможе в " +
                                            "обміні електронними направленнями з вашими колегами " +
                                            "<strong>Рентгеноголами</strong>. Вам потрібно просто переслати мені фото електронного направлення.\n" +
                                            "<i><strong>Але будьте уважні!!!</strong></i> \nНадсилайте фото зі штрих кодом!!!",
                                            parseMode: ParseMode.Html
                                            );

                                        break;


                                }

                                break;
                        }

                        break;

                }
            }
            catch (Exception e)
            {

                throw;
            }

           
        }

        public void Stop()
        {
            m_cts.Cancel();
        }

        private string GetToken()
        {
            string str = String.Empty;

            if (Exists(m_pathToToken))
            {
                str = ReadAllText(m_pathToToken);
            }
            else
            {
                /// Write to log !!! Token wasnt found
            }
            
            return str;
        }

        #endregion

    }
}