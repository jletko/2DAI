// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: mlagents/envs/communicator_objects/space_type_proto.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace MLAgents.CommunicatorObjects {

  /// <summary>Holder for reflection information generated from mlagents/envs/communicator_objects/space_type_proto.proto</summary>
  public static partial class SpaceTypeProtoReflection {

    #region Descriptor
    /// <summary>File descriptor for mlagents/envs/communicator_objects/space_type_proto.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SpaceTypeProtoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjltbGFnZW50cy9lbnZzL2NvbW11bmljYXRvcl9vYmplY3RzL3NwYWNlX3R5",
            "cGVfcHJvdG8ucHJvdG8SFGNvbW11bmljYXRvcl9vYmplY3RzGjltbGFnZW50",
            "cy9lbnZzL2NvbW11bmljYXRvcl9vYmplY3RzL3Jlc29sdXRpb25fcHJvdG8u",
            "cHJvdG8qLgoOU3BhY2VUeXBlUHJvdG8SDAoIZGlzY3JldGUQABIOCgpjb250",
            "aW51b3VzEAFCH6oCHE1MQWdlbnRzLkNvbW11bmljYXRvck9iamVjdHNiBnBy",
            "b3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::MLAgents.CommunicatorObjects.ResolutionProtoReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::MLAgents.CommunicatorObjects.SpaceTypeProto), }, null));
    }
    #endregion

  }
  #region Enums
  public enum SpaceTypeProto {
    [pbr::OriginalName("discrete")] Discrete = 0,
    [pbr::OriginalName("continuous")] Continuous = 1,
  }

  #endregion

}

#endregion Designer generated code
