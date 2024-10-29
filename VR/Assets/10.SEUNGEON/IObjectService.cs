// ProcedureSystem/Core/Services/IObjectService.cs
using Oculus.Interaction;
using System.Collections.Generic;

public interface IObjectService
{
    void RegisterObject(IInteractable obj);
    void RegisterObject(IStatefulObject obj);
    IInteractable GetInteractable(string id);
    IStatefulObject GetStatefulObject(string id);
    List<IInteractable> GetAllInteractables();
    List<IStatefulObject> GetAllStatefulObjects();
}
