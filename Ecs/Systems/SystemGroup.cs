using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace CursedCreatives.Ecs
{
    [Serializable, HideReferenceObjectPicker]
    public class SystemGroup
    {
        private readonly World _world;

        [Title("@GetType().Name")]
        [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true), OdinSerialize]
        private readonly List<IStartSystem> _startSystems;

        [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true), OdinSerialize]
        private readonly List<SystemDisplay<IUpdateSystem>> _updateSystems;

        [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true), OdinSerialize]
        private readonly List<SystemDisplay<IFixedSystem>> _fixedSystems;

        [ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true), OdinSerialize]
        private readonly List<SystemDisplay<ILateSystem>> _lateSystems;

        public SystemGroup(World world)
        {
            _world = world;
            _startSystems = new List<IStartSystem>();
            _updateSystems = new List<SystemDisplay<IUpdateSystem>>();
            _fixedSystems = new List<SystemDisplay<IFixedSystem>>();
            _lateSystems = new List<SystemDisplay<ILateSystem>>();
        }

        public void AddStart(IStartSystem system)
        {
            system.World = _world;
            _startSystems.Add(system);
            system.Start();
        }

        public void AddUpdate(IUpdateSystem system)
        {
            system.World = _world;
            _updateSystems.Add(new SystemDisplay<IUpdateSystem>(true, system));
            system.Start();
        }

        public void AddFixed(IFixedSystem system)
        {
            system.World = _world;
            _fixedSystems.Add(new SystemDisplay<IFixedSystem>(true, system));
            system.Start();
        }

        public void AddLate(ILateSystem system)
        {
            system.World = _world;
            _lateSystems.Add(new SystemDisplay<ILateSystem>(true, system));
            system.Start();
        }

        internal void Update(float deltaTime)
        {
            foreach (var display in _updateSystems)
            {
#if UNITY_EDITOR
                Stopwatch watch = Stopwatch.StartNew();
                if (display.enabled) display.system.Update(deltaTime);
                watch.Stop();
                double ticks = watch.ElapsedTicks;
                display.executionTime = ticks / Stopwatch.Frequency * 1000;
#else
                display.system.Update(deltaTime);
#endif
            }
        }

        internal void FixedUpdate(float fixedDeltaTime)
        {
            foreach (var display in _fixedSystems)
            {
#if UNITY_EDITOR
                Stopwatch watch = Stopwatch.StartNew();
                if (display.enabled) display.system.FixedUpdate(fixedDeltaTime);
                watch.Stop();
                double ticks = watch.ElapsedTicks;
                display.executionTime = ticks / Stopwatch.Frequency * 1000;
#else
                display.system.FixedUpdate(fixedDeltaTime);
#endif
            }
        }

        internal void LateUpdate(float deltaTime)
        {
            foreach (var display in _lateSystems)
            {
#if UNITY_EDITOR
                Stopwatch watch = Stopwatch.StartNew();
                if (display.enabled) display.system.LateUpdate(deltaTime);
                watch.Stop();
                double ticks = watch.ElapsedTicks;
                display.executionTime = ticks / Stopwatch.Frequency * 1000;
#else
                display.system.LateUpdate(deltaTime);
#endif
            }
        }
    }
}