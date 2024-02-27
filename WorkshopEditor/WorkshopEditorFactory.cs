using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopObject;

namespace WorkshopEditor
{
    [TypeGuid("{B75AB372-F1C1-494D-925B-90C0CC7A74BC}")]
    public class WorkshopEditorFactory : IModelessEditorViewFactory
    {
        public string Name => Resource.WorkshopEditorName;

        public string Description => Resource.WorkshopEditorDescription;

        public Icon SmallIcon => Icons.WorkshopIcon;

        public Icon LargeIcon => SmallIcon;

        public bool AcceptsObjectType(Type objectType, Type[] embeddedObjectTypes)
        {
            return typeof(IWorkshopObject).IsAssignableFrom(objectType);
        }

        public IEditorView Create()
        {
            return new WorkshopEditor();
        }
    }
}
