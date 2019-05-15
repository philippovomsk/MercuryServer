using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MercuryServer
{
    class HttpWorker
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HttpWorker(TcpClient client)
        {

            NetworkStream stream = client.GetStream();

            StringBuilder request = new StringBuilder();
            byte[] buffer = new byte[1024];

            int numberOfBytesRead = 0;

            do
            {
                numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);
                request.AppendFormat("{0}", Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead));
            }
            while (stream.DataAvailable);

            log.Debug("Получен запрос: " + request.ToString());

            String requestString = request.ToString();
            int bodypos = requestString.IndexOf("\r\n\r\n");
            string body = "";
            if (bodypos > 0)
            {
                body = requestString.Substring(bodypos + 4);
            }

            JObject requestJson = JObject.Parse(body);

            //(string)requestParams["Чек"]

            JObject responseJson = new JObject();
            responseJson["error"] = "";
            responseJson["Продавец"] = (string)requestJson["Продавец"];

            string Html = responseJson.ToString();

            byte[] htmlbuf = Encoding.UTF8.GetBytes(Html);

            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + htmlbuf.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.UTF8.GetBytes(Str);
            // Отправим его клиенту
            client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            client.Close();
        }
    }
}
