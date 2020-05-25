using System.Linq;
using System.Windows;

namespace AssetManager.DragDrop
{
    public class DefaultDragHandler : IDragSource
    {
        public virtual void StartDrag(DragInfo dragInfo)
        {
            int itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            }
            //else if (itemCount > 1)
            //{
            //    dragInfo.Data = AssetManager.DragDrop.Utilities.TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            //}

            dragInfo.Effects = (dragInfo.Data != null) ? 
                DragDropEffects.Copy | DragDropEffects.Move : 
                DragDropEffects.None;
        }
    }
}
