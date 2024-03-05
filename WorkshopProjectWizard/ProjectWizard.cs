using System;
using System.Collections.Generic;
using System.Text;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using System.Windows.Forms;
using _3S.CoDeSys.DeviceObject;
using System.Diagnostics;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.POUObject;
using _3S.CoDeSys.ImplementationObject;
using _3S.CoDeSys.VarDeclObject;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.LibManObject;
using System.IO;
using WorkshopProjectWizard;
using WorkshopProjectWizard.Properties;
using WorkshopObject;

namespace WorkshopProjectWizard
{
    [TypeGuid("{DF648219-6926-46F3-9272-3C34E3D1917E}")]
    public class ProjectWizard : IProjectWizard
    {
		private static readonly Guid GUID_TASKOBJECTFACTORY = new Guid("{EBBE0F73-C9FF-42e3-9DF9-113EF094F267}");

        public IProject Execute(string stProjectName, string stProjectLocation)
        {
            IProject project = APEnvironment.Engine.Projects.CreateProject(stProjectLocation, stProjectName, ProjectAttributes.Primary, ProjectAttributes.ProvidesLanguageModel);
            if (project == null)
                return null;
            if (File.Exists(project.Path) && APEnvironment.Engine.MessageService.Prompt(Resources.ProjectAlreadyExists, PromptChoice.YesNo, PromptResult.No) != PromptResult.Yes)
            {
                project.Close();
                return null;
            }

            WizardForm form = new WizardForm();
            if (form.ShowDialog(APEnvironment.FrameForm) != DialogResult.OK)
            {
                project.Close();
                return null;
            }
            if (form.DeviceDescription == null)
            {
                project.Close();
                return null;
            }
            if (form.ImplementationObjectFactory == null)
            {
                project.Close();
                return null;
            }

            IUndoManager undoMgr = APEnvironment.ObjectMgr.GetUndoManager(project.Handle);
            Debug.Assert(undoMgr != null);
            try
            {
                undoMgr.BeginCompoundAction(string.Empty);  // TODO

                // Create the device.

                IObjectFactory deviceObjectFactory = APEnvironment.CreateDeviceObjectFactory();
                Debug.Assert(deviceObjectFactory != null);
                IDeviceIdentification deviceId = form.DeviceDescription.DeviceIdentification;
                IDeviceObject deviceObject;
                if (deviceId is IModuleIdentification)
                    deviceObject = (IDeviceObject)deviceObjectFactory.Create(new string[] { deviceId.Type.ToString(), deviceId.Id, deviceId.Version, ((IModuleIdentification)deviceId).ModuleId });
                else
                    deviceObject = (IDeviceObject)deviceObjectFactory.Create(new string[] { deviceId.Type.ToString(), deviceId.Id, deviceId.Version });
                Guid deviceObjectGuid = APEnvironment.ObjectMgr.AddObject(project.Handle, Guid.Empty, Guid.NewGuid(), deviceObject, "Device", -1);
                deviceObjectFactory.ObjectCreated(project.Handle, deviceObjectGuid);

                // Check whether there is a PLCLogic node below the device. If not, notify the user
                // that the wizard could not complete successfully (but return the project created so
                // far).

                Guid plcLogicObjectGuid = Guid.Empty;
                foreach (Guid subObjectGuid in APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, deviceObjectGuid).SubObjectGuids)
                    if (typeof(IPlcLogicObject).IsAssignableFrom(APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, subObjectGuid).ObjectType))
                    {
                        plcLogicObjectGuid = subObjectGuid;
                        break;
                    }
                if (plcLogicObjectGuid == Guid.Empty)
                {
                    APEnvironment.Engine.MessageService.Warning(Resources.DeviceTypeNotProgrammable);
                    return project;
                }

                // Check whether there is an application node below the PLCLogic. If not, create one.

