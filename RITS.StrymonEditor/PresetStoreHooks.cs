using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RITS.StrymonEditor
{
    public static class PresetStoreHooks
    {
        private static StrymonPresetStoreClient.StrymonStoreClient _client;
        public static StrymonPresetStoreClient.StrymonStoreClient OnlineClient
        {
            get
            {
                if (_client == null) _client = new StrymonPresetStoreClient.StrymonStoreClient(Properties.Settings.Default.OnlineService);
                return _client;
            }
        }

    }
}
