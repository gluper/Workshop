using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.ImplementationObject;
using _3S.CoDeSys.DeviceObject;
using System.Diagnostics;

namespace WorkshopProjectWizard
{
    public partial class WizardForm : Form
    {
        private static readonly Guid GUID_STIMPLEMENTATIONOBJECTFACTORY = new Guid("{CC393387-A21C-4f68-A3E3-84C36951965D}");
        private static readonly Guid GUID_PLCFILTER = new Guid("{03C2B5AE-3177-4457-8F3F-24B86BB94F8B}");

        public WizardForm()
        {
            InitializeComponent();
        }

        internal IDeviceDescription DeviceDescription
        {
            get
            {
                DeviceDescriptionEntry entry = _deviceComboBox.SelectedItem as DeviceDescriptionEntry;
                return entry != null ? entry.DeviceDescription : null;
            }
        }

        internal IImplementationObjectFactory ImplementationObjectFactory
        {
            get
            {
                ImplObjectFactoryEntry entry = _languageComboBox.SelectedItem as ImplObjectFactoryEntry;
                return entry != null ? entry.Factory : null;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IDeviceRepository deviceRepository = (IDeviceRepository)ComponentManager.Singleton.InstanceFactory.GetSystemInstance(typeof(IDeviceRepository).FullName);
            Debug.Assert(deviceRepository != null);
            Dictionary<DeviceIdAndType, IDeviceDescription> newestVersions = new Dictionary<DeviceIdAndType, IDeviceDescription>();
            IDeviceCatalogueFilter filter;
            try
            {
                filter = (IDeviceCatalogueFilter)ComponentManager.Singleton.CreateInstance(GUID_PLCFILTER);
            }
            catch
            {
                filter = null;
            }
            foreach (IDeviceDescription deviceDescription in deviceRepository.GetAllDevices())
            {
                if (filter != null)
                {
                    if (!filter.Match(deviceDescription))
                        continue;
                }
                else
                {
                    // Fallback if the PLCFilter is not available. Does not find all programmable
                    // devices.
                    if (!deviceDescription.AllowTopLevel)
                        continue;
                    if (deviceDescription.HideInCatalogue)
                        continue;
                    if (deviceDescription.DeviceIdentification.Type != 4096)
                        continue;
                }

                DeviceIdAndType idAndType = new DeviceIdAndType(deviceDescription.DeviceIdentification.Id, deviceDescription.DeviceIdentification.Type);
                if (newestVersions.ContainsKey(idAndType))
                {
                    try
                    {
                        IDeviceDescription existingDeviceDescription = newestVersions[idAndType];
                        Version v1 = new Version(deviceDescription.DeviceIdentification.Version);
                        Version v2 = new Version(existingDeviceDescription.DeviceIdentification.Version);
                        if (v1 > v2)
                            newestVersions[idAndType] = deviceDescription;
                    }
                    catch { }
                }
                else
                    newestVersions[idAndType] = deviceDescription;
            }
            DeviceDescriptionEntry deviceIdToSelect = null;
            foreach (IDeviceDescription deviceDescription in newestVersions.Values)
            {
                DeviceDescriptionEntry entry = new DeviceDescriptionEntry(deviceDescription);
                _deviceComboBox.Items.Add(entry);
                if (entry.DeviceDescription.DeviceIdentification.Id == OptionsHelper.DeviceId && entry.DeviceDescription.DeviceIdentification.Type == OptionsHelper.DeviceType)
                    deviceIdToSelect = entry;
            }
            if (_deviceComboBox.Items.Count > 0)
                if (deviceIdToSelect != null)
                    _deviceComboBox.SelectedItem = deviceIdToSelect;
                else
                    _deviceComboBox.SelectedIndex = 0;

            ImplObjectFactoryEntry implObjectToSelect = null;
            foreach(Guid typeGuid in ComponentManager.Singleton.PlugInCache.GetTypesFast(typeof(IImplementationObjectFactory).FullName, null))
                try
                {
                    IImplementationObjectFactory factory = (IImplementationObjectFactory)ComponentManager.Singleton.CreateInstance(typeGuid);
                    ImplObjectFactoryEntry entry = new ImplObjectFactoryEntry(typeGuid, factory);
                    _languageComboBox.Items.Add(entry);
                    if (OptionsHelper.ImplementationObjectFactory == Guid.Empty && typeGuid == GUID_STIMPLEMENTATIONOBJECTFACTORY || typeGuid == OptionsHelper.ImplementationObjectFactory)
                        implObjectToSelect = entry;
                }
                catch { }
            if (_languageComboBox.Items.Count > 0)
                if (implObjectToSelect != null)
                    _languageComboBox.SelectedItem = implObjectToSelect;
                else
                    _languageComboBox.SelectedIndex = 0;

            UpdateControlStates();
        }

        private void UpdateControlStates()
        {
            _okButton.Enabled = _deviceComboBox.SelectedItem != null && _languageComboBox.SelectedItem != null;
        }

        private void _deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void _languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            DeviceDescriptionEntry entry1 = _deviceComboBox.SelectedItem as DeviceDescriptionEntry;
            if (entry1 != null)
            {
                OptionsHelper.DeviceId = entry1.DeviceDescription.DeviceIdentification.Id;
                OptionsHelper.DeviceType = entry1.DeviceDescription.DeviceIdentification.Type;
            }

            ImplObjectFactoryEntry entry2 = _languageComboBox.SelectedItem as ImplObjectFactoryEntry;
            if (entry2 != null)
                OptionsHelper.ImplementationObjectFactory = entry2.FactoryGuid;
        }
    }

    class ImplObjectFactoryEntry
    {
        private Guid _factoryGuid;
        private IImplementationObjectFactory _factory;

        public ImplObjectFactoryEntry(Guid factoryGuid, IImplementationObjectFactory factory)
        {
            _factoryGuid = factoryGuid;
            _factory = factory;
        }

        public Guid FactoryGuid
        {
            get { return _factoryGuid; }

        }

        public IImplementationObjectFactory Factory
        {
            get { return _factory; }
        }

        public override string ToString()
        {
            return _factory.Name;
        }
    }

    class DeviceDescriptionEntry
    {
        private IDeviceDescription _deviceDescription;

        public DeviceDescriptionEntry(IDeviceDescription deviceDescription)
        {
            _deviceDescription = deviceDescription;
        }

        public IDeviceDescription DeviceDescription
        {
            get { return _deviceDescription; }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", _deviceDescription.DeviceInfo.Name, _deviceDescription.DeviceInfo.Vendor);
        }
    }

    class DeviceIdAndType
    {
        private string _stId;
        private int _nType;

        internal DeviceIdAndType(string stId, int nType)
        {
            _stId = stId;
            _nType = nType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DeviceIdAndType other = (DeviceIdAndType)obj;
            return _stId == other._stId && _nType == other._nType;
        }

        public override int GetHashCode()
        {
            int nHashCode = _nType.GetHashCode();
            if (_stId != null)
                nHashCode ^= _stId.GetHashCode();
            return nHashCode;
        }
    }
}