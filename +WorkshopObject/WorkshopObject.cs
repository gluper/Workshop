using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopObject
{
    [ReleasedInterface]
    public interface IWorkshopObject : IObject
    {
        string Text { get; set; }

    }

    [ReleasedInterface]
    public interface IWorkshopObject2 : IWorkshopObject
    {
        string Description { get; set; }
        void AddDetails(string st, int v);
    }

    public interface IWorkshopObject3 : IWorkshopObject2
    { 
        void AddDetails(string st, int v, int x);
    }


    // [ReleasedInterface] for Interfaces, [ReleasedEnum] for Enumeration
}
