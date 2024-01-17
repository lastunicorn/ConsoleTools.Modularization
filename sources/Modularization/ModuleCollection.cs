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

using System.Collections.ObjectModel;

namespace DustInTheWind.ConsoleTools.Modularization;

internal class ModuleCollection : Collection<IModule>
{
    private readonly ModuleEngine moduleEngine;

    public ModuleCollection(ModuleEngine moduleEngine)
    {
        this.moduleEngine = moduleEngine ?? throw new ArgumentNullException(nameof(moduleEngine));
    }

    public void AddRange(IEnumerable<IModule> modules)
    {
        foreach (IModule module in modules)
        {
            if (module == null)
                throw new ArgumentException("A null module cannot be added to the collection.", nameof(modules));

            module.ModuleEngine = moduleEngine;
            Items.Add(module);
        }
    }


    public IModule GetModuleOrNull(ModuleId id)
    {
        foreach (IModule module in Items)
        {
            if (module.Id == id)
                return module;
        }

        return null;
    }

    public IModule GetModuleOrThrow(ModuleId id)
    {
        foreach (IModule module in Items)
        {
            if (module.Id == id)
                return module;
        }

        throw new ModuleNotFoundException(id);
    }
}