using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public interface IConfigurationManagerHelper
    {
        Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel);
        object GetSection(string sectionName);
    }
}