                Guid applicationObjectGuid = Guid.Empty;
                foreach (Guid subObjectGuid in APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, plcLogicObjectGuid).SubObjectGuids)
                    if (typeof(IApplicationObject).IsAssignableFrom(APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, subObjectGuid).ObjectType))
                    {
                        applicationObjectGuid = subObjectGuid;
                        break;
                    }
                if (applicationObjectGuid == Guid.Empty)
                {
                    IObject applicationObject = APEnvironment.CreateApplicationObject();
                    applicationObjectGuid = APEnvironment.ObjectMgr.AddObject(project.Handle, plcLogicObjectGuid, Guid.NewGuid(), applicationObject, "Application", -1);
                }
                project.ActiveApplication = applicationObjectGuid;

                // Check whether there is a POU called PLC_PRG below the application. If not, create
                // one.

                Guid plcprgObjectGuid = Guid.Empty;
                foreach (Guid subObjectGuid in APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, applicationObjectGuid).SubObjectGuids)
                {
                    IMetaObjectStub mos = APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, subObjectGuid);
                    if (mos.Name == "PLC_PRG" && typeof(IPOUObject).IsAssignableFrom(mos.ObjectType))
                    {
                        plcprgObjectGuid = subObjectGuid;
                        break;
                    }
                }
                if (plcprgObjectGuid == Guid.Empty)
                {
                    IPOUObject plcprgObject = APEnvironment.CreatePOUObject();
                    ((ITextVarDeclObject)plcprgObject.Interface).TextDocument.Text = "PROGRAM PLC_PRG\r\nVAR\r\nEND_VAR";
                    plcprgObject.Implementation = form.ImplementationObjectFactory.Create();
                    plcprgObjectGuid = APEnvironment.ObjectMgr.AddObject(project.Handle, applicationObjectGuid, Guid.NewGuid(), plcprgObject, "PLC_PRG", -1);
                }

                // Check whether there is a task configuration below the application. If not, create
                // one.

                Guid taskConfigObjectGuid = Guid.Empty;
                foreach (Guid subObjectGuid in APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, applicationObjectGuid).SubObjectGuids)
                    if (typeof(ITaskConfigObject).IsAssignableFrom(APEnvironment.ObjectMgr.GetMetaObjectStub(project.Handle, subObjectGuid).ObjectType))
                    {
                        taskConfigObjectGuid = subObjectGuid;
                        break;
                    }
                if (taskConfigObjectGuid == Guid.Empty)
                {
                    IObject taskConfigObject = APEnvironment.CreateTaskConfigObject();
                    taskConfigObjectGuid = APEnvironment.ObjectMgr.AddObject(project.Handle, applicationObjectGuid, Guid.NewGuid(), taskConfigObject, "Task Configuration", -1);
                }

                // Add a cyclic task for PLC_PRG into the task configuration.
                ITaskObject taskObject = APEnvironment.CreateTaskObject();
                taskObject.Interval = "T#200ms";
                taskObject.KindOf = KindOfTask.Cyclic;
                taskObject.POUs.Add(taskObject.CreatePOU("PLC_PRG"));
                Guid guidTask = APEnvironment.ObjectMgr.AddObject(project.Handle, taskConfigObjectGuid, Guid.NewGuid(), taskObject, "MainTask", -1);
				IObjectFactory taskObjectFactory = APEnvironment.ObjectMgr.ObjectFactoryManager.GetFactory(GUID_TASKOBJECTFACTORY);
				taskObjectFactory.ObjectCreated(project.Handle, guidTask);

                // Add a reference to the currently newest version of Standard.library into the
                // project. If there is not Standard.library present, report to the user.

                // Add a reference to the currently newest version of Standard.library into the
                // project. If there is not Standard.library present, report to the user.
                APEnvironment.LibraryLoader.LoadPlaceholderLibrary("Standard", "Standard, * (System)", project.Handle, applicationObjectGuid, Guid.Empty, false);

                IWorkshopObject2 workshopObject = APEnvironment.CreateWorkshopObject();

                workshopObject.Text = "This is a text for autocreated object";
                workshopObject.Description = "Description for the created object";

                APEnvironment.ObjectMgr.AddObject(project.Handle, applicationObjectGuid, Guid.NewGuid(), workshopObject, "NewWorkshop", -1);
                // Finished. Return the prepared project.

                return project;
            }
            finally
            {
                undoMgr.EndCompoundAction();
            }
        }
    }
}
