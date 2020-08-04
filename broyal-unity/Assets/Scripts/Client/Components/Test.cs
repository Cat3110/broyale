using Unity.Entities;
using Unity.Collections;

[GenerateAuthoringComponent]
public struct Test : IComponentData {
public NativeString32 test;
}
