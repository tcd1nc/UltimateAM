using System;

namespace AssetManager.DragDrop
{
    interface IDragable
    {
        /// <summary>
        /// Type of the data item
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// Remove the object from the collection
        /// </summary>
        void Remove(object i);
    }
}
