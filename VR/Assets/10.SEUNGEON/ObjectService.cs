
// ProcedureSystem/Core/Services/ObjectService.cs
using Oculus.Interaction;
using System.Collections.Generic;

public class ObjectService : IObjectService
{
    private readonly Dictionary<string, IInteractable> _interactables = new Dictionary<string, IInteractable>();
    private readonly Dictionary<string, IStatefulObject> _statefulObjects = new Dictionary<string, IStatefulObject>();

    public void RegisterObject(IInteractable obj)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ObjectId))
        {
            _interactables[obj.ObjectId] = obj;
        }
    }

    public void RegisterObject(IStatefulObject obj)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ObjectId))
        {
            _statefulObjects[obj.ObjectId] = obj;
        }
    }

    public IInteractable GetInteractable(string id) =>
        _interactables.TryGetValue(id, out var obj) ? obj : null;

    public IStatefulObject GetStatefulObject(string id) =>
        _statefulObjects.TryGetValue(id, out var obj) ? obj : null;

    public List<IInteractable> GetAllInteractables() =>
        new List<IInteractable>(_interactables.Values);

    public List<IStatefulObject> GetAllStatefulObjects() =>
        new List<IStatefulObject>(_statefulObjects.Values);
}
