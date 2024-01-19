// Console Tools Modularization
// Copyright (C) 2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.ConsoleTools.Modularization.Tests.ModuleEngineTestingUtils;

namespace DustInTheWind.ConsoleTools.Modularization.Tests.ModuleEngineTests;

public class Run_ModuleThatThrowsTests
{
    private readonly ModuleEngineTestingContext testingContext = new();

    [Fact]
    public void HavingOneModuleThatThrows_WhenEngineIsRun_ThenEventIsRaised()
    {
        DummyModule module1 = testingContext.CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw new Exception();
        };

        bool eventWasRaised = false;
        testingContext.ModuleEngine.ModuleRunException += (o, ev) =>
        {
            eventWasRaised = true;
            testingContext.ModuleEngine.RequestToClose();
        };

        testingContext.RunEngine();

        eventWasRaised.Should().BeTrue();
    }

    [Fact]
    public void HavingOneModuleThatThrows_WhenEngineIsRun_ThenEventContainsException()
    {
        Exception thrownException = new();

        DummyModule module1 = testingContext.CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw thrownException;
        };

        Exception actualException = null;
        testingContext.ModuleEngine.ModuleRunException += (o, ev) =>
        {
            actualException = ev.Exception;
            testingContext.ModuleEngine.RequestToClose();
        };

        testingContext.RunEngine();

        actualException.Should().BeSameAs(thrownException);
    }

    [Fact]
    public void HavingOneModuleThatThrowsAndEventHandlerThatRequestsEngineClose_WhenEngineIsRun_ThenEngineIsClosed()
    {
        DummyModule module1 = testingContext.CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw new Exception();
        };

        testingContext.ModuleEngine.ModuleRunException += (o, ev) =>
        {
            ev.CloseEngine = true;
        };

        testingContext.RunEngine();

        // Engine is closed without reaching the timeout.
    }

    [Fact]
    public void HavingModuleThatThrowsAndEventHandlerThatRequestsSwitchToSecondModule_WhenEngineIsRun_ThenEngineIsRunningTheSecondModule()
    {
        DummyModule module1 = testingContext.CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw new Exception();
        };

        DummyModule module2 = testingContext.CreateDummyModule("mod2");
        module2.OnRun = () =>
        {
            testingContext.ModuleEngine.RequestToClose();
        };

        testingContext.ModuleEngine.ModuleRunException += (o, ev) =>
        {
            ev.NextModule = "mod2";
        };

        testingContext.RunEngine();

        module1.RunCount.Should().Be(1);
        module2.RunCount.Should().Be(1);
    }

    [Fact]
    public void HavingOneModuleThatThrowsAndEventHandlerThatRequestsSwitchToNonexistentModule_WhenEngineIsRun_ThenEngineIsRunningTheDefaultModule()
    {
        DummyModule module1 = testingContext.CreateDummyModule("mod1");
        int runCount = 0;
        module1.OnRun = () =>
        {
            runCount++;

            if (runCount == 1)
                throw new Exception();

            if (runCount == 2)
                testingContext.ModuleEngine.RequestToClose();
        };

        testingContext.ModuleEngine.ModuleRunException += (o, ev) =>
        {
            ev.NextModule = "inexistent";
        };

        testingContext.RunEngine();

        module1.RunCount.Should().Be(2);
    }
}