using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkingCart.Patches
{
    class CartRoastSync : MonoBehaviourPun
    {
        public CartTalkingManager cart;

        public void AttemptRoast()
        {
            if (cart.cartVoiceQueue.Count > 0) return;
            float chanceOfReacting = UnityEngine.Random.value;
            if (chanceOfReacting >= ConfigManager.cartChanceToReactToDamagingItems.Value) return;

            // Choosing track.
            int rand = UnityEngine.Random.Range(0, TalkingCartBase.RoastsFX.Count);

            int numberOfPlayers = FindObjectsOfType<PlayerAvatar>().Length;

            while ((rand == 34 || rand == 35) && numberOfPlayers > 1)
            {
                rand = UnityEngine.Random.Range(0, TalkingCartBase.RoastsFX.Count);
            }

            List<int> inds = new List<int> ();
            List<float> delays = new List<float> ();

            if (rand == 36)
            {
                int clownNearbyInd = TalkingCartBase.EnemyNearbyInd + 4;

                inds.Add(-1);
                delays.Add(2);
            }
            inds.Add(rand);
            delays.Add(0);

            if (SemiFunc.IsMultiplayer())
                base.photonView.RPC("AttemptRoastRPC", RpcTarget.All, inds.ToArray(), delays.ToArray());
            else
                AttemptRoastRPC(inds.ToArray(), delays.ToArray());
        }

        [PunRPC]
        void AttemptRoastRPC(int[] inds, float[] delays)
        {
            for (int i = 0; i < inds.Length; i++)
            {
                int ind = inds[i];
                if(ind == -1)
                {
                    // Clown line.
                    int clownNearbyInd = TalkingCartBase.EnemyNearbyInd + 4;
                    cart.EnqueueValues(TalkingCartBase.SoundFX[clownNearbyInd], delays[i], "Clown nearby");
                } else
                {
                    cart.EnqueueValues(TalkingCartBase.RoastsFX[ind], delays[i], TalkingCartBase.RoastsText[ind]);
                }
            }
        }
    }
}
