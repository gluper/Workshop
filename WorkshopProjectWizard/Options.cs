using System;
using System.Collections.Generic;
using System.Text;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core;

namespace WorkshopProjectWizard
{
    static class OptionsHelper
    {
        private static readonly string DEVICE_ID = "DeviceId";
        private static readonly string DEVICE_TYPE = "DeviceType";
        private static readonly string IMPLEMENTATION_OBJECT_FACTORY = "ImplementationObjectFactory";
        private static readonly string SUB_KEY = "{7C54FBAA-C522-4e75-AAF6-AD2388F2BABC}";

        public static string DeviceId
        {
            get
            {
                if (OptionKey.HasValue(DEVICE_ID, typeof(string)))
                    return (string)OptionKey[DEVICE_ID];
                else
                    return null;
            }

            set { OptionKey[DEVICE_ID] = value; }
        }

        public static int DeviceType
        {
            get
            {
                if (OptionKey.HasValue(DEVICE_TYPE, typeof(int)))
                    return (int)OptionKey[DEVICE_TYPE];
                else
                    return 0;
            }

            set { OptionKey[DEVICE_TYPE] = value; }
        }

        public static Guid ImplementationObjectFactory
        {
            get
            {
                if (OptionKey.HasValue(IMPLEMENTATION_OBJECT_FACTORY, typeof(Guid)))
                    return (Guid)OptionKey[IMPLEMENTATION_OBJECT_FACTORY];
                else
                    return Guid.Empty;
            }

            set { OptionKey[IMPLEMENTATION_OBJECT_FACTORY] = value; }
        }

        private static IOptionKey OptionKey
        {
            get { return APEnvironment.OptionStorage.GetRootKey(OptionRoot.User).CreateSubKey(SUB_KEY); }
        }
    }
}
