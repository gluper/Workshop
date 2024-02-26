﻿using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopObject
{
    [TypeGuid("{BB3037F3-770B-48F3-86CF-79B50430B433}")]
    public class WorkshopObject : GenericObject2, IWorkshopObject
    {
        public static readonly Guid POU_NAMESPACE = new Guid("{E5B60C93-5445-4e40-ADA9-CD9C005549B4}");
        private string _stText;
        IUniqueIdGenerator _idGen = new LocalIdGenerator();
        private IMetaObject _metaObject;


        public string Text 
        {
            get { return _stText; }
            set { _stText = value; }
        }

        public IMetaObject MetaObject 
        {
            get { return _metaObject; }
            set { _metaObject = value; }
        }

        public Guid Namespace => POU_NAMESPACE;

        public bool CanRename => true;

        public IEmbeddedObject[] EmbeddedObjects => null;


        public bool AcceptsChildObject(Type childObjectType)
        {
            return true; // all kind of objects are allowed as children…
        }

        public bool AcceptsParentObject(IObject parentObject)
        {
            return parentObject is IApplicationObject;
        }

        public bool CheckName(string stName)
        {
            return !string.IsNullOrEmpty(stName);
        }

        public int CheckRelationships(IObject parentObject, IObject[] childObjects)
        {
            // allowed only under Application
            if (!(parentObject is IApplicationObject))
                return -1;

            if (childObjects?.Length >= 0)
                return 1;

            return 0;
        }

        public string GetContentString(ref long nPosition, ref int nLength, bool bWord)
        {
            return "TODO"; // something like "request, and to save its state persistently." (content)
        }

        public string GetPositionText(long nPosition)
        {
            return "TODO"; // should return something like "page 40, line 2" (position as text)
        }

        public void HandleRenamed()
        {
        }

        public IUniqueIdGenerator UniqueIdGenerator => _idGen;

        class LocalIdGenerator : DefaultUniqueIdGenerator, IUniqueIdGenerator { }
    }

}
