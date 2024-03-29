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

public class Run_DefaultModuleTests
{
    private readonly ModuleEngineTestingContext testingContext = new();

    [Fact]
    public void HavingOneModuleInEngine_WhenEngineIsRun_ThenThatModuleIsRun()
    {
        DummyModule module1 = testingContext.CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            testingContext.ModuleEngine.RequestToClose();
        };

        testingContext.RunEngine();

        module1.RunCount.Should().Be(1);
    }

    [Fact]
    public void HavingTwoModulesInEngine_WhenEngineIsRun_ThenOnlyFirstModuleIsRun()
    {
        DummyModule module1 = testingContext.CreateDummyModule("mod1");
        module1.OnRun = () =>
        {
            testingContext.ModuleEngine.RequestToClose();
        };

        DummyModule module2 = testingContext.CreateDummyModule("mod2");

        testingContext.RunEngine();

        module1.RunCount.Should().Be(1);
        module2.RunCount.Should().Be(0);
    }

    [Fact]
    public void HavingTwoModulesInEngineAndSecondModuleAsDefault_WhenEngineIsRun_ThenOnlySecondModuleIsRun()
    {
        DummyModule module1 = testingContext.CreateDummyModule("mod1");

        DummyModule module2 = testingContext.CreateDummyModule("mod2");
        module2.OnRun = () =>
        {
            testingContext.ModuleEngine.RequestToClose();
        };

        testingContext.ModuleEngine.SetDefaultModule("mod2");

        testingContext.RunEngine();

        module1.RunCount.Should().Be(0);
        module2.RunCount.Should().Be(1);
    }
}