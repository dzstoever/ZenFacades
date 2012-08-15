//using System;
//using FluentAssertions;
//using Xbehave;
//using Xunit;
//using Xunit.Extensions;
//using Moq;

//namespace Zen.Xunit.Tests
//{
     
//    public class XunitScenarios : UseLogFixture
//    {       
//        [Scenario]
//        public virtual void DoSomething()
//        {
//            Console.WriteLine("I am not in the output");
            
//            "Given some arranged preconditions".Given(() =>
//            {   Console.WriteLine("-arrange-");
//                Log.InfoFormat("given 1");
                
//            }, () => 
//                    {   Console.WriteLine("-dispose-");
//                        Log.InfoFormat("release 1");                    
//                    }
//            );
//            "When the code under test runs".When(() =>
//            {   Console.WriteLine("-act-");
//                Log.InfoFormat("assumming anything");
                
//            });
//            "Then the actual results meet expectations".Then(() =>
//            {   Console.WriteLine("-assert-");
//                Log.InfoFormat("then you are an ass");                
//            });
//        }        
//    }
    
    /*    
    public class xSimpleScenario
    {
        [Scenario]
        public virtual void DoSomething()
        {
            int? x = null;
            "Given x=1 ".Given(() => x = 1);
            "When adding 1 ".When(() => x += 1);
            "Then the result is 2".Then(() => x.Should().Be(2));
        }
    }

    public class xDisposeScenario
    {
        [Scenario]
        public virtual void DoSomething_Dispose()
        {
            Console.WriteLine("I am not in the output");

            "Given some arranged preconditions".Given(() =>
            {Console.WriteLine("-arrange-");                
            }, Dispose //() => { Console.WriteLine("-dispose- (lambda)"); }
            );

            "When the code under test runs".When(() =>
            {Console.WriteLine("-act-");
            });

            "Then the actual results meet expectations".Then(() =>
            {Console.WriteLine("-assert-");
            });
        }

        private void Dispose()
        { Console.WriteLine("-dispose- (method)");
        }
    }
    */
//}
