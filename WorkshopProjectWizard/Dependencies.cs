using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.ComponentModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.POUObject;
using _3S.CoDeSys.TaskConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopProjectWizard
{
    static class APEnvironment
    {
        private static Lazy<DependencyBag> s_bag = new Lazy<DependencyBag>(() => new DependencyBag());

        public static IEngine3 Engine
        {
            get { return s_bag.Value.EngineProvider.Value; }
        }

        public static IObjectManager ObjectMgr
        {
            get { return s_bag.Value.ObjectManagerProvider.Value; }
        }

        public static IOptionStorage OptionStorage
        {
            get { return s_bag.Value.OptionStorageProvider.Value; }
        }

        public static ILibraryLoader6 LibraryLoader
        {
            get { return s_bag.Value.LibraryLoaderProvider.Value; }
        }

        // more access methods and properties...
        public static IWinFormWrapper FrameForm 
        { 
            get 
            { 
                return s_bag.Value.FrameFormProxy.FrameForm; 
            } 
        }

        public static IObjectFactory CreateDeviceObjectFactory() => s_bag.Value.DeviceObjectFactoryProvider.Create();
        public static IApplicationObject CreateApplicationObject() => s_bag.Value.ApplicationObjectProvider.Create();
        public static IPOUObject CreatePOUObject() => s_bag.Value.POUObjectProvider.Create();
        public static ITaskConfigObject CreateTaskConfigObject() => s_bag.Value.TaskConfigObjectProvider.Create();
        public static ITaskObject CreateTaskObject() => s_bag.Value.TaskObjectProvider.Create();
    }



    class DependencyBag : IDependencyInjectable
    {
        public DependencyBag() { ComponentModel.Singleton.InjectDependencies(this, GetType()); }

        public void InjectionComplete() { }

        [InjectFrameForm] 
        public IWinFormWrapperProxy FrameFormProxy { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IEngine3> EngineProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IObjectManager> ObjectManagerProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IOptionStorage> OptionStorageProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<ILibraryLoader6> LibraryLoaderProvider { get; private set; }

        // more dependencies...
        [InjectSingleInstance(Shared = true)]
        public ISingleInstanceProvider<IObjectFactory> DeviceObjectFactoryProvider { get; private set; }

        public ISingleInstanceProvider<IApplicationObject> ApplicationObjectProvider { get; private set; }

        public ISingleInstanceProvider<IPOUObject> POUObjectProvider { get; private set; }

        public ISingleInstanceProvider<ITaskConfigObject> TaskConfigObjectProvider { get; private set; }

        public ISingleInstanceProvider<ITaskObject> TaskObjectProvider { get; private set; }

    }
}
