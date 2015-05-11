using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using STAR;
using SharpDX;

namespace Lab01
{
    [TestClass]
    public class SurfaceCollectionTests
    {
        [TestMethod]
        public void TestMethod1()
        {

            int w = 2, h = 2;
            int area = w * h;

            SurfaceCollection grid = new SurfaceCollection(w, h);

            if (grid.Length != area)
            {
                Assert.Fail("grid is not the correct size. should be " + area + ". is " + grid.Length);
                return;
            }

            int counter = 0;
            grid.ForAll(a => 
            {
                a = new Surface( counter);
                counter++;
            });



           


        }
    }
}
