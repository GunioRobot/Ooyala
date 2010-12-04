using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Ooyala
{
    public class BacklotApi
    {
        public IDictionary<string, string> Parameters { get; private set; }

        private readonly string _secretCode;
        private readonly string _apiUrl;
        private readonly string _partnerCode;

        public BacklotApi(string apiUrl, string secretCode, string partnerCode)
        {
            this._apiUrl = apiUrl;
            this._partnerCode = partnerCode;
            this._secretCode = secretCode;
            this.Parameters = new Dictionary<string, string>();
        }

        private static string Sha256(string s) //Shy Ronnie
        {
            var sh = new SHA256Managed();
            var request = Encoding.UTF8.GetBytes(s);
            sh.Initialize();
            var b4Bbuff = sh.ComputeHash(request, 0, request.Length);
            var b64 = Convert.ToBase64String(b4Bbuff);
            return b64.Substring(0, 43);
        }

        public string BuildRequest()
        {
            var request = new StringBuilder(this._secretCode);
            request.Append(GetParametersAsString(null));

            var base64EncodedString = Sha256(request.ToString());
            var url = string.Format("{0}?pcode={1}&{2}&signature={3}", this._apiUrl, this._partnerCode, GetParametersAsString("&"), HttpUtility.UrlEncode(base64EncodedString));

            return url;
        }

        private string GetParametersAsString(string delimiter)
        {
            var request = new StringBuilder();
            var sortedDict = (from entry in this.Parameters orderby entry.ToString() ascending select entry);

            foreach (var parameter in sortedDict)
            {
                request.Append(parameter.Key).Append("=").Append(parameter.Value);
                if (!string.IsNullOrEmpty(delimiter))
                    request.Append(delimiter);
            }

            var returnValue = request.ToString();
            if (returnValue.EndsWith("&"))
                returnValue = returnValue.Substring(0, returnValue.Length - 1);
            return returnValue;
        }
    }
}