using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopObject
{
    public interface IWorkshopObject : IObject
    {
        string Text { get; set; }
    }
}
