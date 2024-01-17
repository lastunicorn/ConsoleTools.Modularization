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

namespace DustInTheWind.ConsoleTools.Modularization.Tests.ModuleEngineTests;

public class SetDefaultModuleTests
{
    private readonly ModuleEngine moduleEngine = new();

    [Fact]
    public void HavingNoModulesInEngine_WhenSettingDefaultModule_ThenThrows()
    {
        Action action = () =>
        {
            moduleEngine.SetDefaultModule("mod1");
        };

        action.Should().Throw<ModuleNotFoundException>();
    }

    [Fact]
    public void HavingOneModulesInEngine_WhenSettingDefaultModuleToExistingModule_ThenDoesNotThrow()
    {
        AddModule("mod1");

        Action action = () =>
        {
            moduleEngine.SetDefaultModule("mod1");
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void HavingOneModulesInEngine_WhenSettingDefaultModuleToNonExistingModule_ThenThrows()
    {
        AddModule("mod1");

        Action action = () =>
        {
            moduleEngine.SetDefaultModule("mod2");
        };

        action.Should().Throw<ModuleNotFoundException>();
    }

    private void AddModule(ModuleId id)
    {
        DummyModule module = new(id);
        moduleEngine.AddModule(module);
    }
}