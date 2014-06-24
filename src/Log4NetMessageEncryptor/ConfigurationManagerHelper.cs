using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public class ConfigurationManagerHelper: IConfigurationManagerHelper
    {
        public Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
        {
            return ConfigurationManager.OpenExeConfiguration(userLevel);
        }
    }
}
