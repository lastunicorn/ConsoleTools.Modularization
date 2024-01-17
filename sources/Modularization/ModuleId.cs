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

public readonly struct ModuleId
{
    private readonly string value;

    public bool IsEmpty => value == null;

    public static ModuleId Empty { get; } = new();

    public ModuleId(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }

    public static implicit operator string(ModuleId moduleId)
    {
        return moduleId.value;
    }

    public static implicit operator ModuleId(string moduleId)
    {
        return new ModuleId(moduleId);
    }
}