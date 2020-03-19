using Barracuda;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MLAgents
{
    /// <summary>
    /// The Factory to generate policies.
    /// </summary>
    ///
    [AddComponentMenu("ML Agents/Behavior Parameters", (int)MenuGroup.Default)]
    public class BehaviorParameters : MonoBehaviour
    {
        [Serializable]
        public enum BehaviorType
        {
            Default,
            HeuristicOnly,
            InferenceOnly
        }

        [HideInInspector]
        [SerializeField]
        private BrainParameters m_BrainParameters = new BrainParameters();
        [HideInInspector]
        [SerializeField]
        private NNModel m_Model;
        [HideInInspector]
        [SerializeField]
        private InferenceDevice m_InferenceDevice;
        [HideInInspector]
        [SerializeField]
        private BehaviorType m_BehaviorType;
        [HideInInspector]
        [SerializeField]
        private string m_BehaviorName = "My Behavior";
        [HideInInspector]
        [SerializeField]
        public int m_TeamID;
        [FormerlySerializedAs("m_useChildSensors")]
        [HideInInspector]
        [SerializeField]
        [Tooltip("Use all Sensor components attached to child GameObjects of this Agent.")]
        private bool m_UseChildSensors = true;

        public BrainParameters brainParameters
        {
            get { return m_BrainParameters; }
        }

        public bool useChildSensors
        {
            get { return m_UseChildSensors; }
        }

        public string behaviorName
        {
            get { return m_BehaviorName; }
        }

        public BehaviorType Behavior
        {
            get { return m_BehaviorType; }
        }

        public NNModel Model
        {
            get { return m_Model; }
        }

        /// <summary>
        /// Returns the behavior name, concatenated with any other metadata (i.e. team id).
        /// </summary>
        public string fullyQualifiedBehaviorName
        {
            get { return m_BehaviorName + "?team=" + m_TeamID; }
        }

        internal IPolicy GeneratePolicy(Func<float[]> heuristic)
        {
            switch (m_BehaviorType)
            {
                case BehaviorType.HeuristicOnly:
                    return new HeuristicPolicy(heuristic);
                case BehaviorType.InferenceOnly:
                    return new BarracudaPolicy(m_BrainParameters, m_Model, m_InferenceDevice);
                case BehaviorType.Default:
                    if (Academy.Instance.IsCommunicatorOn)
                    {
                        return new RemotePolicy(m_BrainParameters, fullyQualifiedBehaviorName);
                    }
                    if (m_Model != null)
                    {
                        return new BarracudaPolicy(m_BrainParameters, m_Model, m_InferenceDevice);
                    }
                    else
                    {
                        return new HeuristicPolicy(heuristic);
                    }
                default:
                    return new HeuristicPolicy(heuristic);
            }
        }

        public void GiveModel(
            string newBehaviorName,
            NNModel model,
            InferenceDevice inferenceDevice = InferenceDevice.CPU)
        {
            m_Model = model;
            m_InferenceDevice = inferenceDevice;
            m_BehaviorName = newBehaviorName;
        }
    }
}
