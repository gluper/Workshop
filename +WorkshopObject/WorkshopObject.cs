using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopObject
{
    // [ReleasedInterface]
    public interface IWorkshopObject : IObject
    {
        string Text { get; set; }
    }

    public interface IWorkshopObject2 : IWorkshopObject
    {
        string Description { get; set; }
    }


    // [ReleasedInterface] for Interfaces, [ReleasedEnum] for Enumeration
}
