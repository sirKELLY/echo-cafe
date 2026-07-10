using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    // this is the base class for a work station
    /* a station will do the following
     * allow character to interact
     * make an item after a delay
     * it allows multiple users
     * each item is produced independently
     * each user has an independent timer
     *
     * example coffee station will take 10 seconds to produce a coffee
     * characters are the player and the echoes
     */
    public class WorkStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private StationInfo info;

        // independent per-user progress: the player and each echo craft simultaneously
        private readonly Dictionary<ExecuteBehaviour, float> _progress = new();

        // called every FixedUpdate a character is engaged with this station; returns progress 0..1
        public float Interact(ExecuteBehaviour user)
        {
            // can't craft with full hands: deliver or drop what you're carrying first
            if (user.IsHoldingItem) return 0f;

            _progress.TryGetValue(user, out float t);
            t += Time.fixedDeltaTime;

            if (t >= info.craftTime)
            {
                GiveItem(user);
                _progress.Remove(user);
                return 1f; // completed this frame (starts over next frame if still engaged)
            }

            _progress[user] = t;
            return t / info.craftTime;
        }

        // leaving range / releasing interact cancels this user's craft
        public void StopInteract(ExecuteBehaviour user)
        {
            _progress.Remove(user);
        }

        private void GiveItem(ExecuteBehaviour user)
        {
            // the crafter stayed the whole time and has empty hands, so they receive it directly
            user.ReceiveItem(info.output);
            Debug.Log($"{user.name} finished crafting {info.output.displayName} at {name}");
        }
    }
}