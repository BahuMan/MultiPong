using System;
using UnityEngine;
using Procurios.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace MultiPong.Unittest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPlayerInfo()
        {
            var playa = new PongPlayer("id of player");
            playa.goalLeft = new Vector2(1f, 2f);
            playa.goalRight = new Vector2(3f, 4f);
            playa.length = 5f;
            playa.height = 6f;
            string msg = playa.toJSON();
            object boe = JSON.JsonDecode(msg);

            PongPlayer second = new PongPlayer((Hashtable) boe);
            string msg2 = second.toJSON();

            Assert.AreEqual(msg, msg2);
        }

        [TestMethod]
        public void TestBallInfo()
        {
            var ball1 = new PongBall();
            ball1.ballid = "id of ball";
            ball1.position = new Vector2(1f, 2f);
            ball1.velocity = new Vector2(3f, 4f);
            ball1.diameter = 5f;
            string msg = PongSerializer.forBallMove(ball1);
            object boe = JSON.JsonDecode(msg);

            PongBall second = new PongBall();
            second = PongSerializer.fromBallMove(second, (Hashtable) boe);
            string msg2 = PongSerializer.forBallMove(second);

            Assert.AreEqual(msg, msg2);
        }
    }
}
