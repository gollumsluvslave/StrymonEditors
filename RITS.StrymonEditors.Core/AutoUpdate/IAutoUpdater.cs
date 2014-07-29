using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.AutoUpdate
{
    public interface IAutoUpdater
    {
        bool CheckForUpdate();
        void RunUpdate();
    }
}
