using adoProcess.Models;
using Microsoft.VisualStudio.Services.Organization.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adoProcess.Helper
{
    public class ConfigHelper
    {
        private string _organization = "";
        private string _personalAccessToken = "";

        public ConfigHelper()
        {
            Init();
        }

        private void Init()
        {
            string jsonFilePath = @"config.json";
            
            try
            {
                Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(jsonFilePath));

                this._organization = config.organization;
                this._personalAccessToken = config.personalaccesstoken;
            }
            catch(Exception)
            {
                this._organization = string.Empty;
                this._personalAccessToken = string.Empty;
            }
        }

        public string Organization { 
            get { return _organization; } 
        }
        
        public string PersonalAccessToken {  
            get { return _personalAccessToken; } 
        }
      
    }
}
