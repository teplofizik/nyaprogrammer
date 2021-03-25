using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Params.Types;

namespace Params
{
    public class NotifiedParameterStorage : ParameterStorage
    {
        /// <summary>
        /// Состояние устройство изменился
        /// </summary>
        public event EventHandler onStorageUpdate;

        private bool IsUpdated = false;

        public void checkUpdated()
        {
            if (IsUpdated)
            {
                IsUpdated = false;
                if (onStorageUpdate != null) onStorageUpdate(this, new EventArgs());
            }
        }

        protected override void notifyChanged()
        {
            base.notifyChanged();
            IsUpdated = true;
        }
    }
}
