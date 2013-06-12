using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;
using System.IO;

namespace tenmiktwit
{
    class Program
    {
        static void Main(string[] args)
        {
            //if문시작
            //오버플로방지
            if (args.Length == 0)
            {

            }
            else
            {
                //오리지날 소스코드 = http://garyshortblog.wordpress.com/2011/02/11/a-twitter-oauth-example-in-c/
                //버그수정 = @tensimiku (twitter)
                //오류수정환영합니다
                //args 받음
                string oauthpath = Directory.GetCurrentDirectory() + "//tensi.miku";
                System.IO.StreamReader oauth =
                new System.IO.StreamReader(oauthpath);
                string oauthtoken = oauth.ReadLine();
                oauthtoken = oauthtoken.Replace("oauthtoken=", "");
                string oauthtokenSecret = oauth.ReadLine();
                oauthtokenSecret = oauthtokenSecret.Replace("oauthtokenSecret=", "");
                string Consumerkey = oauth.ReadLine();
                Consumerkey = Consumerkey.Replace("Consumerkey=", "");
                string ConsumerkeySecret = oauth.ReadLine();
                ConsumerkeySecret = ConsumerkeySecret.Replace("ConsumerkeySecret=", "");


                string azurea = args[1];
                //인코딩 안되는건 수동으로 ㅜㅜ
                string escape = azurea.Replace("*", "%2A");
                escape = escape.Replace("(", "%28");
                escape = escape.Replace(")", "%29");
                escape = escape.Replace("!", "%21");
                escape = escape.Replace("'", "%27");


                string status = escape;
                string inreply = args[0];
                string postBody = "status=" + status + "&in_reply_to_status_id=" + inreply + "&include_entities=true";

                string oauth_consumer_key = Consumerkey;
                //args[3]
                string oauth_nonce = Convert.ToBase64String(
                    new ASCIIEncoding().GetBytes(
                        DateTime.Now.Ticks.ToString()));

                string oauth_signature_method = "HMAC-SHA1";
                string oauth_token = oauthtoken;
                //string args[2]

                TimeSpan ts = DateTime.UtcNow -
                    new DateTime(1970, 1, 1, 0, 0, 0, 0);

                string oauth_timestamp =
                    Convert.ToInt64(ts.TotalSeconds).ToString();

                string oauth_version = "1.0";

                //시그니쳐 사인 베이스 스트링을 만들때
                //알파벳순으로 배열되기때문에 SD를 사용
                //이렇게하면. 알아서 해줌
                SortedDictionary<string, string> sd =
                    new SortedDictionary<string, string>();

                sd.Add("status", status);
                sd.Add("oauth_version", oauth_version);
                sd.Add("oauth_consumer_key", oauth_consumer_key);
                sd.Add("oauth_nonce", oauth_nonce);
                sd.Add("oauth_signature_method", oauth_signature_method);
                sd.Add("oauth_timestamp", oauth_timestamp);
                sd.Add("oauth_token", oauth_token);

                //시그니쳐 스트링을 만듬
                string baseString = String.Empty;
                baseString += "POST" + "&";
                baseString += Uri.EscapeDataString(
                    "https://api.twitter.com/1.1/statuses/update.json")
                    + "&";
                baseString += "in_reply_to_status_id%3D" + inreply + "%26"; //inreply 정보 basestring에추가
                baseString += "include_entities%3Dtrue%26";

                foreach (KeyValuePair<string, string> entry in sd)
                {
                    baseString += Uri.EscapeDataString(entry.Key +
                        "=" + entry.Value + "&");
                }

                //status의 끝부분에있는 &를 지움
                //이건 urlencode되있으니 잊지말고 3글자를 지울것!
                baseString =
                    baseString.Substring(0, baseString.Length - 3);

                //사인키를 만듬
                string consumerSecret = ConsumerkeySecret;
                //[5]

                string oauth_token_secret = oauthtokenSecret;
                //[4]

                string signingKey =
                    Uri.EscapeDataString(consumerSecret) + "&" +
                    Uri.EscapeDataString(oauth_token_secret);

                //사인
                HMACSHA1 hasher = new HMACSHA1(
                    new ASCIIEncoding().GetBytes(signingKey));

                string signatureString = Convert.ToBase64String(
                    hasher.ComputeHash(
                    new ASCIIEncoding().GetBytes(baseString)));

                //트위터한테 우리는 뭐안한다고 말해줌
                ServicePointManager.Expect100Continue = false;

                //웹 리퀘스트 초기화.
                //authorization 헤더 만듬
                HttpWebRequest hwr =
                    (HttpWebRequest)WebRequest.Create(
                    @"https://api.twitter.com/1.1/statuses/update.json");

                string authorizationHeaderParams = String.Empty;
                authorizationHeaderParams += "OAuth ";

                authorizationHeaderParams += "oauth_consumer_key=" + "\"" +
                    Uri.EscapeDataString(oauth_consumer_key) + "\",";

                authorizationHeaderParams += "oauth_nonce=" + "\"" +
                    Uri.EscapeDataString(oauth_nonce) + "\",";

                authorizationHeaderParams += "oauth_signature=" + "\""
                    + Uri.EscapeDataString(signatureString) + "\",";

                authorizationHeaderParams +=
                    "oauth_signature_method=" + "\"" +
                    Uri.EscapeDataString(oauth_signature_method) +
                    "\",";

                authorizationHeaderParams += "oauth_timestamp=" + "\"" +
                    Uri.EscapeDataString(oauth_timestamp) + "\",";


                authorizationHeaderParams += "oauth_token=" + "\"" +
                    Uri.EscapeDataString(oauth_token) + "\",";

                authorizationHeaderParams += "oauth_version=" + "\"" +
                    Uri.EscapeDataString(oauth_version) + "\"";

                hwr.Headers.Add(
                    "Authorization", authorizationHeaderParams);

                //postbody를 리퀘
                hwr.Method = "POST";
                hwr.ContentType = "application/x-www-form-urlencoded";
                Stream stream = hwr.GetRequestStream();
                byte[] bodyBytes =
                    new UTF8Encoding().GetBytes(postBody);

                stream.Write(bodyBytes, 0, bodyBytes.Length);
                stream.Flush();
                stream.Close();

                //타임아웃시간
                //넘어가면 서버가 영..
                hwr.Timeout = 3 * 60 * 1000;

                try
                {
                    HttpWebResponse rsp = hwr.GetResponse()
                        as HttpWebResponse;


                    //영 좋은 반응이 왔어요
                }
                catch (WebException e)
                {
                    string errreport = System.IO.Directory.GetCurrentDirectory() + "//err.txt";
                    string reportfile = baseString + "/n";
                    reportfile += postBody + "/n";
                    reportfile += e;
                    reportfile = reportfile.Replace("/n", System.Environment.NewLine);
                    using (FileStream fs = File.Create(errreport))
                    {
                        Byte[] txt = new UTF8Encoding(true).GetBytes(reportfile);
                        fs.Write(txt, 0, txt.Length);
                    }
                    //영 안좋은 일이 일어났어요
                }
            }//else 끝남
        }//Main끝남
    }//Class끝남
}//네임스페이스끝남


