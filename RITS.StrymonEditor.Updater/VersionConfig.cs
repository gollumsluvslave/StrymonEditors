using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace RITS.StrymonEditor.AutoUpdate
{
    /// <summary>
    /// Version information for the Editor to support auto updates
    /// </summary>
    [Serializable]
    public class VersionConfig
    {
        private string _applicationName;
        private string _version;
        private string _updateCheckTarget;
        private string _updateZipTarget;

        /// <summary>
        /// The name of the application
        /// </summary>
        [XmlIgnore]
        public string ApplicationName
        {
            get { return "RITS.StrymonEditor"; }
            set {  }
        }

        /// <summary>
        /// The version of the application
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        [XmlIgnore]
        public int VersionNo
        {
            get { return Convert.ToInt32(_version.Replace(".", "")); }
        }

        /// <summary>
        /// The root url where all components of the application is stored
        /// </summary>
        public string UpdateCheckTarget
        {
            get { return _updateCheckTarget; }
            set { _updateCheckTarget = value; }
        }

        /// <summary>
        /// The root url where all components of the application is stored
        /// </summary>
        public string UpdateZipTarget
        {
            get { return _updateZipTarget; }
            set { _updateZipTarget = value; }
        }

    }
}
