﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using ReimuAPI.ReimuBase;

namespace CNBlackListSoamChecker
{
    internal class CNBlacklistApi
    {
        private static string ApiURL;
        private static string ApiKey;

        internal static void PostToAPI(int uid, bool ban, int level, long expires, string reason = null)
        {
            try
            {
                string configPath;
                if (ApiURL == null)
                {
                    configPath = Environment.GetEnvironmentVariable("CNBLACKLIST_CONFIGPATH");
                    if (configPath == "" || configPath == null) configPath = "./plugincfg/soamchecker/api.json";
                    string configContent = File.ReadAllText(configPath);
                    PrivateApiConfig data = (PrivateApiConfig) new DataContractJsonSerializer(
                        typeof(PrivateApiConfig)
                    ).ReadObject(
                        new MemoryStream(
                            Encoding.UTF8.GetBytes(configContent)
                        )
                    );
                    ApiURL = data.ApiURL;
                    ApiKey = data.ApiKey;
                }
            }
            catch (Exception)
            {
                return;
            }

            string realBan;
            if (ban)
                realBan = "true";
            else
                realBan = "false";
            if (reason != null)
                reason = "&reason=" + reason;
            else
                reason = "";
            try
            {
                string resultMsg = TgApi.getDefaultApiConnection().postWeb(
                    ApiURL,
                    "method=set_value&apikey=" + ApiKey +
                    "&uid=" + uid +
                    "&ban=" + realBan +
                    "&level=" + level +
                    "&expires=" + expires + reason
                ).Content;
                if (resultMsg.IndexOf("\"ok\": false") != -1)
                    throw new Exception("API result = false:\n\n" + resultMsg);
            }
            catch (Exception e)
            {
                RAPI.GetExceptionListener().OnException(e);
            }
        }
    }

    public class PrivateApiConfig
    {
        public string ApiKey { get; set; }
        public string ApiURL { get; set; }
    }
}