using UnityEngine;
using UnityEngine.AI;

namespace LazyPanClean {
    public class LPAgentUtil : LPSingleton<LPAgentUtil> {
        public NavMeshPath GetAgentPath(NavMeshAgent agent, Vector3 targetPoint) {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(targetPoint, path);
            return path;
        }
    }
}
