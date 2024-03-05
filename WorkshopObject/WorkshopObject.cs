using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
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
    [StorageVersion("1.0.0.0")]
    public class WorkshopObject : GenericObject2, IWorkshopObject2, IFindReplace
    {
        public static readonly Guid POU_NAMESPACE = new Guid("{E5B60C93-5445-4e40-ADA9-CD9C005549B4}");
        private string _stText;
        private string _stDescription;
        IUniqueIdGenerator _idGen = new LocalIdGenerator();
        private IMetaObject _metaObject;

        [DefaultSerialization("Text"), StorageVersion("1.0.0.0")]
        [DefaultDuplication(DuplicationMethod.Deep)]
        [StorageDefaultValue(null)]
        public string Text 
        {
            get { return _stText; }
            set { _stText = value; }
        }

        [DefaultSerialization("Description"), StorageVersion("1.1.0.0")]
        [DefaultDuplication(DuplicationMethod.Deep)]
        [StorageDefaultValue(null)]
        public string Description 
        { 
            get { return _stDescription; }
            set { _stDescription = value; }
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
            PositionHelper.SplitPosition(nPosition, out long nPositionId, out short nPositionOffset);
            if (nPositionId == 0 && _stText?.Length >= nPositionOffset + nLength)
                return _stText.Substring(nPositionOffset, nLength);
            else if (_stDescription?.Length >= nPositionOffset + nLength)
                return _stDescription.Substring(nPositionOffset, nLength);
            else
                return string.Empty;
        }

        public string GetPositionText(long nPosition)
        {
            PositionHelper.SplitPosition(nPosition, out long nPositionId, out short nPositionOffset);
            if (nPositionId == 0)
                return "Field 'Text'";
            else if (nPositionId == 1)
                return "Field 'Description'";
            else
                return "Unknown position";
        }

        public void HandleRenamed()
        {
        }

        public SearchableTextBlock[] GetSearchableTextBlocks()
        {
            SearchableTextBlock[] blocks = new SearchableTextBlock[2];
            blocks[0].PositionIds = new long[] { 0 };
            blocks[0].PositionOffsets = new int[] { 0 };
            blocks[0].Text = _stText;

            blocks[1].PositionIds = new long[] { 1 };
            blocks[1].PositionOffsets = new int[] { 0 };
            blocks[1].Text = _stDescription;

            return blocks;
        }

        public void Replace(long nPosition, int nLength, string stReplacement)
        {
            PositionHelper.SplitPosition(nPosition, out long nPositionId, out short nPositionOffset);

            if (nPositionId == 0)
            {
                // relates to the "text"
                _stText = _stText.Remove(nPositionOffset, nLength);
                _stText = _stText.Insert(nPositionOffset, stReplacement);
            }
            else if (nPositionId == 1)
            {
                // relates to the "description"
                _stDescription = _stDescription.Remove(nPositionOffset, nLength);
                _stDescription = _stDescription.Insert(nPositionOffset, stReplacement);
            }
        }

        public void AddDetails(string st, int v)
        {
            // add implementation
        }

        public void AddDetails(string st, int v, int x)
        {
            AddDetails(st, v);
            
            // ....

        }

        public IUniqueIdGenerator UniqueIdGenerator => _idGen;


        class LocalIdGenerator : DefaultUniqueIdGenerator, IUniqueIdGenerator { }
    }

}
