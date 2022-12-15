using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace EncodingDecodingApi.Filter
{
    
    /// <summary>
    /// Created By Ahmed
    /// </summary>
    public class EncodeDecoreMiddleware
    {
        private RequestDelegate next;
        bool encodeEnable = true;

        public EncodeDecoreMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            context.Request.ContentType= "application/json";
            Console.WriteLine("LoggingMiddleware invoked.");
            
            var stream = request.Body;// currently holds the original stream                    
            var originalContent = await new StreamReader(stream).ReadToEndAsync();
            var notModified = true;
            try
            {
                    string Reqtemp = encodeEnable ? Base64.Base64Decode(originalContent) : originalContent;
                    //replace request stream to downstream handlers
                    var requestContent = new StringContent(Reqtemp, Encoding.UTF8, "application/json");
                    stream = await requestContent.ReadAsStreamAsync();//modified stream
                    notModified = encodeEnable ? false :true;
            }
            catch
            {
                //No-op or log error
            }
            if (notModified)
            {
                //put original data back for the downstream to read
                var requestData = Encoding.UTF8.GetBytes(originalContent);
                stream = new MemoryStream(requestData);
            }

            request.Body = stream;

            var originalBodyStream = context.Response.Body;

            // Create new memory stream for reading the response; Response body streams are write-only, therefore memory stream is needed here to read
            await using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;
            try
            {
                await this.next(context);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            // Set stream pointer position to 0 before reading
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Read the body from the stream
            var ResoriginalContent = await new StreamReader(memoryStream).ReadToEndAsync();

            // Reset the position to 0 after reading
            memoryStream.Seek(0, SeekOrigin.Begin);
             
            string Restemp = encodeEnable ? Base64.Base64Encode(ResoriginalContent) : ResoriginalContent;
            Dictionary<string, string> resp = new Dictionary<string, string>();
            resp.Add("data", Restemp);
            var jsonString = JsonConvert.SerializeObject(Restemp);
 
            await context.Response.WriteAsync(jsonString);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
 
            await context.Response.Body.CopyToAsync(originalBodyStream); //it prevents it must be async, if it isn't there is an exception in .Net 6.
            context.Response.Body = originalBodyStream;

        }
         
    }
}
