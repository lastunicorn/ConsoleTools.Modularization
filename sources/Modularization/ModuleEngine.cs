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

namespace DustInTheWind.ConsoleTools.Modularization;

public class ModuleEngine
{
    private readonly ModuleCollection modules;

    private IModule currentModule;
    private IModule defaultModule;
    private volatile bool closeWasRequested;
    private IModule requestedModule;
    private ModuleEngineState state;

    public event EventHandler<ModuleRunExceptionEventArgs> ModuleRunException;

    public ModuleEngine()
    {
        modules = new ModuleCollection(this);
    }

    public ModuleEngine(IEnumerable<IModule> modules)
    {
        if (modules == null) throw new ArgumentNullException(nameof(modules));

        this.modules = new ModuleCollection(this);
        this.modules.AddRange(modules);
    }

    public void AddModule(IModule module)
    {
        if (state == ModuleEngineState.Running)
            throw new EngineRunningException();

        if (module == null) throw new ArgumentNullException(nameof(module));

        modules.Add(module);
    }

    public void SetDefaultModule(ModuleId moduleId)
    {
        if (state == ModuleEngineState.Running)
            throw new EngineRunningException();

        defaultModule = modules.GetModuleOrThrow(moduleId);
    }

    public void Run()
    {
        if (state == ModuleEngineState.Running)
            throw new EngineRunningException();

        state = ModuleEngineState.Running;

        try
        {
            RunInternal();
        }
        finally
        {
            state = ModuleEngineState.StandBy;
        }
    }

    private void RunInternal()
    {
        closeWasRequested = false;
        requestedModule = GetDefaultModule();

        while (!closeWasRequested)
        {
            currentModule = ChooseNextModule();

            try
            {
                currentModule.Run();
            }
            catch (Exception ex)
            {
                ModuleRunExceptionEventArgs eventArgs = new(ex);
                OnModuleRunException(eventArgs);

                if (eventArgs.CloseEngine)
                {
                    closeWasRequested = true;
                    requestedModule = null;
                }
                else
                {
                    requestedModule = eventArgs.NextModule.IsEmpty
                        ? GetDefaultModule()
                        : modules.GetModuleOrNull(eventArgs.NextModule) ?? GetDefaultModule();
                }
            }
        }
    }

    private IModule ChooseNextModule()
    {
        if (requestedModule != null)
        {
            IModule nextModule = requestedModule;
            requestedModule = null;
            return nextModule;
        }

        return GetDefaultModule();
    }

    private IModule GetDefaultModule()
    {
        if (defaultModule != null)
            return defaultModule;

        IModule firstModule = modules.FirstOrDefault();
        return firstModule ?? throw new NoModulesExistException();
    }

    public void RequestToChangeModule(ModuleId moduleId)
    {
        requestedModule = modules.GetModuleOrThrow(moduleId);
        currentModule?.RequestExit();
    }

    public void RequestToClose()
    {
        closeWasRequested = true;
        requestedModule = null;
        currentModule?.RequestExit();
    }

    protected virtual void OnModuleRunException(ModuleRunExceptionEventArgs e)
    {
        ModuleRunException?.Invoke(this, e);
    }
}