using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Params.Types;

namespace Params
{
    public class SelfNotifiedParameterStorage : NotifiedParameterStorage
    {
        /// <summary>
        /// Известить об изменениях
        /// </summary>
        protected override void notifyChanged()
        {
            base.notifyChanged();
            checkUpdated();
        }
    }
}
