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

        /// <summary>
        /// Version Number as integer
        /// </summary>
        [XmlIgnore]
        public int VersionNo
        {
            get { return Convert.ToInt32(_version.Replace(".", "")); }
        }

        /// <summary>
        /// The url that points to the <see cref="VersionConfig"/> file that is used to determine a new version is available
        /// </summary>
        public string UpdateCheckTarget
        {
            get { return _updateCheckTarget; }
            set { _updateCheckTarget = value; }
        }

        /// <summary>
        /// The http url where the zip of the version of the application is to be found
        /// </summary>
        public string UpdateZipTarget
        {
            get { return _updateZipTarget; }
            set { _updateZipTarget = value; }
        }

    }
}
