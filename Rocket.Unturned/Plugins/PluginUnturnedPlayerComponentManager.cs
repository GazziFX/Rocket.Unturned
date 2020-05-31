﻿using Rocket.API;
using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Rocket.Unturned.Plugins
{
    public sealed class PluginUnturnedPlayerComponentManager : MonoBehaviour
    {
        private Assembly assembly;
        private List<Type> unturnedPlayerComponents = new List<Type>();
        
        private void OnDisable()
        {
            try
            {
                U.Events.OnPlayerConnected -= addPlayerComponents;
                var list = new List<Type>();
                foreach (var p in unturnedPlayerComponents)
                {
                    if (p.Assembly != assembly)
                        list.Add(p);
                }
                unturnedPlayerComponents = list;
                /*List<Type> playerComponents = RocketHelper.GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent));
                foreach (Type playerComponent in playerComponents)
                {
                    //Provider.Players.ForEach(p => p.Player.gameObject.TryRemoveComponent(playerComponent.GetType()));
                }*/
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex);
            }
        }

        private void OnEnable()
        {
            try
            {  
                IRocketPlugin plugin = GetComponent<IRocketPlugin>();
                assembly = plugin.GetType().Assembly;

                U.Events.OnBeforePlayerConnected += addPlayerComponents;
                unturnedPlayerComponents.AddRange(RocketHelper.GetTypesFromParentClass(assembly, typeof(UnturnedPlayerComponent)));

                foreach (Type playerComponent in unturnedPlayerComponents)
                {
                    Core.Logging.Logger.Log("Adding UnturnedPlayerComponent: "+playerComponent.Name);
                    //Provider.Players.ForEach(p => p.Player.gameObject.TryAddComponent(playerComponent.GetType()));
                }
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex);
            }
        }

        private void addPlayerComponents(IRocketPlayer p)
        {
            foreach (Type component in unturnedPlayerComponents)
            {
                ((UnturnedPlayer)p).Player.gameObject.AddComponent(component);
            }
        }
    }
}