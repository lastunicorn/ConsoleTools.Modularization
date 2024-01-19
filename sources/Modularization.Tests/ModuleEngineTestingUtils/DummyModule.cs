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

namespace DustInTheWind.ConsoleTools.Modularization.Tests.ModuleEngineTestingUtils;

internal sealed class DummyModule : IModule, IDisposable
{
    private readonly ManualResetEventSlim manualResetEventSlim = new();

    public ModuleId Id { get; }

    public ModuleEngine ModuleEngine { get; set; }

    public int RunCount { get; private set; }

    public int RequestExitCount { get; private set; }

    public Action OnRun { get; set; }

    public DummyModule(ModuleId id)
    {
        Id = id;
    }

    public void Run()
    {
        RunCount++;

        manualResetEventSlim.Reset();
        OnRun?.Invoke();

        manualResetEventSlim.Wait();
    }

    public void RequestExit()
    {
        RequestExitCount++;
        manualResetEventSlim.Set();
    }

    public override string ToString()
    {
        return Id;
    }

    public void Dispose()
    {
        manualResetEventSlim?.Dispose();
    }
}