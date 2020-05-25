using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetManager.DragDrop
{
    public interface IDragSource
    {
        void StartDrag(DragInfo dragInfo);
    }
}
