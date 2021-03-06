﻿using UnityEngine;
using UnityEditor;
using System;
using NUnit.Framework;
using Unidux;

public class StoreTest
{
    [Test]
    public void RenderSubscribeTest()
    {
        var store = new Store<State>(new State());
        var render = new SampleRender();
        store.AddRenderer(render.Render);

        store.Dispatch(new Action());
        store.ForceUpdate();
        Assert.AreEqual(1, render.Count);

        store.Dispatch(new Action());
        store.ForceUpdate();
        Assert.AreEqual(2, render.Count);

        store.RemoveRenderer(render.Render);

        store.Dispatch(new Action());
        store.ForceUpdate();
        Assert.AreEqual(2, render.Count);
    }

    [Test]
    public void RenderMultipleSubscribeTest()
    {
        var store = new Store<State>(new State());
        var count1 = 0;
        var count2 = 0;
        Unidux.Renderer<State> render1 = (State state) =>
        {
            count1++;
        };
        Unidux.Renderer<State> render2 = (State state) =>
        {
            count2++;
        };

        store.AddRenderer(render1);
        store.AddRenderer(render2);

        store.ForceUpdate();
        Assert.AreEqual(1, count1);
        Assert.AreEqual(1, count2);

        store.RemoveRenderer(render1);

        store.ForceUpdate();
        Assert.AreEqual(1, count1);
        Assert.AreEqual(2, count2);

        store.RemoveRenderer(render2);

        store.ForceUpdate();
        Assert.AreEqual(1, count1);
        Assert.AreEqual(2, count2);
    }

    [Test]
    public void ReducerSubscribeTest()
    {
        var store = new Store<State>(new State());
        var reducer = new SampleReducer();
        store.AddReducer<Action>(reducer.Reduce);

        store.Dispatch(new Action());
        Assert.AreEqual(1, reducer.Count);

        store.Dispatch(new Action());
        Assert.AreEqual(2, reducer.Count);

        store.RemoveReducer<Action>(reducer.Reduce);
        store.Dispatch(new Action());
        Assert.AreEqual(2, reducer.Count);
    }

    [Test]
    public void ResetStateChangedTest()
    {
        var store = new Store<State>(new State());
        var count = 0;

        store.AddRenderer((state) =>
        {
            count++;
            Assert.IsTrue(state.Changed.IsStateChanged());
        });

        Assert.IsFalse(store.State.Changed.IsStateChanged());
        store.State.Changed.SetStateChanged();

        store.ForceUpdate();
        Assert.IsFalse(store.State.Changed.IsStateChanged());
        Assert.AreEqual(1, count);
    }

    class State : StateBase<State>
    {
        public ChangedState Changed { get; set; }

        public State()
        {
            this.Changed = new ChangedState();
        }

        public override State Clone()
        {
            var state = (State)MemberwiseClone();
            state.Changed = state.Changed.Clone();
            return state;
        }
    }

    class ChangedState : StateElement<ChangedState>
    {
    }

    class Action
    {
    }

    class SampleRender
    {
        public int Count = 0;

        public void Render(State state)
        {
            Count++;
        }
    }

    class SampleReducer
    {
        public int Count = 0;

        public State Reduce(State state, Action action)
        {
            Count++;
            return state;
        }
    }
}
