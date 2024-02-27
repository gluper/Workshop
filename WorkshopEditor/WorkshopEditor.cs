using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkshopEditor
{
    [TypeGuid("{62A942FC-BF9F-4EBF-89CB-5280DF0763B9}")]
    public partial class WorkshopEditor : UserControl, IEditor, IEditorView
    {
        int _nProjectHandle;
        Guid _guidObject;

        private IMetaObject _metaObject;

        public WorkshopEditor()
        {
            InitializeComponent();
        }

        public int ProjectHandle => _nProjectHandle;

        public Guid ObjectGuid => _guidObject;

        public IMetaObject GetObjectToModify()
        {
            try
            {
                if (_metaObject == null)
                {
                    // we have no metaobject at all. Try to request a modifiable copy
                    _metaObject = APEnvironment.ObjectMgr.GetObjectToModify(_nProjectHandle, _guidObject);
                    return _metaObject;
                }
                else if (!_metaObject.IsToModify)
                {
                    // we have a metaobject, but not for the modification
                    // Try to request a modifiable copy. It will success if no other editor has a modifiable copy to it
                    _metaObject = APEnvironment.ObjectMgr.GetObjectToModify(_nProjectHandle, _guidObject);
                    return _metaObject;
                }
                else
                    return _metaObject;
            }
            catch 
            {
                // some error happens, probably some other editor has a modifiable copy
                return null;
            }
        }

        public IMetaObject GetObjectToRead()
        {
            if (_metaObject == null)
                _metaObject = APEnvironment.ObjectMgr.GetObjectToRead( _nProjectHandle, _guidObject);
            return _metaObject;
        }

        public void Reload()
        {
            // this is initial call or
            // some other editor has changed our object. We need to invalidate our copy.
            _metaObject = null;
        }

        public void Save(bool bCommit)
        {
            if (_metaObject != null && _metaObject.IsToModify)
            {
                // write the changes made back to the database
                APEnvironment.ObjectMgr.SetObject(_metaObject, bCommit, this);
                _metaObject = null;
            }
        }

        public void SetObject(int nProjectHandle, Guid objectGuid)
        {
            _nProjectHandle = nProjectHandle;
            _guidObject = objectGuid;
            _metaObject = null;
        }

        // ------- 
        // IEditorView methods

        public IEditor Editor => this;

        public Control Control => this;

        public Control[] Panes => null; 

        public string Caption
        {
            get
            {
                IMetaObjectStub mos = APEnvironment.ObjectMgr.GetMetaObjectStub(_nProjectHandle, _guidObject);
                return mos != null ? mos.Name : "???";
            }
        }

        public string Description
        {
            get
            {
                return APEnvironment.ObjectMgr.GetFullName(_nProjectHandle, _guidObject);
            }
        }

        public Icon SmallIcon => Icons.WorkshopIcon;

        public Icon LargeIcon => SmallIcon;

        public DockingPosition DefaultDockingPosition => DockingPosition.MDIChild;

        public DockingPosition PossibleDockingPositions => DockingPosition.MDIChild;

        public bool CanExecuteStandardCommand(Guid commandGuid)
        {
            return false;
        }

        public void ExecuteStandardCommand(Guid commandGuid)
        {
            // TODO
            // standard command are : copy, paste, cut, undo, redo, delete, select all
        }

        public int ComparePositions(long nPosition1, long nPosition2)
        {
			long nPositionId1;
			long nPositionId2;
			short nPositionOffset1;
			short nPositionOffset2;
            // Position in AP (64) is combination of PositionID(48) and PositionOffset(16)
			PositionHelper.SplitPosition(nPosition1, out nPositionId1, out nPositionOffset1);
			PositionHelper.SplitPosition(nPosition2, out nPositionId2, out nPositionOffset2);

            return nPositionOffset1 - nPositionOffset2;
        }

        public void GetSelection(out long nPosition, out int nLength)
        {
            nPosition = PositionHelper.CombinePosition(nPositionId: 0, (short)_textBox.SelectionStart);
            nLength = _textBox.SelectionLength; 
        }

        public void Select(long nPosition, int nLength)
        {
            PositionHelper.SplitPosition(nPosition, out long nPositionId, out short nPositionOffset);
            try
            {
                _textBox.Select(nPositionOffset, nLength);
            }
            catch
            { 
                // do nothing
            }
        }

        public void Mark(long nPosition, int nLength, object tag)
        {
        }


        public void UnmarkAll(object tag)
        {
        }
    }
}
