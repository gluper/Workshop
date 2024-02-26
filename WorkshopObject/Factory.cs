using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkshopObject
{
    [TypeGuid("{7061AFE9-40F1-4131-94AD-94A70FC015E1}")]
    public class WorkshopObjectFactory : IObjectFactory
    {
        private readonly WorkshopObjectWizard _wizard = new WorkshopObjectWizard();

        public Guid Namespace => WorkshopObject.POU_NAMESPACE;

        public string Name => Resource.WorkshopObjectName;

        public string Description => Resource.WorkshopObjectDescription;

        public Icon SmallIcon
        {
            get 
            {
                // return Resource.WorkshopObjectSmallIcon not recommended, because the internal implementation will cause handle leak
                return Icons.WorkshopIcon; // a better implementation, because the static Icons class is used
            }
        }

        public Icon LargeIcon => SmallIcon;

        public Control WizardPage => _wizard;

        public Control ObjectNameControl => _wizard.textBox1;

        public string ObjectBaseName => "WORK_SHOP";

        public Type ObjectType => typeof(WorkshopObject);

        public Type[] EmbeddedObjectTypes => null;

        public bool AcceptsParentObject(IObject parentObject)
        {
            return parentObject is IApplicationObject;
        }

        public IObject Create()
        {
            return new WorkshopObject();
        }

        public IObject Create(string[] stBatchArguments)
        {
            return Create();
        }

        public void ObjectCreated(int nProjectHandle, Guid guidObject)
        {
            // do nothing here
        }
    }
}
