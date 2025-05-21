using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour
{
   CharacterManager character;

   [Header("Position")]
   public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
   public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


   public Vector3 networkPositionVelocity;

   public float networkPositionSmoothTime = 0.1f;
   public float networkRotationSmoothTime = 0.1f;

   [Header("Animator")]
   public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

   public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
   public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

   [Header("Flags")]
   public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

   [Header("Resources")]
   public NetworkVariable<float> currStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
   public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
   public NetworkVariable<int> currHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
   public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

   [Header("Stats")]

   public NetworkVariable<int> fortitude = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
   public NetworkVariable<int> life = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);






   protected virtual void Awake()
   {
      character = GetComponent<CharacterManager>();
   }

   public void CheckHP(int oldValue, int newValue)
   {
      if (currHealth.Value <= 0)
      {
         StartCoroutine(character.ProcessDeathEvent());
      }
      //PREVENTS US FROM OVER HEALING
      if (character.IsOwner)
      {
         if (currHealth.Value > maxHealth.Value)
         {
            currHealth.Value = maxHealth.Value;
         }
      }
   }

   // A server rpc is a function called from a client , to the server (in our ce the host)
   [ServerRpc]
   public void NotifyTheServerOfActionAnimationServerRpc(ulong clientId, string animationId, bool applyRootMotion)
   {

      // if this character is the host/server, then activate the client rpc
      if (IsServer)
      {
         PlayActionAnimationForAllClientsClientRpc(clientId, animationId, applyRootMotion);
      }
   }

   // a client rpc is sent to all clients present in the server
   [ClientRpc]
   public void PlayActionAnimationForAllClientsClientRpc(ulong clientId, string animationId, bool applyRootMotion)
   {
      // we make sure to not run the function on the character who set it (so we dont play the animation twice)
      if (clientId != NetworkManager.Singleton.LocalClientId)
      {
         PerformActionAnimationFromServer(animationId, applyRootMotion);
      }
   }

   private void PerformActionAnimationFromServer(string animationId, bool applyRootMotion)
   {
      character.animator.applyRootMotion = applyRootMotion;
      character.animator.CrossFade(animationId, 0.2f);
   }
}
