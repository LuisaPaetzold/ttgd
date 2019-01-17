﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor.Animations;

public class Test_PMTreasureChest
{
    [UnityTest]
    public IEnumerator Test_ChestOpensIfPlayerCloseEnough()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest();
        chest.player = player;
        chest.transform.position = new Vector3(chest.openDistance + 1, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(chest.isOpen, "Treasure chest started open instead of closed!");

        player.transform.position = new Vector3(chest.openDistance, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(chest.isOpen, "Treasure chest did not open when player approached!");

    }

    [UnityTest]
    public IEnumerator Test_ChestPlaysAnimationAfterOpening()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest(true);
        chest.player = player;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(chest.animator, "There was no animator added to the chest!");
        Assert.IsNotNull(chest.animator.runtimeAnimatorController, "There was no animator controller added to the chest's animator!");

        chest.transform.position = new Vector3(chest.openDistance - 1, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(chest.animator.IsInTransition(0), "Animator was not in transition to next animation!");
        
        yield return new WaitForSeconds(chest.animator.GetAnimatorTransitionInfo(0).duration);

        Assert.IsTrue(chest.animator.GetCurrentAnimatorStateInfo(0).IsName("Open"), "Current animation did not have a matching name!");
        Assert.IsTrue(chest.animator.GetCurrentAnimatorStateInfo(0).IsTag("Open"), "Current animation did not have a matching tag!");

        // Here we can also test animation speed, playback time, and more. WRITE
    }

    [UnityTest]
    public IEnumerator Test_ChestOpenAnimationIsPlayingAndNotLooping()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest(true);
        chest.player = player;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(chest.animator, "There was no animator added to the chest!");
        Assert.IsNotNull(chest.animator.runtimeAnimatorController, "There was no animator controller added to the chest's animator!");

        chest.transform.position = new Vector3(chest.openDistance - 1, 0, 0);

        yield return new WaitForSeconds(chest.animator.GetAnimatorTransitionInfo(0).duration);

        Assert.IsFalse(chest.animator.GetCurrentAnimatorStateInfo(0).loop, "Open animation was looping but shouldn't!");
        Assert.AreEqual(1, chest.animator.GetCurrentAnimatorStateInfo(0).speed, "Open animation speed was not 1!");
    }


        // --------------------- helper methods ----------------------------------------

        public Player CreatePlayer(bool setUpComponentsInTest = true)
    {
        Player p = new GameObject().AddComponent<Player>();
        if (setUpComponentsInTest)
        {
            p.stats = new PlayerStatsClass();
            p.inventory = new PlayerInventoryClass();
        }
        return p;
    }

    public TreasureChest CreateChest(bool addAnimator = false)
    {
        TreasureChest t = new GameObject().AddComponent<TreasureChest>();

        if (addAnimator)
        {
            Animator anim = t.gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/AssetStore/Low Poly Toon Chests/Chest.controller");
        }

        return t;
    }
}
