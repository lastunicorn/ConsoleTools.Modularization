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

namespace DustInTheWind.ConsoleTools.Modularization.Tests.ModuleEngineTestingUtils;

internal class ModuleEngineTestingContext
{
    public ModuleEngine ModuleEngine { get; } = new();

    public DummyModule CreateDummyModule(string id)
    {
        DummyModule module1 = new(id);
        ModuleEngine.AddModule(module1);
        return module1;
    }

    public void RunEngine(int maxExecutionTime = 100)
    {
        TimeBoxExecution.CreateNew(maxExecutionTime)
            .OnElapsedTime(() =>
            {
                ModuleEngine.RequestToClose();
            })
            .Run(() =>
            {
                ModuleEngine.Run();
            });
    }
}