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

using DustInTheWind.ConsoleTools.Modularization.Tests.Utils;

namespace DustInTheWind.ConsoleTools.Modularization.Tests.ModuleEngineTests;

public class Run_ModuleThatThrowsTests
{
    private readonly ModuleEngine moduleEngine = new();

    [Fact]
    public void HavingOneModuleThatThrows_WhenEngineIsRun_ThenEventIsRaised()
    {
        DummyModule module1 = CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw new Exception();
        };

        bool eventWasRaised = false;
        moduleEngine.ModuleRunException += (o, ev) =>
        {
            eventWasRaised = true;
            moduleEngine.RequestToClose();
        };

        RunEngineInTimeBox();

        eventWasRaised.Should().BeTrue();
    }

    [Fact]
    public void HavingOneModuleThatThrows_WhenEngineIsRun_ThenEventContainsException()
    {
        Exception thrownException = new();

        DummyModule module1 = CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw thrownException;
        };

        Exception actualException = null;
        moduleEngine.ModuleRunException += (o, ev) =>
        {
            actualException = ev.Exception;
            moduleEngine.RequestToClose();
        };

        RunEngineInTimeBox();

        actualException.Should().BeSameAs(thrownException);
    }

    [Fact]
    public void HavingOneModuleThatThrowsAndEventHandlerThatRequestsEngineClose_WhenEngineIsRun_ThenEngineIsClosed()
    {
        DummyModule module1 = CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw new Exception();
        };

        moduleEngine.ModuleRunException += (o, ev) =>
        {
            ev.CloseEngine = true;
        };

        RunEngineInTimeBox();

        // Engine is closed without reaching the timeout.
    }

    [Fact]
    public void HavingModuleThatThrowsAndEventHandlerThatRequestsSwitchToSecondModule_WhenEngineIsRun_ThenEngineIsRunningTheSecondModule()
    {
        DummyModule module1 = CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            throw new Exception();
        };

        DummyModule module2 = CreateDummyModule("mod2");
        module2.OnRun = () =>
        {
            moduleEngine.RequestToClose();
        };

        moduleEngine.ModuleRunException += (o, ev) =>
        {
            ev.NextModule = "mod2";
        };

        RunEngineInTimeBox();

        module1.RunCount.Should().Be(1);
        module2.RunCount.Should().Be(1);
    }

    [Fact]
    public void HavingOneModuleThatThrowsAndEventHandlerThatRequestsSwitchToNonexistentModule_WhenEngineIsRun_ThenEngineIsRunningTheDefaultModule()
    {
        DummyModule module1 = CreateDummyModule("mod1");
        int runCount = 0;
        module1.OnRun = () =>
        {
            runCount++;

            if (runCount == 1)
                throw new Exception();

            if (runCount == 2)
                moduleEngine.RequestToClose();
        };

        moduleEngine.ModuleRunException += (o, ev) =>
        {
            ev.NextModule = "inexistent";
        };

        RunEngineInTimeBox();

        module1.RunCount.Should().Be(2);
    }

    private DummyModule CreateDummyModule(string id)
    {
        DummyModule module1 = new(id);
        moduleEngine.AddModule(module1);
        return module1;
    }

    private void RunEngineInTimeBox(int maxExecutionTime = 100)
    {
        TimeBoxExecution.CreateNew(maxExecutionTime)
            .OnElapsedTime(() =>
            {
                moduleEngine.RequestToClose();
            })
            .Run(() =>
            {
                moduleEngine.Run();
            });
    }
}